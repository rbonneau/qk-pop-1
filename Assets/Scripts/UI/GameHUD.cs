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
    #region Singleton Enforcement
    private static GameHUD instance;
    public static GameHUD Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameHUD>();
            }
            return instance;
        }
    }
    #endregion

#pragma warning disable 0219
#pragma warning disable 0414
    GameObject UIhud;
    GameObject mainHUDCanvas;               //!<The canvas HUD is rendered on
    GameObject worldMapCanvas;              //!<All the game map elements
    GameObject player;                      //!<reference to player
    public GameObject pauseMenu;
    public PauseMenu accessManager;
    public MainMenuManager menuManager;
	public bool showMinimap = true;
	public RenderTexture MiniMapRenderTexture;
	public Material MiniMapMaterial;
	public float minimapXOffset;
	public float minimapYOffset;
	public Sprite[] targetableIcons;
	public Sprite enemyIcon;
	public bool calcCompass = false;
	public List<GameObject> mapLabels;
	public GameObject testObjective;

	//public GameObject closestTargetIconPrefab;

	GameObject mapCam;								//!<Camera used for minimap

	static GameObject objectiveText;						//!<Objective Text UI element
    static Text QuestNotText;

	

	public bool skillsOpen = false;
	bool canSpin = false;
	GameObject closeMapButton;
	GameObject phoneButtons;
	//GameObject mapElements;
	GameObject compassCameraPoint;					//!<Point at camera location used to calculate objective positions
	public GameObject compass;
	GameObject slider;
	GameObject leftArrow;
	GameObject rightArrow;
	GameObject controls;
	GameObject pcControls;
	GameObject xboxControls;
	GameObject psControls;
	GameObject questManagerUI;
	GameObject minimapRedArrow; 					//!<Arrow for the player representation on the minimap
	GameObject mainCamera;
	GameObject minimapCamera;
	GameObject minimapCompass;
	List<GameObject> targetsInRange;
	GameObject closestTargetIcon;
	Button pauseMenuResumeButton;
	float offset = 10f;

    void Awake()
    {
        UIhud = GameObject.Find("UI");
        mainHUDCanvas = GameObject.Find("mainHUD");
        worldMapCanvas = GameObject.Find("worldMapCanvas");
        //gameMap = GameObject.Find("mapBG");
        player = GameObject.Find("_Player");

        if (!pauseMenu) {
            pauseMenu = GameObject.Find("pauseMenu");
        }
        pauseMenu.SetActive(false);
        //!Turn on UI stuff
        worldMapCanvas.SetActive(true);

        //!Fill mapLabels array
        GameObject[] tempMapLables = GameObject.FindGameObjectsWithTag("worldMapLabel");
        foreach (GameObject label in tempMapLables){
        	mapLabels.Add(label);
        }
        closeMapButton = GameObject.Find("CloseMapButton");

        if (closeMapButton) {
            closeMapButton.SetActive(false);
        }
		
        //!Set compassCameraPoint reference
        compassCameraPoint = GameObject.Find("compassCameraPoint");
        compass = GameObject.Find("compassSlider");
        slider = compass.transform.FindChild("Handle Slide Area").gameObject;
        leftArrow = compass.transform.FindChild("leftArrow").gameObject;
        rightArrow = compass.transform.FindChild("rightArrow").gameObject;

        //!Set objective text reference
        objectiveText = GameObject.Find("ObjectiveNotice");
        QuestNotText = GameObject.Find("ObjectiveText").GetComponent<Text>();
        Debug.Log("ui", QuestNotText.text);
        objectiveText.SetActive(false);

        phoneButtons = GameObject.Find("PhoneButtons");

		controls = GameObject.Find ("Controls");
		if(!controls){
			Debug.Log ("ui", "Could not find the 'Controls' GameObject in the current Scene: " + Application.loadedLevelName);
		}
		else{
			controls.SetActive (false);
			pcControls = controls.transform.FindChild ("MoreJournalInfo").FindChild("ScrollView").FindChild("KeyboardControls").gameObject;
			xboxControls = controls.transform.FindChild ("MoreJournalInfo").FindChild("ScrollView").FindChild("XBOXControls").gameObject;
			psControls = controls.transform.FindChild ("MoreJournalInfo").FindChild("ScrollView").FindChild("PlayStationControls").gameObject;
			xboxControls.SetActive (false);
			psControls.SetActive(false);
		}
		questManagerUI = GameObject.Find ("QuestManagerUI");
		if (!questManagerUI) {
			print ("Could not find the 'QuestManagerUI' GameObject in the current Scene: " + Application.loadedLevelName);
		}
		minimapRedArrow = GameObject.Find ("MinimapRedArrow");
		if(!minimapRedArrow){
			print ("Could not find the 'MinimapRedArrow' GameObject in the current Scene: " + Application.loadedLevelName);
		}
		mainCamera = GameObject.Find ("_Main Camera");
		if(!mainCamera){
			print("Could not find the '_Main Camera' GameObject in the current Scene: " + Application.loadedLevelName);
		}
		minimapCamera = GameObject.Find ("MinimapCamera");
		if(!minimapCamera){
			print("Could not find the 'MinimapCamera' GameObject in the current Scene: " + Application.loadedLevelName);
		}
		minimapCompass = GameObject.Find ("MinimapCompass");
		if (!minimapCompass) {
			print("Could not find the 'MinimapCompass' GameObject in the current Scene: " + Application.loadedLevelName);
		}
		closestTargetIcon = GameObject.Find ("ClosestTargetIcon");
		if (!closestTargetIcon) {
			Debug.Error ("HUD", "Could not find the 'ClosestTargetIcon' GameObject in the current Scene: " + Application.loadedLevelName);
		}
		pauseMenuResumeButton = pauseMenu.transform.FindChild ("resumeButton").gameObject.GetComponent<Button> ();
	}

	void Start() {
		/*
		Dictionary<string, string> keyboardButtons = InputManager.input.keyButtons;
		Dictionary<string, string> controllerButtons = InputManager.input.controllerButtons;

		controlsText.text = "";
		
		foreach(KeyValuePair<string, string> button in keyboardButtons){
			controlsText.text += button.Key + " - " + button.Value + "\n"; 
		}
		controlsText.text += "\n";
		foreach(KeyValuePair<string, string> button in controllerButtons){
			controlsText.text += button.Key + " - " + button.Value + "\n";
		}
		print (controlsText.text);
		controlsText.gameObject.SetActive (false);
		*/
	}

	/*!Update function that is called once every frame
	 * Handles the opening and closing of the journal and when to display an icon above the selected or closest target
	 */
	void Update(){
		
		if (Input.GetKeyDown (KeyCode.Escape) && controls.activeSelf) {
			HideControls();
		}
		
		if (Input.GetKeyDown (KeyCode.Tab) && (!controls.activeSelf && !questManagerUI.activeSelf && !PauseMenu.Instance.isOnPauseMenu)){
			PauseMenu.Instance.OpenOrClosePauseMenu ();
			ShowQMUI();
			}
		else if(Input.GetKeyDown (KeyCode.Tab) && questManagerUI.activeSelf){
			HideQMUI();
			PauseMenu.Instance.OpenOrClosePauseMenu ();
		}
		
		
		/* Function calls for displaying icon above a target object.
		   This is no longer used.
		   targetsInRange = PoPCamera.AcquireTarget ();
		if (PoPCamera.instance.CurrentTarget ()) {
			DisplayIconAboveTarget (PoPCamera.instance.CurrentTarget ());
		}
		else if (targetsInRange.Count != 0) {
			DisplayIconAboveTarget (targetsInRange[0]);
		}
		else {
			closestTargetIcon.transform.position = new Vector3(1000,1000,1000);
		}
		*/
	}

	void FixedUpdate() {
		UpdateMapObjects();

		//!Set the compass indicator
		if(!testObjective || !calcCompass){
			leftArrow.SetActive (false);
			slider.SetActive (false);
			rightArrow.SetActive (false);
		}
		else{
			setCompassValue(calculateObjectiveAngle(testObjective));
		}
	}

	void OnGUI(){
		//!Calls the DrawTexture function to display the minimap on the screen
		if (showMinimap) {
			Graphics.DrawTexture (new Rect (minimapXOffset, Screen.height - 256 - minimapYOffset, 256, 256), MiniMapRenderTexture, MiniMapMaterial);
		}
	}

    IEnumerator DisplayObjectiveNotification(string message, bool isGold)
    {
        objectiveText.SetActive(true);
		if (isGold) {
			objectiveText.transform.FindChild ("BlueBackground").gameObject.SetActive (false);
			objectiveText.transform.FindChild ("ExclamationMark").gameObject.SetActive (false);
		}
		else {
			objectiveText.transform.FindChild ("GoldBackground").gameObject.SetActive (false);
		}

        QuestNotText.text = message;
		while (true) {
			float delayTime = Time.realtimeSinceStartup + 3f;
			while(Time.realtimeSinceStartup < delayTime){
				yield return null;
			}
			break;
		}
        //yield return new WaitForSeconds(3);
        CanvasGroup canvas = objectiveText.GetComponent<CanvasGroup>();
        while (canvas.alpha > 0)
        {
            canvas.alpha -= 0.05f;
            yield return new WaitForEndOfFrame();
        }
		objectiveText.transform.FindChild ("BlueBackground").gameObject.SetActive (true);
		objectiveText.transform.FindChild ("ExclamationMark").gameObject.SetActive (true);
		objectiveText.transform.FindChild ("GoldBackground").gameObject.SetActive (true);
        canvas.alpha = 1f;
        objectiveText.SetActive(false);
    }

	//!Call this to update objective tet at top of the screen
	[EventVisible]
	public void UpdateObjectiveText(string newObjective, bool isGold)
    {
        StartCoroutine(DisplayObjectiveNotification(newObjective, isGold));
	}

	//!Rotates and moves all of the relevant objects on the minimap
	public void UpdateMapObjects() {
		Quaternion newRotation;
		foreach(GameObject curLabel in mapLabels) {
			RotateMapObjectsAboutGameObject(curLabel, mainCamera);
		}
		RotateMapObjectsAboutGameObject(minimapRedArrow, player);
		moveMapObjectsFromOtherObject(minimapRedArrow, player, new Vector3(0, -15, 0));
		RotateMapObjectsAboutGameObject (minimapCamera, mainCamera);
		moveMapObjectsFromOtherObject(minimapCamera, player, new Vector3(0, -10, 0));
		moveMapObjectsFromOtherObject (minimapCompass, player, new Vector3(0, -16, 0));
	}

	/*!Rotates items on the minimap always face in the correct direction
	 * The mainObject is the object that is to be rotated
	 * The secondaryObject is the object that the mainObject is to be rotated in reference to
	 * For example if you wanted to rotate a text object on the screen as the main camera turns,
	 * the text object would be the mainObject and the main camera would be the secondaryObject
	 */
	void RotateMapObjectsAboutGameObject(GameObject mainObject, GameObject secondaryObject){
		Quaternion newRotation;
		newRotation = Quaternion.Euler(new Vector3(90, 0, -secondaryObject.transform.rotation.eulerAngles.y));
		if(mainObject.GetComponent<RectTransform>()){
			mainObject.GetComponent<RectTransform>().rotation = newRotation;
		}
		else{
			mainObject.GetComponent<Transform>().rotation = newRotation;
		}
	}
	
	/*!Moves items on the minimap so they follow another object that is in the world
	 * The mainObject is the object to be moved, the secondaryObject is the object that the mainObject is following,
	 * and the positionOffest is an offset to have control over the depth of the minimap related objects.
	 */
	void moveMapObjectsFromOtherObject(GameObject mainObject, GameObject secondaryObject, Vector3 positionOffest){
		mainObject.transform.position = secondaryObject.transform.position + positionOffest;
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

		slider.SetActive (true);

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
		pointToObjective = objective.transform.position - compassCameraPoint.transform.position;
		pointToObjective.Normalize();

		//!Set this vector to point right away from camera
		pointStraightFromCam = -compassCameraPoint.transform.right;
		pointStraightFromCam.Normalize();

		//!return angle from right facing vector

		return Vector3.Angle(pointStraightFromCam, pointToObjective);


	}

	/*!When this function is called from a POP Event, it will move the target position of the
	 * compass. This points the compass to the next objective.
	 */
	[EventVisibleAttribute]
	public void MoveCompassTargetPoint(GameObject NextQuestLocation){
		testObjective.transform.position = NextQuestLocation.transform.position;
		calcCompass = true;
		return;
	}

	/*!When this function is called from a POP Event, it will begin a dialogue. The function
	 * takes in an integer that is the index of the dialogue (created via dialoguer)
	 */
	[EventVisibleAttribute]
	public void TriggerDialoguer(int dialogueIndex){
		QK_Character_Movement.Instance.inADialogue = true;
		Dialoguer.StartDialogue (dialogueIndex, dialoguerCallback);
	}

	/*!This function is called when the dialogue sequence ends
	 */
	void dialoguerCallback(){
		QK_Character_Movement.Instance.inADialogue = false;
	}

	//This is for testing
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Finish") {
			UpdateObjectiveText("Objective Complete!", false);
		}
	}


	//Shows map on phone and roates and resizes phone to screen
	/*public void showMap() {
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
	*/
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
	/*!Shows the pause menu
	 */
	public void showPauseMenu () {
		pauseMenu.SetActive (true);
		accessManager.isOnPauseMenu = true;
		pauseMenuResumeButton.Select ();
	}

	/*!Hides the pause menu
	 */
	public void hidePauseMenu () {
		pauseMenu.SetActive (false);
	}

	/*!Loads the scene with the name that is passed into the function
	 */
	public void loadScene(string s) {
		Application.LoadLevel(s);
	}

	//!When this function is called the game closes. Only works in a build of the game
	public void quitGame () {
		Application.Quit ();
	}
	
    public void openOptions() 
    {
        menuManager.GoToOptions();
    }
	
	/*!This function activates the QuestManagerUI and deactivates the Pause Menu
	 */
	public void ShowQMUI(){
		questManagerUI.SetActive (true);
		accessManager.isOnPauseMenu = false;
		questManagerUI.GetComponent<QuestManagerUIController> ().showQuests ();
		pauseMenu.SetActive (false);
	}

	/*!This function deactivates the QuestManagerUI and activates the Pause Menu
	 */
	public void HideQMUI(){
		questManagerUI.SetActive (false);
		showPauseMenu ();
		//pauseMenu.SetActive (true);
		//accessManager.isOnPauseMenu = true;
	}

	public void ShowControls(){
		showMinimap = false;
		controls.SetActive (true);
		accessManager.isOnPauseMenu = false;
		pauseMenu.SetActive (false);
		controls.transform.FindChild ("MainScrollView").FindChild ("ControlItems").FindChild ("KeyboardControlsButton").GetComponent<Button>().Select();
	}
	
	public void HideControls(){
		controls.SetActive (false);
		pauseMenu.SetActive (true);
		showPauseMenu ();
	}

	public void ShowPCControls(){
		pcControls.SetActive(true);
		xboxControls.SetActive (false);
		psControls.SetActive(false);
	}
	
	public void ShowXBOXControls(){
		pcControls.SetActive(false);
		xboxControls.SetActive (true);
		psControls.SetActive(false);
	}
	
	public void showPlayStationControls(){
		pcControls.SetActive(false);
		xboxControls.SetActive (false);
		psControls.SetActive(true);
	}

	/*! This function displays an icon above the object that will be targeted by the player should they press the target button
	 *  The function takes in a GameObject that the icon will be displayed above
	 *  If an object is currently being targeted, the icon will stay above it
	 *  If the object is an enemy, the enemy icon will be displayed above it
	 *  If the object can be affected by the player's selected ability, an icon with a corresponding color to the ability will be displayed above the object
	 *  Otherwise a blue icon is shown above the object
	 */
	void DisplayIconAboveTarget(GameObject targetObject){
        
		/*
        if(targetObject.GetComponent<Enemy>()){
			closestTargetIcon.GetComponent<Image>().sprite = enemyIcon;
		}
		else if(AbilityDockController.instance.getSelectedAbility() == 0 && targetObject.GetComponent<Item>().pullCompatible){
			closestTargetIcon.GetComponent<Image>().sprite = targetableIcons[0];
		}
		else if(AbilityDockController.instance.getSelectedAbility() == 1 && targetObject.GetComponent<Item>().pushCompatible){
			closestTargetIcon.GetComponent<Image>().sprite = targetableIcons[1];
		}
		else if(AbilityDockController.instance.getSelectedAbility() == 2 && targetObject.GetComponent<Item>().stunCompatible){
			closestTargetIcon.GetComponent<Image>().sprite = targetableIcons[2];
		}
		else if(AbilityDockController.instance.getSelectedAbility() == 3 && targetObject.GetComponent<Item>().soundThrowCompatible){
			closestTargetIcon.GetComponent<Image>().sprite = targetableIcons[3];
		}
		else if(AbilityDockController.instance.getSelectedAbility() == 4 && targetObject.GetComponent<Item>().cutCompatible){
			closestTargetIcon.GetComponent<Image>().sprite = targetableIcons[4];
		}
		else{
			closestTargetIcon.GetComponent<Image>().sprite = targetableIcons[5];
		}
		closestTargetIcon.transform.position = targetObject.transform.position + new Vector3(0, 3, 0);
		closestTargetIcon.transform.rotation = mainCamera.transform.rotation;
        */
	}
}