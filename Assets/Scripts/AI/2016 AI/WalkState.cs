using UnityEngine;
using System.Collections;

public class WalkState : IEnemyState
{
    private readonly StatePatternEnemy enemy;

    public WalkState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Walk();
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void ToPatrolState()
    {
        Debug.Log("Cant transition into itself");
    }

    public void ToChaseState()
    {

    }

    public void ToGuardState()
    {

    }

    public void ToDazedState()
    {

    }

    public void ToDistractedState(Transform distractedPoint)
    {

    }

    public void ToSearchingState()
    {

    }
    public void ToSuspiciousState()
    {

    }

    public void ToKOState()
    {

    }

    public void ToWalkState()
    {

    }

    public void ToPointSearchState(float minAngle, float maxAngle, float turnSpeed, int searchCount)
    {

    }
    void Walk()
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
                            if (CheckpointScript.getSearch()[enemy.CheckpointCount] == true)
                            {
                                ToPointSearchState(CheckpointScript.getMinAngle()[enemy.CheckpointCount], CheckpointScript.getMaxAngle()[enemy.CheckpointCount], CheckpointScript.getTurnSpeed()[enemy.CheckpointCount], CheckpointScript.getLoopCount()[enemy.CheckpointCount]);
                            }
                            enemy.CheckpointCount++;
                            enemy.navPoint = CheckpointScript.getPoints()[enemy.CheckpointCount];
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
                            Debug.Log("works");
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
                            if ((enemy.CheckpointCount < CheckpointScript.getPoints().Count - 1) && (enemy.back == false))
                            {
                                if (enemy.CheckpointCount != CheckpointScript.getPoints().Count - 1)
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
        enemy.navMeshAgent.destination = enemy.navPoint;
        enemy.navMeshAgent.Resume();
        enemy.navMeshAgent.speed = enemy.moveSpeed;

    }
}
