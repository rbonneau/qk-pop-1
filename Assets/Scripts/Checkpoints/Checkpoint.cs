using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (SphereCollider))]
/*! \class Checkpoint
 * \brief It's a standard checkpoint.
 * 
 * Checkpoint reports to CheckpointManager. A checkpoint will add itself to the list Checkpoint.AllCheckpoints on awake
 * and enable. It will remove itself from the list when it is disabled. When the player collides with the checkpoint
 * the checkpoint sets itself to CheckpointManager.LatestWorldCheckpoint. 
 */
public class Checkpoint : MonoBehaviour
{
//START OLD CODE
    public float minDist = 0.0f; //!<Minimum distance away from the Checkpoint that CheckpointTrigger needs to be
    public bool aiSearch;
	public AIPath path_reference;

//END OLD CODE

	void OnAwake()
	{

		//add self to list of all active checkpoints
		OnEnable();

	}//END void OnAwake()

//START OLD CODE
    void OnDrawGizmos()
	{
        Gizmos.DrawIcon (transform.position, "checkpointGizmo.png");
    }

	public Vector3 getPosition()
    {
		return transform.position;
	}

    public Quaternion getRotation()
    {
        return transform.rotation;
    }

    public bool getSearch()
    {
        return aiSearch;
    }
//END OLD CODE

    //These script is for the AI checkpoints however this segment of code is for the players checkpoints
	void OnTriggerEnter(Collider col)
	{

        FFP.Debug.Log("checkpoint", "Checkpoint: collision trigger");

		if(col.gameObject == QK_Character_Movement.Instance.gameObject)
		{
			//make self the most recently reached checkpoint
			CheckpointManager.instance.SetLatestWorldCheckpoint(transform);
            //			CheckpointManager.LatestWorldCheckPoint = transform;
            FFP.Debug.Log("checkpoint", "Checkpoint: " + gameObject.name + " is LatestWorldCheckpoint");
		}
		else
		{
            FFP.Debug.Log("checkpoint", "Checkpoint: collision with non player collider");
		}

	}//END void OnTriggerEnter(Collider col)
    
	void OnEnable()
	{

		//if this checkpoint is not on the list of active checkpoints
		if(!CheckpointManager.AllCheckpoints.Contains(transform))
		{

			//add to list
			CheckpointManager.AllCheckpoints.Add(transform);
            FFP.Debug.Log("checkpoint", "Checkpoint: " + gameObject.name + " enabled and added to list AllCheckpoints");

		}

	}//END void OnEnable()

	void OnDisable()
	{

		//remove self from list when checkpoint is disabled or destroyed
		CheckpointManager.AllCheckpoints.Remove(transform);
		FFP.Debug.Log("checkpoint", "Checkpoint: " + gameObject.name + " disabled and removed from list AllCheckpoints");

	}//END void OnDisable()

}//END public class Checkpoint : MonoBehaviour
