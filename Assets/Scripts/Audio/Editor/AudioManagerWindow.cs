using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using System.Collections;


public class AudioManagerWindow : EditorWindow
{

    private GameObject selectedGO;
    private AudioSource aSource;
    private AudioClip aClip;
    private AudioMixerGroup aMixer;
    private float maxDistance;

    // Creats a menu tab in the cusutom tab
    [MenuItem("Custom Tools/Audio Manager")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AudioManagerWindow));
    }

    // Sets the gameobject to the selected gameobject
    private void OnSelectionChange()
    {
        selectedGO = Selection.activeGameObject;
        aClip = null;
        aMixer = null;
        Repaint(); // Refreshes when the new gameobject is selected
    }


    private void OnGUI()
	{
        /*
        float audioListenerCounter = 0;
        
        foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.GetComponent<AudioListener>())
            {
                audioListenerCounter++;
            }
        }
        
		if(audioListenerCounter <= 0)
        {
            Debug.Log("There is no audio listener");
        }
		*/

        #region Adds Audio Source and Clip to GameObject
        // If nothing is selected, it loops out of the GUI
        if (selectedGO)
        {
            // Checks to see if the selected gameobject has an audio source
            // if it does, then it sets that game object to aSource to use in the following functions
            if (selectedGO.GetComponent<AudioSource>())
            {
                aSource = selectedGO.GetComponent<AudioSource>();
                //aSource.playOnAwake = false;
                if (selectedGO.GetComponent<AudioSource>().clip)
                {
                    /*
                    Everytime a new gameobject is selected, 
                    if the audio source already has a clip
                    it will be set to the audio source's clip
                    so the last one won't be transfered over
                    */
                    aClip = selectedGO.GetComponent<AudioSource>().clip;
                }
                if(selectedGO.GetComponent<AudioSource>().outputAudioMixerGroup)
                {
                    /*
                    If the source already has a mixer attached to it,
                    The audio window manager will set theirs as the 
                    audio source's output audio mixer
                    */
                    aMixer = selectedGO.GetComponent<AudioSource>().outputAudioMixerGroup;
                }
            }


            GUILayout.Label("Object selected: " + selectedGO.name); // Printing the gameobject name 
                                                                    // If the selected gameobject has no audio source,
                                                                    // There is an option to add an audio source component
            if (!selectedGO.GetComponent<AudioSource>())
            {
                if (GUILayout.Button("Add Audio Source"))
                {
                    selectedGO.AddComponent<AudioSource>();
                    // Sets the gameobject's audio source to the global audio source aSource
                    aSource = selectedGO.GetComponent<AudioSource>();
                }
            }
            /*
            If the selected gameobject has an audio source,
            It first gives gives the option select a clip.
            After a clip is selected, it is then set to gameobject's audiosource clip

            There is also an option to mute and unmute the audio source
            If the audio source is not muted, then you can mute.
            If it is muted, you can unmute it.

            You can also delete the audio source from the gameobject
            */
            if (selectedGO.GetComponent<AudioSource>())
            {
                // Gives the option to pick an audio clip from the asset folder
                EditorGUILayout.BeginHorizontal();
                aClip = EditorGUILayout.ObjectField(aClip, typeof(AudioClip), true) as AudioClip;
                aSource.clip = aClip;
                EditorGUILayout.EndHorizontal();

                // Sets the output of the Audio Source to a mixer
                EditorGUILayout.BeginHorizontal();
                aMixer = EditorGUILayout.ObjectField(aMixer, typeof(AudioMixerGroup), true) as AudioMixerGroup;
                aSource.outputAudioMixerGroup = aMixer;
                EditorGUILayout.EndHorizontal();

                

                // If the audio source is not muted, you can mute it here
                if (!aSource.mute)
                {
                    if (GUILayout.Button("Mute This Audio Source"))
                    {
                        aSource.mute = true;
                    }
                }

                // If the audio source is muted, you can unmute it here
                else if (aSource.mute)
                {
                    if (GUILayout.Button("Unmute This Audio Source"))
                    {
                        aSource.mute = false;
                    }
                }

                // Deletes the audio source component from the game object
                // Also sets Audio Source's clip and mixer to null
                if (GUILayout.Button("Delete Audio Source"))
                {
                    DestroyImmediate(selectedGO.GetComponent<AudioSource>());
                    aClip = null;
                    aMixer = null;
                }

                EditorGUILayout.Space();
				/*
                if (!selectedGO.GetComponent<PlayAudio>())
                {
                    if (GUILayout.Button("Add Play Audio Script"))
                    {
                        selectedGO.AddComponent<PlayAudio>();
                    }
                }
                
                if (selectedGO.GetComponent<PlayAudio>())
                {
                    selectedGO.GetComponent<PlayAudio>().minDistance = EditorGUILayout.FloatField("Min Distance: ", selectedGO.GetComponent<PlayAudio>().minDistance);
                    selectedGO.GetComponent<PlayAudio>().maxDistance = EditorGUILayout.FloatField("Max Distance: ", selectedGO.GetComponent<PlayAudio>().maxDistance);
                }
				*/
				
            }
        }
        #endregion
        EditorGUILayout.Space();
        #region Mute Audio Source
        // If this button is pressed, it searches through 
        // all the gameobject and it picks those that have an audio source
        // It then mutes all of them
        if (GUILayout.Button("Mute All Audio Source"))
        {
            foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.GetComponent<AudioSource>())
                    obj.GetComponent<AudioSource>().mute = true;
            }
        }
        // If this button is pressed, it searches through 
        // all the gameobject and it picks those that have an audio source
        // It then unmute all of them
        if (GUILayout.Button("Unmute All Audio Component"))
        {
            foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.GetComponent<AudioSource>())
                    obj.GetComponent<AudioSource>().mute = false;
            }
        }
        #endregion
        /* 
        Shows all the current playing audio source during game time
        Shows the time left on the clip
        When a clip is done playing, it will give a notification that the clip is done playing
		*/
        if (Application.isPlaying)
        {
            GUILayout.Label("Current Playing List");
            foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.GetComponent<AudioSource>())
                {
                    if (obj.GetComponent<AudioSource>().isPlaying)
                    {
                        GUILayout.Label(obj.name + " - Time Left: " + (obj.GetComponent<AudioSource>().clip.length - obj.GetComponent<AudioSource>().time));
                    }
                    else if(!obj.GetComponent<AudioSource>().isPlaying)
                    {
                        GUILayout.Label(obj.name + " is done");
                    }
                    #region Paused/UnPaused
                    /*
                    This is to pause the audio source.
                    May or may not be needed
                    if (obj.GetComponent<AudioSource>().isPlaying)
                    {
                        if (GUILayout.Button("Pause " + obj.name))
                        {
                            obj.GetComponent<AudioSource>().Pause();
                        }
                    }
                    else if (!obj.GetComponent<AudioSource>().isPlaying)
                    {
                        if (GUILayout.Button("unPause " + obj.name))
                        {
                            obj.GetComponent<AudioSource>().UnPause();
                        }
                    }
                }
                */
                    #endregion
                }
            }

        }
        // Repaint();
    }

    void OnInspectorUpdate()
    {
        this.Repaint();
    }
}