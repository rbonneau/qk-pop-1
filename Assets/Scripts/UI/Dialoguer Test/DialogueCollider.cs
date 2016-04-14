﻿using UnityEngine;
using System.Collections;

public class DialogueCollider : MonoBehaviour {

	public int NPCDialogueNumber;
	public bool NPCDialogue;
	public GameObject NPCFocus;
	public bool NPCProgressQuest;

	void Start ()
	{
		NPCDialogue = false;
	}

	void OnTriggerStay (Collider col) {
		if (col.tag == "Player")
			return;
		if (col.tag == "NPC") {
			NPCFocus = col.gameObject;
			if (NPCFocus.GetComponent<DialogueCheck> ().hasDialogue == true) {
				NPCDialogueNumber = NPCFocus.GetComponent<DialogueCheck> ().dialogueNumber;
				NPCDialogue = true;
			}
		}
	}

	void OnTriggerExit (Collider col) {
		NPCFocus = null;
		NPCDialogue = false;
	}

	
	public void ProgressQuest(){
		if (NPCFocus.GetComponent<ProgressQuestAfterDialogue> ()) {
			NPCFocus.GetComponent<ProgressQuestAfterDialogue> ().ProgressQuest ();
		}
	}
}