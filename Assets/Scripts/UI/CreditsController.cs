using UnityEngine;
using System.Collections;

public class CreditsController : MonoBehaviour {

	public float scrollSpeed;
	public RectTransform[] creditTransforms;

	void Update(){
		foreach (RectTransform rt in creditTransforms) {
			rt.Translate(0, scrollSpeed * Time.deltaTime, 0);
		}

		if (creditTransforms [0].position.y >= 15000) {
			Debug.Log("Looping");
			foreach(RectTransform rt in creditTransforms){
				rt.transform.position -= new Vector3(0f, 15151f, 0f);
			}
		}
	}
}
