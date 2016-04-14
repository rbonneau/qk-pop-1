using UnityEngine;
using System.Collections;

public class DialogueCheck : MonoBehaviour {
	
	public bool hasDialogue;
	public int dialogueNumber;

	private GameObject initiation;
	private GameObject player;
	private Vector3 targetPos;
	private Quaternion targetRotation;
	private Quaternion originalPos;
	private bool dialogueCheck;
	private GameObject NPC;

	void Start() {
		initiation = GameObject.Find("Init");
		if (!initiation) {
			Debug.LogError ("Could not find the 'Init' GameObject in the scene: " + Application.loadedLevelName + ". Should be in the UI prefab");
		}
		player = GameObject.FindGameObjectWithTag ("Player");
		if (!player) {
			Debug.LogError("Could not find the 'Player' GameObject in the scene: " + Application.loadedLevelName);
		}
		originalPos = transform.rotation;
		NPC = gameObject;
	}

	void LateUpdate(){
		dialogueCheck = initiation.GetComponent<Init> ().inDialogue;
		if (dialogueCheck && player.GetComponentInChildren<DialogueCollider>().NPCFocus == NPC) {
			targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position;
			targetRotation = Quaternion.LookRotation (targetPos, Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
		}
		if (!dialogueCheck) {
			transform.rotation = Quaternion.Slerp(transform.rotation, originalPos, Time.deltaTime * 2.0f);
		}
	}
}