using UnityEngine;
using System.Collections;

public class PatrolState : IEnemyState

{
    private readonly StatePatternEnemy enemy;
    private float LookTimer;
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
        enemy.moveSpeed = enemy.chaseSpeed;
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
        enemy.moveSpeed = enemy.patrolSpeed;
    }

    public void ToSearchingState()
    {
        enemy.currentState = enemy.searchingState;
        enemy.moveSpeed = enemy.patrolSpeed;
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

    public void ToPointSearchState(float minAngle, float maxAngle, float turnSpeed, int searchCount)
    {
        enemy.pointSearchState.currentAngle = enemy.transform.forward;
        enemy._minAngle = minAngle;
        enemy._maxAngle = maxAngle;
        enemy._turnSpeed = turnSpeed;
        enemy._searchCount = searchCount;
        enemy.currentState = enemy.pointSearchState;
    }

    private void Look()
    {
        //if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        RaycastHit hit;
        if (Vector3.Angle(enemy.player.transform.position - enemy.transform.position, enemy.transform.forward) < enemy.sightAngle)
        {
            if (Physics.Raycast(enemy.transform.position, enemy.player.transform.position - enemy.transform.position, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
                //check if player is hidden - if not hidden run below else do nothing (player is not seen)
            {
                enemy.chaseTarget = hit.transform;
                //if enemy is alert type
                //ToChaseState();
                ToSuspiciousState();
            }
        }
    }
    void Patrol()
    {
        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending && enemy.navMeshAgent.remainingDistance != 0)
        {
            if (enemy.Pathways[enemy.PathwayCount] == null)
            {
                return;
            }
            enemy.Path = enemy.Pathways[enemy.PathwayCount];
            AIPath CheckpointScript = enemy.Path.GetComponent<AIPath>();
            if (enemy.PathwayCount <= enemy.Pathways.Count - 1)
            {
                if (enemy.Path == null)
                {
                    Debug.Log("there is no assigned path");
                    return;
                }
                switch (enemy.PathType[enemy.PathwayCount])
                {
                    case 0: //From A to B to C etc (one way)
                        if (enemy.CheckpointCount < CheckpointScript.getPoints().Count)
                        {
                            if (enemy.CheckpointCount != CheckpointScript.getPoints().Count)
                            {
                                if (CheckpointScript.getSearch()[enemy.CheckpointCount] == true)
                                {
                                    ToPointSearchState(CheckpointScript.getMinAngle()[enemy.CheckpointCount], CheckpointScript.getMaxAngle()[enemy.CheckpointCount], CheckpointScript.getTurnSpeed()[enemy.CheckpointCount], CheckpointScript.getLoopCount()[enemy.CheckpointCount]);
                                }
                                enemy.CheckpointCount++;
                                enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];

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
                                if (enemy.CheckpointCount < CheckpointScript.getPoints().Count - 1)
                                {
                                    if (CheckpointScript.getSearch()[enemy.CheckpointCount] == true)
                                    {
                                        ToPointSearchState(CheckpointScript.getMinAngle()[enemy.CheckpointCount], CheckpointScript.getMaxAngle()[enemy.CheckpointCount], CheckpointScript.getTurnSpeed()[enemy.CheckpointCount], CheckpointScript.getLoopCount()[enemy.CheckpointCount]);
                                    }
                                    enemy.CheckpointCount++;
                                    enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];
                                }
                                else
                                {
                                    if (CheckpointScript.getSearch()[enemy.CheckpointCount] == true)
                                    {
                                        ToPointSearchState(CheckpointScript.getMinAngle()[enemy.CheckpointCount], CheckpointScript.getMaxAngle()[enemy.CheckpointCount], CheckpointScript.getTurnSpeed()[enemy.CheckpointCount], CheckpointScript.getLoopCount()[enemy.CheckpointCount]);
                                    }
                                    enemy.CheckpointCount = 0;
                                    enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];
                                    if (!enemy.infinite[enemy.PathwayCount])
                                    {
                                        enemy.LoopCount++;
                                    }
                                }
                            }
                            else
                            {
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
                            if ((enemy.CheckpointCount < CheckpointScript.getPoints().Count -1) && (enemy.back == false))
                            {
                                if (enemy.CheckpointCount != CheckpointScript.getPoints().Count- 1)
                                {
                                    if (CheckpointScript.getSearch()[enemy.CheckpointCount] == true)
                                    {
                                        ToPointSearchState(CheckpointScript.getMinAngle()[enemy.CheckpointCount], CheckpointScript.getMaxAngle()[enemy.CheckpointCount], CheckpointScript.getTurnSpeed()[enemy.CheckpointCount], CheckpointScript.getLoopCount()[enemy.CheckpointCount]
                );
                                    }
                                    enemy.CheckpointCount++;
                                    enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];
                                }
                            }
                            else
                            {
                                if (enemy.CheckpointCount > 0)
                                {
                                    enemy.back = true;
                                    if (CheckpointScript.getSearch()[enemy.CheckpointCount] == true)
                                    {
                                        ToPointSearchState(CheckpointScript.getMinAngle()[enemy.CheckpointCount], CheckpointScript.getMaxAngle()[enemy.CheckpointCount], CheckpointScript.getTurnSpeed()[enemy.CheckpointCount], CheckpointScript.getLoopCount()[enemy.CheckpointCount]);
                                    }
                                    enemy.CheckpointCount--;
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
