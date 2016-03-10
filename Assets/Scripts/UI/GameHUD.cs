using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Debug = FFP.Debug;

/*! ----------------------------------------------------------------------------
 * Main hud control, contains functions for updating HUD information on the player's screen
 * these functions are designed to be called from whatever script needs to update them.
 * ----------------------------------------------------------------------------
 */
[EventVisible("UI")]
public class GameHUD : MonoBehaviour {
	#region singletonEnforcement
	private static GameHUD instance;
	public static GameHUD Instance {
		get {
			return instance;
		}
		private set { }
	}
	#endregion

#pragma warning disable 0219
#pragma warning disable 0414
    GameObject UIhud;
	GameObject mainHUDCanvas;				//!<The canvas HUD is rendered on
	GameObject worldMapCanvas;				//!<All the game map elements
	GameObject gameMap;						//!<The map iamge on a plane
	GameObject player;						//!<reference to player
	public GameObject pauseMenu;

	public PauseMenu accessManager;
    public MainMenuManager menuManager;
	
	GameObject mapCam;								//!<Camera used for minimap
	static GameObject objectiveText;						//!<Objective Text UI element

	GameObject[] mapLabels;							//!<Array of text taht appears on minimap

	public bool skillsOpen = false;
	bool canSpin = false;
	GameObject closeMapButton;
	GameObject phoneButtons;
	GameObject mapElements;
	GameObject compassCameraPoint;					//!<Point at camera location used to calculate objective positions
	GameObject compass;
	GameObject slider;
	GameObject leftArrow;
	GameObject rightArrow;

	GameObject testObjective;

	void Awake() {
		#region singletonEnforcement
		if(instance == null) {
			instance = this;
		} else {
			Destroy(this.gameObject);
			Debug.Error("core", "Second GameHUD detected. Deleting gameOject.");
			return;
		}
		#endregion

        UIhud = GameObject.Find("_UI");
		mainHUDCanvas = GameObject.Find("mainHUD");
		worldMapCanvas = GameObject.Find("worldMapCanvas");
		gameMap = GameObject.Find("mapBG");
		player = GameObject.Find("_Player");
		testObjective = GameObject.Find("TestObjective");
        if (!pauseMenu)
        {
            pauseMenu = GameObject.Find("pauseMenu");
        }
		pauseMenu.SetActive (false);
		
		//!Turn on UI stuff
		worldMapCanvas.SetActive(true);

		//!Fill mapLabels array
		mapLabels = GameObject.FindGameObjectsWithTag("worldMapLabel");
		closeMapButton = GameObject.Find("CloseMapButton");
		closeMapButton.SetActive(false);

		//!Set mapcam reference
		mapCam = GameObject.Find("mapCam");
		//!Set compassCameraPoint reference
		compassCameraPoint = GameObject.Find("compassCameraPoint");
		compass = GameObject.Find("compassSlider");
		slider = compass.transform.FindChild ("Handle Slide Area").gameObject;
		slider.SetActive (false);
		leftArrow = compass.transform.FindChild ("leftArrow").gameObject;
		leftArrow.SetActive (false);
		rightArrow = compass.transform.FindChild ("rightArrow").gameObject;
		rightArrow.SetActive (false);

		//!Set objective text reference
		objectiveText = GameObject.Find("objectiveText");

		phoneButtons = GameObject.Find("PhoneButtons");
		mapElements = GameObject.Find("MapElements");
		mapElements.SetActive(false);

	}

	void Start() {
		//Place the ability buttons in the Phone Menu
		//SpawnHudAbilityIcons ();
	}

	void FixedUpdate() {
		//!This is for testing, call update map from player movements
		rotateMapObjects();

		//!Set the compass indicator
		setCompassValue(calculateObjectiveAngle(testObjective));
	}

	//!Call this to update objective tet at top of the screen
	[EventVisible]
	public void UpdateObjectiveText(string newObjective) {
		objectiveText.GetComponent<Text>().text = newObjective;
	}

	//!Rotates map labels so that the text is always right side up, call this from anything that rotates the camera
	//!Right now its based on Player rotation, needs to be based on camera
	public void rotateMapObjects() {
		Quaternion newRotation;
		foreach(GameObject curLabel in mapLabels) {
			newRotation = Quaternion.Euler(new Vector3(90, 0, -player.transform.rotation.eulerAngles.y));
			curLabel.GetComponent<RectTransform>().rotation = newRotation;
		}
	}

	public void setCompassValue(float newValue) {
        if (testObjective == null)
        {
            return;
        }

		//!Calculates distances between "the player and the objective" and "the camera and the objective"
		float distanceBetweenCamAndObj = Vector3.Distance (compassCameraPoint.transform.position, testObjective.transform.position);
		float distanceBetweenPlayerAndObj = Vector3.Distance (player.transform.position, testObjective.transform.position);



		//!If the camera is closer to the objective, this means the objective is behind the player.
		//!In these first two cases, the compass will be forced to one side or the other as to not confuse the player
		if (distanceBetweenCamAndObj < distanceBetweenPlayerAndObj && newValue >= 90) {
			newValue = 105;
		} 
		else if (distanceBetweenCamAndObj < distanceBetweenPlayerAndObj && newValue < 90) {
			newValue = 75;
		}

		else if(newValue > 105) {
			newValue = 105;
		}
		else if(newValue < 75) {
			newValue = 75;
		}

		if (newValue == 105) {
			slider.SetActive (false);
			rightArrow.SetActive (true);
			leftArrow.SetActive(false);
		} 
		else if (newValue == 75) {
			slider.SetActive (false);
			leftArrow.SetActive (true);
			rightArrow.SetActive(false);
		} 
		else {
			leftArrow.SetActive(false);
			rightArrow.SetActive(false);
			slider.SetActive(true);
		}
		compass.GetComponent<Slider>().value = newValue;
	}

	public float calculateObjectiveAngle(GameObject objective) {
        if (objective == null)
        {
            return 0;
        }
		Vector3 pointToObjective;
		Vector3 pointStraightFromCam;

		//!Set points to determine which side of the player the vector towards the objective is going
		GameObject camPointRight = GameObject.Find("camPointRight");
		GameObject camPointleft = GameObject.Find("camPointLeft");


		//!create vector3 from player to objective and normalize it
		pointToObjective = objective.gameObject.transform.position - compassCameraPoint.transform.position;
		pointToObjective.Normalize();

		//!Set this vector to point right away from camera
		pointStraightFromCam = -compassCameraPoint.transform.right;
		pointStraightFromCam.Normalize();

		//!return angle from right facing vector

		return Vector3.Angle(pointStraightFromCam, pointToObjective);


	}

	//This is for testing
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Finish") {
			UpdateObjectiveText("Objective Complete!");
		}
	}


	//Shows map on phone and roates and resizes phone to screen
	public void showMap() {
		phoneButtons.SetActive(false);
		mapElements.SetActive(true);
		closeMapButton.SetActive(true);
		GameObject.Find("PhoneMenu").GetComponent<Animator>().SetBool("mapActive", true);
	}

	//hides map and plays close phone animation
	public void hideMap() {
		mapElements.SetActive(false);
		phoneButtons.SetActive(true);
		closeMapButton.SetActive(false);
		GameObject.Find("PhoneMenu").GetComponent<Animator>().SetBool("mapActive", false);

	}

	public void ChangeInputToUI(bool change = true) {/*
		if(change)
		InputManager.instance.ChangeInputType("UIInputType");
		else
			InputManager.instance.ChangeInputType("GameInputType");*/
	}

    public void PauseNoMenu() {
        accessManager.setPause();
        
    }

    public void timeNormal() {
        accessManager.setTimeNormal();
    }

    public void timeManipulate(float speed) {
        //speed = 2.0f;
        if(speed > 0 && speed < 1.75) {
            accessManager.manipulateTime(speed);
        } 
        else {
            System.Console.WriteLine("Error value of Speed GameHUD :: timeManipulate(float speed)");
        }
        
     }

	public void showPauseMenu () {
		pauseMenu.SetActive (true);
		
	}

	public void hidePauseMenu () {
		pauseMenu.SetActive (false);
		accessManager.unPauseGameBtt();
	}

	public void loadScene(string s) {
		Application.LoadLevel(s);
	}

	public void quitGame () {
		Application.Quit ();
	}

    public void openOptions() 
    {
        menuManager.GoToOptions();
    }
}
