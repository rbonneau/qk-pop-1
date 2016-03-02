using UnityEngine;
using System.Collections;
/*
stealth mini game triggered when one enemy is in the suspicious state
one list of enemies
*/
 //public function that decrements timer of all ai in the alert/suspicious state
public class AIManager : MonoBehaviour {

    public GameObject[] AiChildren;
    public bool playerHidden;
    public int numberChasing;

    private static AIManager instance;

    private AIManager() { }

    public static AIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AIManager();
            }
            return instance;
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
                break;
            }
            playerHidden = false;
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
