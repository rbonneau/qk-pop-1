using UnityEngine;
using System.Collections;

public class PatrolState : IEnemyState

{
    private readonly StatePatternEnemy enemy;
    private int nextWayPoint;

    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }


    public void UpdateState()
    {
        Look();
        Patrol();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ToSearchingState();
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

    public void ToDistractedState()
    {

    }

    public void ToSearchingState()
    {
        enemy.currentState = enemy.searchingState;
        enemy.moveSpeed = 5f;
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

    private void Look()
    {
        RaycastHit hit;
        //if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))

            {
                enemy.chaseTarget = hit.transform;
            ToChaseState();
            } 
    }
    void Patrol()
    {
        /*
        enemy.meshRendererFlag.material.color = Color.green;
        enemy.navMeshAgent.destination = enemy.wayPoints[nextWayPoint].position;
        enemy.navMeshAgent.Resume();

        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending)
        {
            nextWayPoint = (nextWayPoint + 1) % enemy.wayPoints.Length;
        }
        */
        

        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending)
        {
            if (enemy.PathwayCount <= enemy.Pathways.Count - 1)
            {
                enemy.Path = enemy.Pathways[enemy.PathwayCount];
                if (enemy.Path == null)
                {
                    return;
                }
                AIPath CheckpointScript = enemy.Path.GetComponent<AIPath>();

                switch (enemy.PathType[enemy.PathwayCount])
                {

                    case 0:
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

                    case 1:
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

                    case 2:
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

                    case 3:
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
        enemy.navMeshAgent.destination = enemy.navPoint;
        enemy.navMeshAgent.Resume();
    }

}
