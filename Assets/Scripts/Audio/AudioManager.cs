using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using SimpleJSON;
using System.IO;
using Debug = FFP.Debug;

/*!
 *	Manages audio loading and playing for all audio types. This class also handles preloading of priority audio.
 *	written by Ace Spring 2015
 */
[EventVisibleAttribute]
public class AudioManager : MonoBehaviour {

	#region singletonEnforcement
	private static AudioManager _instance;

	public static AudioManager Instance {
		get {
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<AudioManager>();

				//Dont distroy on new scene load
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}
	#endregion

	const string soundListFilePath = "/Resources/audioListData.json";		//Path for the audio json file

	[SerializeField]
	private AudioMixer masterMixer;		//!Location for the master mixer from scene
	[SerializeField]
	private AudioMixer ambienceMixer;	//!Location for the ambience mixer from scene
	[SerializeField]
	private AudioMixer effectMixer;		//!Location for the effect mixer from scene
	[SerializeField]
	private AudioMixer musicMixer;		//!Location for the music mixer from scene
	[SerializeField]
	private AudioMixer voiceMixer;		//!Location for the voice mixer from scene

	[Range(1.0f, 100.0f)]
	public float editorVol = 60f;		//!Used for testing volume or indirect vol control

	//Dictionaries that hold the file names from json file
	Dictionary<string, bool> ambienceDict = new Dictionary<string, bool>();			//!Dictionaries that hold the ambience file names from json file
	Dictionary<string, bool> effectDict = new Dictionary<string, bool>();			//!Dictionaries that hold the effect file names from json file
	Dictionary<string, bool> musicDict = new Dictionary<string, bool>();			//!Dictionaries that hold the music file names from json file
	Dictionary<string, bool> voiceDict = new Dictionary<string, bool>();			//!Dictionaries that hold the voice file names from json file
	Dictionary<string, string> priorityDict = new Dictionary<string, string>();		//!Dictionaries that hold the priority file names from json file

	Dictionary<string, AudioSource> activeAudio = new Dictionary<string, AudioSource>();	//!holds songs that are actively playing
	Dictionary<string, AudioClip> priorityAudio = new Dictionary<string, AudioClip>();		//!holds songs that are priority to be loaded at start

	//Used for testing
	GameObject player;					//test player game object
	public bool testing = false;		//if we are running the testing
	//bool onePause = false;				//pause single active audio
	//bool groupPause = false;
	//bool allPause = false;
	//bool oneUnPause = false;
	//bool groupUnPause = false;
	//bool allUnPause = false;
	//bool stop = false;

	//Possible later use for custom gui buttons and visual output
	public int numberOfActiveAudio = 0;			//may need to be public for custom inspector window
	public int numberOfPriorityAudio = 0;		//may need to be public for custom inspector window

    void Awake()
    {
        _instance = null;
    }
    
	//! Unity Start function
    void Start()
    {
		Debug.Log("audio", "SoundManager has started");

		#region singletonCreation
		if(_instance == null) {
			//If I am the first instance, make me the Singleton
			Debug.Log("audio", "Audio Singleton did not exist, creating");
			_instance = this;
			DontDestroyOnLoad(this);
		} else {
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			Debug.Error("audio", "There is more than one Audio Singleton, Destroying");
			if(this != _instance)
				Destroy(this.gameObject);
		}
		#endregion

		if(testing) {
			player = new GameObject("PlayerTest");
			Debug.Warning("audio", "Audio testing is enabled");
		}

			
		//JSON file load check
		if(!loadListFromFile(soundListFilePath)) {
			Debug.Error("audio", "JSON file did not load");
		} else
			Debug.Log("audio", "JSON file loaded");

        changeVol("master", 40f);

		generatePriority();
	}

	//! Unity Update function
	void Update() {

		#region Testing features
		//For testing only
		if(testing) {

			editorVol = 60f;

			if(Input.GetButtonDown("Fire1")) {
				changeVol("master", editorVol);
				Debug.Log("audio", "Temp Volume level is now " + editorVol);
				//Debug.Log("audio", "tempClip's file");
			}
			if(Input.GetButtonDown("Fire2"))
				Debug.Log("audio", "Volume level is now " + seeVol("master"));		//slightly slower than change vol
			//changeVol("reset", tempvol);
			if(Input.GetButtonDown("Jump")) {
				//hard code song load
				//player = new GameObject("PlayerTest");
				Debug.Log("audio", "spacebar pressed playing music1");
				playMe(player, "music", "music1");
			}

			if(activeAudio.Count > 0) {
				//activeAudioEmpty = false;
				numberOfActiveAudio = activeAudio.Count;
				displayTime(player.GetComponent<AudioSource>());		//displays play time & total time in debug log
			} else {
				//activeAudioEmpty = true;
				numberOfActiveAudio = 0;
			}

			if(priorityAudio.Count > 0) {
				//priorityAudioEmpty = false;
				numberOfPriorityAudio = priorityAudio.Count;
				//displayTime(player.GetComponent<AudioSource>());		//displays play time & total time in debug log
			} else {
				//priorityAudioEmpty = true;
				numberOfPriorityAudio = 0;
			}

			//pause checking
			//if(onePause) {
			//	pauseMe(player);
			//} else if(groupPause) {
			//	pauseGroup("music");
			//} else if(allPause) {
			//	pauseAll();
			//} else if(oneUnPause) {
			//	oneUnPause = false;
			//	unpauseMe(player);
			//} else if(groupUnPause) {
			//	groupUnPause = false;
			//	unpauseGroup("music");
			//} else if(allUnPause) {
			//	allUnPause = false;
			//	unpauseAll();
			//} else if(stop) {
			//	stopMe(player);
			//	stop = false;
			//}
		}
		#endregion 

	}

	#region json file reading
	//!checks JSON file exists and loads it
	public bool loadListFromFile(string path) {		//checks to make sure json file is there
		
		if(!System.IO.File.Exists(Application.dataPath + path)) {
			Debug.Error("audio", "File does not exist: " + Application.dataPath + path);
			return false;
		}
		string json = System.IO.File.ReadAllText(Application.dataPath + path);
		return loadListFromJson(json);
	}

	//!checks JSON file isnt empty and loads data into audio dictionaries
	public bool loadListFromJson(string json) {		
		JSONNode soundData = JSON.Parse(json);			//the whole JSON file
		if(soundData == null) {
			Debug.Error("audio", "Json file is empty");
			return false;
		}
		
		JSONNode audioNode = soundData["audio"];		//Just the ones under audio
		string[] types = { "ambience", "effect", "music", "voice" };

		foreach(string type in types) {
			JSONArray tempArray = audioNode[type].AsArray;	//just the objects under the type
			foreach(JSONNode j in tempArray) {
				var name = j[0];				//j[0] is name of object
				bool priority = j[1].AsBool;	//j[1] priority of object
				addSound(type, name, priority);
				//if(priority)
				//	loadPriority(type, name);
			}
		}
		return true;
	}
	#endregion 
	
	#region General Sound Functions
	//!Adds sound info from JSON to Dictionaries
	public void addSound(string type, string name, bool priority){
		switch(type.ToLower()) {
			case "ambience":
				ambienceDict.Add(name, priority);
				break;
			case "effect":
				effectDict.Add(name, priority);
				break;
			case "music":
				musicDict.Add(name, priority);
				break;
			case "voice":
				voiceDict.Add(name, priority);
				break;
			default:
				Debug.Log("audio", type + "is not an option, or you spelled it wrong");
				break;
		}
		if(priority)
			priorityDict.Add(name, type);
	} 

	//!checks if sounds in dictionary
	public bool findSound(string type, string name) {
		switch(type.ToLower()) {
			case "ambience":
				if(ambienceDict.ContainsKey(name))
					return true;
				else
					return false;
			case "effect":
				if(effectDict.ContainsKey(name))
					return true;
				else
					return false;
			case "music":
				if(musicDict.ContainsKey(name))
					return true;
				else
					return false;
			case "voice":
				if(voiceDict.ContainsKey(name))
					return true;
				else
					return false;
			default:
				Debug.Log("audio", type + "is not an option, or you spelled it wrong");
				return false;
		}
	}


	//!checks if sounds in any dictionary, returns which dictionary.
	public string findSound(string name) {
		
		if(ambienceDict.ContainsKey(name))
			return "ambience";
		else if(effectDict.ContainsKey(name))
			return "effect";
		else if(musicDict.ContainsKey(name))
			return "music";
		else if(voiceDict.ContainsKey(name))
			return "voice";
		else{
			Debug.Log("audio", name + " is not found in any dictionary, how did you mess this up?");
				return "none";
		}
	}

	//!returns dictionary requested
    public Dictionary<string, bool> getDictionary(string type)
    {
        switch (type.ToLower())
        {
            case "ambience":
                return ambienceDict;
            case "effect":
                return effectDict;
            case "music":
                return musicDict;
            case "voice":
                return voiceDict;
            default:
                Debug.Log("audio", type + "is not an option, or you spelled it wrong");
                return null;
        }
    }

	//!Displays playhead location and total time to audio debug
	void displayTime(AudioSource source) {

		//total time
		float totalSeconds = source.clip.length;
		float totalSec = totalSeconds % 60;
		int totalMin = Mathf.FloorToInt(totalSeconds / 60);

		//play head
		float sec = source.time % 60;
		int min = Mathf.FloorToInt(source.time / 60);


		Debug.Log("audio", "Display: current play head location " + min + ":" + sec);
		Debug.Log("audio", "Display: Total time " + totalMin + ":" + totalSec);
	}

	//!Takes priority entries from JSON file and loads them into a dictionary of audio sources
	void generatePriority() {
		foreach(string name in priorityDict.Keys){
			string type = priorityDict[name];
			loadPriority(type, name);
		}
		
	}

	//!Loads priority audio files into the priority que
	void loadPriority(string type, string name) {
		Debug.Log("audio", "Load priority, name is " + name);
		StartCoroutine(loadAudio(null, type, name, "priority"));
	}
    #endregion

    #region Play Functions
    //!Checks to see if there was a target object given, if the target is already playing audio, if the requested audio is already preloaded, and calls loadAudio
	[EventVisible]
	public void playMe(GameObject target, string type, string name) {
		string loadType;

		if (!target){
            target = GameObject.Find("_Player");
            Debug.Log("audio", "PlayMe: target object was null, target is now the player");
        }
		if(playingCurrently(target)) {
			Debug.Warning("audio", "PlayMe: target object is already playing audio");
		} else {
			if(priorityAudio.ContainsKey(name))
				loadType = "priority";
			else
				loadType = "play";

			StartCoroutine(loadAudio(target, type, name, loadType));
		}
    }

	//!Coroutine that checks the play medium(editor/application), creates file path, loads sound into a clip, if its a priority it adds it to list else it calls the audio source builder function
    IEnumerator loadAudio(GameObject target, string type, string name, string loadType)
    {
        string typePath;
		string filename;
        AudioMixer mixerGroup;
		
		//checking if its a play or a priority load
		if(loadType == "play") {
			filename = name + ".ogg";
			Debug.Log("audio", "LoadAudio: load type is " + loadType + " filename is " + filename);
		} else if(loadType == "priority") {
			filename = name;
			Debug.Log("audio", "LoadAudio: load type is " + loadType + " filename is " + filename);
		} else {
			filename = "temp";
			Debug.Log("audio", "LoadAudio: load type is " + loadType + " filename is " + filename);
		}

        //files handled differently between editor and player
        //in editor: /Assets/StreamingAssets
        //in player: Application.streamingAssetsPath
        string dir;
        if (Application.isEditor){
            dir = Application.dataPath + "/Audio";

            switch (type.ToLower())
            {
                case "ambience":
                    typePath = "file:///" + dir + "/ambience/" + filename;
                    mixerGroup = ambienceMixer;
                    break;
                case "effect":
                    typePath = "file:///" + dir + "/Effects/" + filename;
                    mixerGroup = effectMixer;
                    break;
                case "music":
                    typePath = "file:///" + dir + "/Music/" + filename;
                    mixerGroup = musicMixer;
                    break;
                case "voice":
                    typePath = "file:///" + dir + "/Voice/" + filename;
                    mixerGroup = voiceMixer;
                    break;
                default:
                    Debug.Log("audio", type + " is not an option, or you spelled it wrong");
                    typePath = "path is invalid";
                    mixerGroup = null;
                    break;
            }
        }else{
            dir = Application.streamingAssetsPath + "/Audio";                   //Untested

            switch (type.ToLower())
            {
                case "ambience":
                    typePath = "file:///" + dir + "/ambience/" + filename;
                    mixerGroup = ambienceMixer;
                    break;
                case "effect":
                    typePath = "file:///" + dir + "/Effects/" + filename;
                    mixerGroup = effectMixer;
                    break;
                case "music":
                    typePath = "file:///" + dir + "/Music/" + filename;
                    mixerGroup = musicMixer;
                    break;
                case "voice":
                    typePath = "file:///" + dir + "/Voice/" + filename;
                    mixerGroup = voiceMixer;
                    break;
                default:
                    Debug.Log("audio", type + "is not an option, or you spelled it wrong");
                    typePath = "path is invalid";
                    mixerGroup = null;
                    break;
            }
        }
		//if its already loaded
		if(priorityAudio.ContainsKey(name)) {
			Debug.Log("audio", "LoadAudio: " + name + " is already on the priority list, finishing loading");
			finishedLoading(priorityAudio[name], mixerGroup, target, name, loadType);
		} else {

			//build and log path based on filename
			Debug.Log("audio", "LoadAudio: file path created: " + typePath);

			var www = new WWW(typePath);
			yield return www;               //now we wait

			if(string.IsNullOrEmpty(www.error)) {
				//we have now finished loading the file
				//if you do use an asset bundle, you just handle it a bit differently here
				AudioClip clip = www.audioClip;
				Debug.Log("audio", "LoadAudio: " + name + " Audio clip loaded from file, " + mixerGroup + " is the mixer group");
				
				//finished loading -- fire callback
				name = Path.GetFileNameWithoutExtension(typePath);
				if(loadType == "priority")
					priorityAudio.Add(name, clip);
				else
					finishedLoading(clip, mixerGroup, target, name, loadType);
			} else {
				//bail out!
				Debug.Error("audio", "LoadAudio: WWW Error: " + www.error);
			}
		}
    }

    //!callback function, builds audio source once a clip is loaded
    void finishedLoading(AudioClip clip, AudioMixer mixerGroup, GameObject go, string name, string loadType){

		AudioSource source = go.AddComponent<AudioSource>();

        AudioMixerGroup[] groupArray = mixerGroup.FindMatchingGroups(name);
        Debug.Log("audio", "FinishedLoading: group array length is " + groupArray.Length);

        if (groupArray.Length == 1){
            source.outputAudioMixerGroup = groupArray[0];           //how i set the output for the source's mixergroup
            source.clip = clip;
			
			source.Play();
			//Add to active audio dictionary
			activeAudio.Add(name, source);

			Debug.Log("audio", "FinishedLoading: source name is " + name + " and has been added to active audio");
			Debug.Log("audio", "FinishedLoading: clip length is " + source.clip.length);
            
			//start a new coroutine to wait for DESTRUCTION TIME
			StartCoroutine(reclaimSource(source, name, loadType));
		}else if (groupArray.Length > 1){
			Debug.Warning("audio", "FinishedLoading: more than 1 mixer group exists for the sound " + name);
        }else{
			Debug.Warning("audio", "FinishedLoading: No mixer group exists for sound " + name);
        }
    }

	//!deletes the audio source when its no longer needed
    IEnumerator reclaimSource(AudioSource source, string name, string loadType)
    {
		yield return new WaitForSeconds(0.9f);
		//wait until finished playing
		while((source.time > 0.0f)) {
			if(source.time < 0.1f) {
				Debug.Log("audio", name + "ReclaimSource:  has finished playing");
				break;
			}
			yield return null;
		}

        //stop tracking the source
        activeAudio.Remove(name);

        //destroy the source
        if(loadType != "priority")
			Destroy(source);
    }
	
	//!checks to see if the target's audio is currently playing
	bool playingCurrently(GameObject target) {
		if(!target.GetComponent<AudioSource>())
			return false;
		else
			return true;
	}

    /* THIS IS A NEW TEST */
    [EventVisible]
    public void playAudio(GameObject gO)
    {
        float length, timeLeft;
        AudioSource aSource;
        aSource = gO.GetComponent<AudioSource>();
        length = aSource.clip.length;
        //print(length);
		length -= aSource.time;
		if (length == 0 || length == aSource.clip.length)
			aSource.Play ();
		else
			aSource.mute = false;
    }
    [EventVisible]
    public void muteAudio(GameObject gO)
    {
        AudioSource aSource;
        aSource = gO.GetComponent<AudioSource>();
        // Plays when the player enters the pop event
        aSource.mute = true;
    }
    #endregion	//play functions

	#region stop functions
	//!Stops the target object's audio
    void stopMe(GameObject target){
        target.GetComponent<AudioSource>().Stop();
		Debug.Log("audio", target.name + " has stopped");
    }

	//!stops all active playing sound
	void stopAll() {
		foreach(string entry in activeAudio.Keys)
			activeAudio[entry].Stop();
		Debug.Log("audio", "All active audio stopped");
	}
	#endregion	//stop functions

	#region Pause/unpause files Function
	//!Pause single target's audio
	void pauseMe(GameObject target) {
		target.GetComponent<AudioSource>().Pause();
		Debug.Log("audio", target.name + "'s audio is paused");
	}

	//!Pause all audio of a type
	void pauseGroup(string type){
		Dictionary<string, bool> tempDict = getDictionary(type);

		foreach(string name in activeAudio.Keys) {
			if(tempDict.ContainsKey(name + ".ogg")) {
				activeAudio[name].Pause();
				Debug.Log("audio", name + " from group " + type +" is paused");
			} else
				Debug.Log("audio", name + " from group " + type + " is not paused");
		}
		Debug.Log("audio","all of " + type + " group is paused");
    }

	//!Pause all active audio
	void pauseAll() {
		foreach(string entry in activeAudio.Keys)
			activeAudio[entry].Pause();
		Debug.Log("audio", "All active audio paused");
	}


	//!Pause single target's audio
	void unpauseMe(GameObject target) {
		target.GetComponent<AudioSource>().UnPause();
		Debug.Log("audio", target.name + "'s audio has been unpaused");
	}

	//!Pause all audio of a type
	void unpauseGroup(string type) {
		Dictionary<string, bool> tempDict = getDictionary(type);

		foreach(string name in activeAudio.Keys) {
			if(tempDict.ContainsKey(name + ".ogg")) {
				activeAudio[name].UnPause();
				Debug.Log("audio", name + " from group " + type + " has been unpaused");
			}
		}
		Debug.Log("audio", "all of " + type + " group is paused");
	}

	//!Pause all active audio
	void unpauseAll() {
		foreach(string entry in activeAudio.Keys)
			activeAudio[entry].UnPause();
		Debug.Log("audio", "All active audio has been unpaused");
	}
	#endregion	//pause/unpause

	#region Sound volume
	[EventVisible]
	//!Change volume for groups in MasterMixer, handles volume levels of 1-100 converts to dB level for mixer
	public void changeVol(string name, float level){
        if (masterMixer == null)
        {
            return;
        }
		level = level - 80f;	//done so that vol at 50% will actually be 50% of total volume
		switch(name.ToLower()){
			case "master":
				masterMixer.SetFloat ("MasterVol", level);
				break;
			case "ambience":
				masterMixer.SetFloat ("MaxambienceVol", level);
				break;
			case "effect":
				masterMixer.SetFloat ("MaxEffectVol", level);
				break;
			case "music":
				masterMixer.SetFloat ("MaxMusicVol", level);
				break;
			case "voice":
				masterMixer.SetFloat ("MaxVoiceVol", level);
				break;
			case "reset":
				masterMixer.ClearFloat ("MasterVol");
				masterMixer.ClearFloat ("MaxambienceVol");
				masterMixer.ClearFloat ("MaxEffectVol");
				masterMixer.ClearFloat ("MaxMusicVol");
				masterMixer.ClearFloat ("MaxVoiceVol");
				editorVol = 60f;
				break;
			default:
				Debug.Log("audio", name + "is not an option, or you spelled it wrong");
				break;
		}
	}

	[EventVisible]
	//!Returns the current volume levels in MasterMixer, returns 300f if requested volume is wrong
	public float seeVol(string name){
		float level = 100f;
		switch(name.ToLower()) {
			case "master":
				masterMixer.GetFloat ("MasterVol", out level);
				level = level + 80f;
				return level;
			case "ambience":
				masterMixer.GetFloat ("MaxambienceVol", out level);
				level = level + 80f;
				return level;
			case "effect":
				masterMixer.GetFloat ("MaxEffectVol", out level);
				level = level + 80f;
				return level;
			case "music":
				masterMixer.GetFloat ("MaxMusicVol", out level);
				level = level + 80f;
				return level;
			case "voice":
				masterMixer.GetFloat ("MaxVoiceVol", out level);
				level = level + 80f;
				return level;
			default:
				Debug.Log("audio", name + "is not an option, or you spelled it wrong");
				return 300f;
		}
	}
	#endregion	//sound volume
}	//end of audio manager



/*Ideas/todo
 * DONE:
 *	Master Volumes
 *		public functions: changeVol, seeVol
 *		handles volume level 1-100, converts to dB level of mixer 
 *	Loading files, All play functions, All pause/unpause functions
 * 
 * json sound list - DONE
 *	json file: filename, type of sound, priority always load or on use load
 *	http://jsonformatter.curiousconcept.com/ - format json, may not be simplejson
 *	http://json.org/example
 *	look at checkpointManager.cs & AchievementManager.cs
 *	http://wiki.unity3d.com/index.php/SimpleJSON
 *	ALL TYPE HAS BEEN SET TO LOWERCASE AND INPUT HAS BEEN CONVERTED VIA ToLower()
 * 
 * 
 * 
 * Audio should come from streaming Assets
 *   http://docs.unity3d.com/Manual/StreamingAssets.html
 *   http://answers.unity3d.com/questions/11021/how-can-i-send-and-receive-data-to-and-from-a-url.html
 *   
 *		http://answers.unity3d.com/questions/787173/load-ogg-file-into-audioclip.html
 *		http://answers.unity3d.com/questions/332450/how-do-i-load-an-audioclip-from-code.html
 *  
 *	Asset bundles
 *		http://docs.unity3d.com/Manual/BuildingAssetBundles5x.html
 *		http://docs.unity3d.com/ScriptReference/AssetBundle.html
 *		http://answers.unity3d.com/questions/416895/wwwloadfromcacheordownload-and-audioclip-with-thre.html
 *		http://answers.unity3d.com/questions/7653/dynamic-asset-loading.html
 *		https://unity3d.com/learn/tutorials/modules/intermediate/live-training-archive/unity5-asset-bundles *******
 * 
 * check sounds are on list and ready to load
 *   make sure file paths are correct either in JSON or hardcoded
 * Add priority sounds into game at game start
 * 
 * 
 * when called play sound or throw error
 * gui should call sound manager for volume changes
 * 
 * 
 * play functions
 *	void play sound - done
 *	void pause sound
 *	bool is playing - done
 *	void stop playing - done
 *	
 *	
 *   pause all, group sounds
 * 
 *  play on player, if given no object play on player - done
 *  play on object, if given object play on object - done
 *  
 * local object sound script should call sound manager
 * sound manager should create audio source & delete when done playing - done
 * 
 * After clip load & play, attach to dictionary for visual output of sounds playing with time left in clip.
 *	coroutein - keep checking to update time, if finished send call to delete audio source
 *	http://docs.unity3d.com/ScriptReference/AudioSource-time.html
 * 
 * Not by tuesday
 *  Custom sound inspector window
 *   http://unity3d.com/learn/tutorials/modules/intermediate/editor/building-custom-inspector
 *   on inspector gui - only in manager
 *     display list of active sounds
 *     print as text/cant interract
 *     total time & time left of each
 * 
 *	when object is loaded prep its sound for creation
 *		possible use sound effects
 * 
*/