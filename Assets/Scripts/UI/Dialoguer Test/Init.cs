using UnityEngine;
using System.Collections;
using Debug = FFP.Debug;

public class Init : MonoBehaviour {

	//Design options for placement and size of prompt textbox and text
	public string textBoxText;
	public int textBoxWidth;
	public int textBoxHeight;
	public int textBoxVertOffset;
	public int textBoxHorzOffset;
	public bool inDialogue;
	
	private GameObject player;
	private bool dialogueCol;
	private DialogueCollider playerDC;

	GameObject interactDialoguer;

	void Awake () {
		Dialoguer.Initialize ();
		interactDialoguer = GameObject.Find ("InteractDialoguer");
		if (!interactDialoguer) {
			Debug.Log ("ui", "Could not find the 'InteractDialoguer' GameObject in the scene: " + Application.loadedLevelName);
		} else {
			interactDialoguer.SetActive (false);
		}
	}

	// Use this for initialization
	void Start () {
		dialogueCol = false;
		player = GameObject.FindGameObjectWithTag ("Player");
		playerDC = player.GetComponentInChildren<DialogueCollider> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Check if NPC has dialogue
		if (playerDC.NPCDialogue == true) {
			//Asks player if they want to interact
			dialogueCol = true;
			interactDialoguer.SetActive(true);
			if (Input.GetKeyDown (KeyCode.F)) {
				QK_Character_Movement.Instance.inADialogue = true;
				Dialoguer.StartDialogue (playerDC.NPCDialogueNumber, dialoguerCallback);
				this.enabled = false;
				inDialogue = true;
				interactDialoguer.SetActive (false);
			}
		}
		else {
			dialogueCol = false;
			interactDialoguer.SetActive (false);
		}
		/*if (!dialogueGUI.GetComponent<DialoguerGUI> ().showingDialoguer) {
			inDialogue = false;
		}*/
	}

	//***********OLD IMPLEMENTATION***************
	//Checks if player is interacting and runs dialoguer
	/*
	void OnGUI() {
		if (dialogueCol) {
			GUI.Box (new Rect ((Screen.width / 2) - (textBoxWidth / 2) + textBoxHorzOffset, (Screen.height / 2) - (textBoxHeight / 2) + textBoxVertOffset, textBoxWidth, textBoxHeight), textBoxText);
			if (triggerDialoguer) {
				Dialoguer.StartDialogue (player.GetComponentInChildren<DialogueCollider> ().NPCDialogueNumber, dialoguerCallback);
				this.enabled = false;
				triggerDialoguer = false;
				inDialogue = true;
			}
		}
	}
	*/
	//********************************************

	/*!This function is called at the end of a dialogue sequnce.
	 * This calls the progress quest function on the player, which
	 * calls a function on the NPC the player was interacting with
	 * to progress a certain quest.
	 * 
	 * See ProgressQuestAfterDialogue.cs for more info
	 */
	private void dialoguerCallback(){
		QK_Character_Movement.Instance.inADialogue = false;
		playerDC.ProgressQuest ();
		this.enabled = true;
	}
}
