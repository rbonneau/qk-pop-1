using UnityEngine;
using System.Collections;
using Debug = FFP.Debug;
/*
stealth mini game triggered when one enemy is in the suspicious state
one list of enemies
*/
 //public function that decrements timer of all ai in the alert/suspicious state
public class AIManager : MonoBehaviour {

    public GameObject[] AiChildren;
    public bool playerHidden;
    public int numberChasing;
    public GameObject alertScript;

    private static AIManager _instance;
    public static AIManager instance
    {
        get
        {
            _instance = _instance ?? (_instance = GameObject.FindObjectOfType<AIManager>());
            if (_instance == null)
            {
                Debug.Warning("ai", "AI Manager is not in scene but a script is attempting to reference it.");
            }
            return _instance;
        }
    }

    private AIManager() { }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //for each object with the StatePatternEnemy script and that is not a child of this object, add as a child to this object
        StatePatternEnemy[] notChildren = FindObjectsOfType(typeof(StatePatternEnemy)) as StatePatternEnemy[];
        foreach(StatePatternEnemy ai in notChildren)
        {
            ai.transform.parent = this.gameObject.transform;
        }
    }


    void Start ()
    {
        AiChildren = new GameObject[transform.childCount];
        int childCount = 0;
        foreach (Transform child in transform)
        {
            AiChildren[childCount] = child.gameObject;
            childCount++;
        }
    }

    void Update()
    {

        numberChasing = 0;
        for (int i = 0; i < AiChildren.Length; i++)
        {
            //Checks if any of the AI that were chasing the target can see the player
            if (AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "ChaseState" || AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "SearchingState")
            {
                numberChasing++;
            }
            i++;
        }
        if (numberChasing == 0)
        {
            playLessIntense();
        }
        else
        {
            playIntense();
        }
        
    }
    


    public int checkChasing()
    {
        numberChasing = 0;
        for (int i = 0; i < AiChildren.Length; i++)
        {
            //Checks if any of the AI that were chasing the target can see the player
            if (AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "ChaseState" || AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "SearchingState")
            {
                numberChasing++;
            }
            i++;
        }
        if (numberChasing == 0)
        {
            alertScript.GetComponent<Alerted>().stopAudio();
        }
        return numberChasing;
    }


    public bool checkForPlayer ()
    {
        for (int i = 0; i < AiChildren.Length; i++)
        {
            //Checks if any of the AI that were chasing the target can see the player
            if (AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "ChaseState" && AiChildren[i].GetComponent<StatePatternEnemy>().seesTarget == false)
            {
                playerHidden = true;
                alertScript.GetComponent<Alerted>().lessintense();
                break;
            }
            playerHidden = false;
            alertScript.GetComponent<Alerted>().intense();
            i++;
        }
        return playerHidden;
    }

    public void resumePatrol ()
    {
        for (int i = 0; i < AiChildren.Length; i++)
        {
            //Checks if any of the AI that were chasing the target can see the player
            if (AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "ChaseState" || AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "SearchingState")
            {
                AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToPatrolState();
            }
        }
    }

    public void resumeChase()
    {
        for (int i = 0; i < AiChildren.Length; i++)
        {
            //Checks if any of the AI that were chasing the target can see the player
            if (AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "ChaseState" || AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "SearchingState")
            {
                AiChildren[i].GetComponent<StatePatternEnemy>().chaseTarget = AiChildren[i].GetComponent<StatePatternEnemy>().player.transform;
                AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToChaseState();
            }
        }
    }

    private void playIntense()
    {
        alertScript.GetComponent<Alerted>().intense();
    }

    private void playLessIntense()
    {
        alertScript.GetComponent<Alerted>().lessintense();
    }

    /*
    to add
        raycast to ovject the player is hiding in
        if player is colliding with the object then the AI can see them


    */











    /* OLD CODE
    Transform[] AI_children;
	int AI_children_length;

	// Start Gathers all children that this manager is in charge of
	void Start () 
	{
		AI_children = GetComponentsInChildren<Transform> ();
		AI_children_length = AI_children.Length;
	}
	//Tells all AI in group to head to given point
	void SetNav (string target, Vector3 position) 
	{
		for (int i = 0; i < AI_children_length; i++) 
		{
			AI_children[i].GetComponent<AIMain>().ChangeNavPoint(target,position);		
		}
	}
	//Tells all AI in group to panic
		//Aggressive AI will not panic - use on non_aggressive NPC
	void SetPanic ()
	{
		for (int i = 0; i < AI_children_length; i++) 
		{
			AI_children[i].GetComponent<AIMain>().panic = true;
		}
	}
	//Tells all AI in group to become aggressive
	void SetAggression ()
	{
		for (int i = 0; i < AI_children_length; i++) 
		{
			AI_children[i].GetComponent<AIMain>().aggression = true;
		}
	}
	//AI become immediatly aggressive and will hunt and attack the player
	void AttackPlayer ()
	{
		for (int i = 0; i < AI_children_length; i++) 
		{
			AI_children[i].GetComponent<AIMain>().aggression = true;
			AI_children[i].GetComponent<AIMain>().aggressionLevel = AI_children[i].GetComponent<AIMain>().aggressionLimit;
			AI_children[i].GetComponent<AIMain>().seesTarget = true;
		}
	}
	//Delete all AI children
	void Destroy()
	{
		for (int i = 0; i < AI_children_length; i++) 
		{
			Destroy(AI_children[i]);
		}
	}
	//AI are told to return to their start points
	void Return()
	{
		for (int i = 0; i < AI_children_length; i++) 
		{
			AI_children[i].GetComponent<AIMain>().ChangeNavPoint("Start Path",GetComponent<AIMain>().startPoint.transform.position); 
		}
	}
		*/
}
