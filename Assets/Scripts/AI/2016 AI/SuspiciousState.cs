using UnityEngine;
using System.Collections;

public class SuspiciousState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    private float suspiciousTimer;
    private float suspiciousTimerLimit = 5f;
    private Color lerpedColor = Color.yellow;
    private bool checking = false;
    private float chaseTimer = 0;

    public SuspiciousState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Search();
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        suspiciousTimer = 0;
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        suspiciousTimer = 0;
    }

    public void ToGuardState()
    {

    }

    public void ToDazedState()
    {
        enemy.currentState = enemy.dazedState;
        chaseTimer = 0f;
        enemy.moveSpeed = 0f;
        suspiciousTimer = 0;
    }

    public void ToDistractedState(Transform distractedPoint)
    {

    }

    public void ToSearchingState()
    {

    }
    public void ToSuspiciousState()
    {
        Debug.Log("Cant transition into itself");
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
    //if player leaves line of sight during suspicion state (ie. hide behind a tree) and the player is a certain distance away, have the AI go over and investigate the players position
    private void Search()
    {
        RaycastHit hit;
        if (checking == false)
        {
            //if the player is in the ai's cone of vision
            if (Vector3.Angle(enemy.chaseTarget.position - enemy.transform.position, enemy.transform.forward) < enemy.sightAngle)
            {
                enemy.navMeshAgent.Stop();
                //look at the the player
                Quaternion lookAtPlayer = Quaternion.LookRotation(enemy.chaseTarget.position - enemy.transform.position);
                enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, lookAtPlayer, enemy.searchingTurnSpeed * 2 * Time.deltaTime);
                //if the ai can raycast towards the player up to the enemys sight distance, and no objects get in the way
                if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
                {
                    //increase the search timer until it reaches the limit then go to search state
                    if (suspiciousTimer > suspiciousTimerLimit)
                    {
                        ToChaseState();
                    }
                    else
                    {
                        suspiciousTimer += Time.deltaTime;
                    }
                }
                //if the player has broken line of sight by going behind another object or going too far away
                else
                {
                    //if suspiciousTimer is at 0 or below go to patrol state, otherwise decrease timer.
                    if (suspiciousTimer <= 0)
                    {
                        ToPatrolState(); ;
                    }
                    else
                    {
                        //if the player is within a certain distance, ai moves to the players position
                        if (Vector3.Distance(enemy.transform.position, enemy.chaseTarget.transform.position) <= enemy.suspiciousCheckRange)
                        {
                            checking = true;
                        }
                        suspiciousTimer -= Time.deltaTime;
                    }

                }
            }
            //if player is not in the ai cone of vision
            else
            {
                suspiciousTimer -= Time.deltaTime;
                if (suspiciousTimer <= 0)
                {
                    ToPatrolState();
                }
            }
        }
        else
        {
            enemy.navMeshAgent.destination = enemy.chaseTarget.position;
            enemy.navMeshAgent.Resume();
            chaseTimer += Time.deltaTime;
            if (enemy.moveSpeed <= 0)
            {
                enemy.moveSpeed = 0; //ensures that if the subtraction puts it into a negative value its set to 0
                ToDazedState();
            }
            else
            {
                enemy.moveSpeed = enemy.moveSpeed - (chaseTimer / 1000);
            }
            if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
            {
                checking = false;
            }
            //move to the players position 
            //if player is seen checking is false

        }
        lerpedColor = Color.Lerp(Color.yellow, Color.red, suspiciousTimer/suspiciousTimerLimit);
        enemy.meshRendererFlag.material.color = lerpedColor;
    }
}