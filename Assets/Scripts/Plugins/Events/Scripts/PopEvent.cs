using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopEvent : MonoBehaviour {

    public List<EventHalf> conditions;
    public List<EventHalf> actions;

    public string[] andOrCompare = new string[] { "Every Condition", "One or More", "Exactly One" };
    public string andOrCompareString;
    public int andOrCompareIndex;

	public string[] iconType = new string[] {"Blue '!'", "Blue '?'", "Blue Star"};
	public string iconTypeString;
	public int iconTypeIndex;

    public string uniqueId; //!<    Used to identify an event from anywhere in the scene

    public bool isActive = true;
    public bool isRegional = true;
    public bool executeOnce = true;
    public bool hasExecuted = false;
	
	public bool addMapIcon;
	
    private PopEvent nextEvent;

    public float totalTimeActive = 0;
    private float timer = 0;
    public float delay = 1;

    public float regionRadius = 1;
    public bool drawRegionTwo = false;
    public Vector3 conditionRegionCenter = new Vector3(0, 0, 0);
    public float conditionRegionRadius = 1;
    
    bool activeOnce = false;		//! Used to execute commands exactly one time when the pop event becomes active

	GameObject minimapCanvas;
	GameObject mapIcon;
	
    public PopEvent() {
        conditions = new List<EventHalf>();
        conditions.Add(new EventHalf());
        actions = new List<EventHalf>();
        actions.Add(new EventHalf());
    }

    void Awake() {
    	minimapCanvas = GameObject.Find ("worldMapCanvas");
    	if(!minimapCanvas){
    		Debug.LogError("Could not find the 'worldMapCanvas' GameObject");
    	}
        EventListener.AddPopEvent(this);
        PopEvent[] popEvents = gameObject.GetComponents<PopEvent>();
        for (int i = 0; i < popEvents.Length - 1; i++) { //  Don't check the last element
            if (this.Equals(popEvents[i])) {
                nextEvent = popEvents[i + 1];
                break;
            }
        }

        foreach (EventHalf condition in conditions){
            if (condition.e_classString == "Player Enters Area" || condition.e_classString == "Player Leaves Area") {
                SphereCollider newCollider = gameObject.AddComponent<SphereCollider>();
                newCollider.radius = conditionRegionRadius;
                newCollider.isTrigger = true;
                gameObject.layer = 2;
                break;
            }
        }
    }

    void Update() {
        if (isActive == false) {
        	if(!mapIcon){
        		return;
        	}
        	else{
     		   	GameHUD.Instance.mapLabels.Remove(mapIcon);
        		Destroy (mapIcon);
        	}
        }
        if(isActive && !activeOnce){
        	activeOnce = true;
        	if(addMapIcon){
        		switch(iconTypeString){
					case "Blue '!'":
						mapIcon = GameObject.Instantiate (Resources.Load("MapIconExclamationMark"), new Vector3(transform.position.x, transform.position.z, -15), Quaternion.identity) as GameObject;	
						break;
					case "Blue '?'":
						mapIcon = GameObject.Instantiate (Resources.Load("MapIconQuestionMark"), new Vector3(transform.position.x, transform.position.z, -15), Quaternion.identity) as GameObject;	
						break;
					case "Blue Star":
						mapIcon = GameObject.Instantiate (Resources.Load("MapIconStar"), new Vector3(transform.position.x, transform.position.z, -15), Quaternion.identity) as GameObject;	
						break;
					default:
						Debug.LogError ("Could not determine which icon to display in the POP Event of: " + gameObject.name);
						return;
				}
				mapIcon.transform.SetParent(minimapCanvas.transform, false);
				mapIcon.transform.SetSiblingIndex (1);
				GameHUD.Instance.mapLabels.Add(mapIcon);
        	}
        }
        if (executeOnce == true && hasExecuted == true) { return; }

        totalTimeActive += Time.deltaTime;

        if (timer < delay) {
            timer += Time.deltaTime;
        }
        if (timer >= delay) {
            timer = 0;
            EventListener.SlowUpdate(this);
        }
    }

    void OnTriggerEnter(Collider other) {
		//DebugOnScreen.Log ("Entering the POP event");
        if (isActive == false) { return; }
        if (other.gameObject.GetComponent<QK_Character_Movement>())
        {
			//DebugOnScreen.Log (other.gameObject.name + " is entering the POP event " + this.gameObject.name);
			//DebugOnScreen.Log(other.gameObject.name);
			//DebugOnScreen.Log (this.name);
            EventListener.SlowUpdate(this);
        }
    }
    void OnTriggerExit(Collider other) {
        if (isActive == false) { return; }
		if (other.gameObject.GetComponent<QK_Character_Movement>())
        {
            EventListener.SlowUpdate(this);
        }
    }

    void OnDrawGizmosSelected() {
        // The other portion of the custom Editor graphics are found in PopEventEditor.cs
        if (isRegional == true) {
        }

        if (drawRegionTwo) {
            Gizmos.color = new Color(0, 1, 0, 0.3F);
            Gizmos.DrawSphere(transform.position, conditionRegionRadius);
        }
    }

    public void ActivateNextEvent() {
        if (nextEvent != null) {
            nextEvent.MakeActive(true);
        }
    }

    public void MakeActive(bool active, bool force = false) {
        if (active == true && executeOnce == true && hasExecuted == true && force == false) { return; }
        isActive = active;
    }
    
    void OnDestroy(){
    	if(mapIcon){
    		GameHUD.Instance.mapLabels.Remove (mapIcon);
    		Destroy(mapIcon);
    	}
    }
}