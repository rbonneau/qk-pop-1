using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiSpawnTrigger : MonoBehaviour
{
    public bool spawned = false;
    public GameObject toActivate;
    public List<GameObject> AI;

    void OnTriggerEnter(Collider col)
    {


        if (col.gameObject == QK_Character_Movement.Instance.gameObject && spawned == false)
        {
            spawnEnemy();
        }
    }
    void spawnEnemy()
    {
        spawned = true;
        foreach (GameObject ai in AI)
        {
            ai.SetActive(true);
        }

        //set spawned to true;
        //for every object in the list
        //toggle the active state of the object

    }
}
