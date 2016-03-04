using UnityEngine;
using System.Collections;

public class PatrolState : IEnemyState

{
    private readonly StatePatternEnemy enemy;

    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }


    public void UpdateState()
    {
        Look();
        Patrol();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent("PlayNoise") != null)
        {
            enemy.noiseLoc = col.gameObject.transform;
            enemy.currentState = enemy.distractedState;
        }
    }

    public void ToPatrolState()
    {
        Debug.Log("Cant transition into itself");
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        enemy.moveSpeed = 10f;
    }

    public void ToGuardState()
    {

    }

    public void ToDazedState()
    {

    }

    public void ToDistractedState(Transform distractedPoint)
    {
        enemy.currentState = enemy.distractedState;
        enemy.noiseLoc = distractedPoint;
        enemy.moveSpeed = 5f;
    }

    public void ToSearchingState()
    {
        enemy.currentState = enemy.searchingState;
        enemy.moveSpeed = 5f;
    }

    public void ToSuspiciousState()
    {
        enemy.seesTarget = false;
        enemy.currentState = enemy.suspiciousState;
    }

    public void ToKOState()
    {

    }

    public void ToWalkState()
    {

    }

    private void Look()
    {
        //if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        RaycastHit hit;
        if (Vector3.Angle(enemy.player.transform.position - enemy.transform.position, enemy.transform.forward) < enemy.sightAngle)
        {
            if (Physics.Raycast(enemy.transform.position, enemy.player.transform.position - enemy.transform.position, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
            {
                Debug.Log("works");
                enemy.chaseTarget = hit.transform;
                //if enemy is alert type
                //ToChaseState();
                ToSuspiciousState();
            }
        }
    }
    void Patrol()
    {
        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending)
        {
            enemy.Path = enemy.Pathways[enemy.PathwayCount];
            AIPath CheckpointScript = enemy.Path.GetComponent<AIPath>();
            if (enemy.PathwayCount <= enemy.Pathways.Count - 1)
            {
                if (enemy.Path == null)
                {
                    Debug.Log("there is no assigned path");
                    return;
                }
                //have the searchcheck here? if the current checkpoint search is true then search
                switch (enemy.PathType[enemy.PathwayCount])
                {

                    case 0: //From A to B to C etc (one way)
                        if (enemy.CheckpointCount < CheckpointScript.getPoints().Count)
                        {
                            enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];
                            if (enemy.CheckpointCount != CheckpointScript.getPoints().Count)
                            {
                                enemy.CheckpointCount++;
                            }
                        }
                        else
                        {
                            if (enemy.PathwayCount != enemy.Pathways.Count - 1)
                            {
                                enemy.PathwayCount++;
                                enemy.CheckpointCount = 0;
                            }
                            else
                            {
                                return;
                            }
                        }
                        break;

                    case 1: //looping
                        if (enemy.LoopCount <= enemy.nofLoops[enemy.PathwayCount])
                        {
                            if (enemy.CheckpointCount < CheckpointScript.getPoints().Count)
                            {
                                enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];

                                if (enemy.CheckpointCount != CheckpointScript.getPoints().Count)
                                {
                                    enemy.CheckpointCount++;
                                }
                            }
                            else
                            {
                                enemy.CheckpointCount = 0;

                                if (!enemy.infinite[enemy.PathwayCount])
                                {
                                    enemy.LoopCount++;
                                }
                            }
                        }
                        else
                        {
                            enemy.PathwayCount++;
                            enemy.CheckpointCount = 0;
                            enemy.LoopCount = 1;
                        }
                        break;

                    case 2: //back and forth
                        if (enemy.LoopCount <= enemy.nofLoops[enemy.PathwayCount])
                        {
                            if ((enemy.CheckpointCount < CheckpointScript.getPoints().Count) && (enemy.back == false))
                            {
                                enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];
                                if (enemy.CheckpointCount != CheckpointScript.getPoints().Count)
                                {
                                    enemy.CheckpointCount++;
                                }
                            }
                            else
                            {
                                if (enemy.CheckpointCount > 0)
                                {
                                    enemy.back = true;
                                    enemy.CheckpointCount--;
                                    string CheckpointCountString = enemy.CheckpointCount.ToString();
                                    enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];

                                }
                                else
                                {
                                    enemy.back = false;

                                    if (!enemy.infinite[enemy.PathwayCount])
                                    {
                                        enemy.LoopCount++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            enemy.PathwayCount++;
                            enemy.CheckpointCount = 0;
                            enemy.LoopCount = 1;
                        }
                        break;

                    case 3: //guard a single point
                        if (enemy.CheckpointCount < CheckpointScript.getPoints().Count)
                        {

                            string CheckpointCountString = enemy.CheckpointCount.ToString();
                            enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];
                            if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending)
                                enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, CheckpointScript.getRotations()[enemy.CheckpointCount], enemy.searchingTurnSpeed * 2 * Time.deltaTime);

                        }
                        break;
                }
            }
            else
            {

            }





        }
        enemy.meshRendererFlag.material.color = Color.green;
        enemy.navMeshAgent.destination = enemy.navPoint;
        enemy.navMeshAgent.Resume();
    }

}
