using UnityEngine;
using System.Collections;

public class SuspiciousState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    private float suspiciousTimer;
    private float suspiciousTimerLimit = 5f;
    private Color lerpedColor = Color.yellow;

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

    }

    public void ToDistractedState()
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

    private void Search()
    {
        enemy.sightAngle = 60f;
        enemy.sightRange = 20f;
        RaycastHit hit;
        if (Vector3.Angle(enemy.chaseTarget.position - enemy.transform.position, enemy.transform.forward) < enemy.sightAngle)
        {
            enemy.navMeshAgent.Stop();
            Quaternion lookAtPlayer = Quaternion.LookRotation(enemy.chaseTarget.position - enemy.transform.position);
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, lookAtPlayer, enemy.searchingTurnSpeed * 2 * Time.deltaTime);
            if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
            {
                if (suspiciousTimer > suspiciousTimerLimit)
                {
                    ToChaseState();
                }
                else
                {
                    suspiciousTimer += Time.deltaTime;
                }
            }
            else
            {
                if (suspiciousTimer <= 0)
                {
                    ToPatrolState(); ;
                }
                else
                {
                    suspiciousTimer -= Time.deltaTime;
                }
            }
        }
        
        else
        {
            if (suspiciousTimer <= 0)
            {
                ToChaseState();
            }
            else
            {
                suspiciousTimer -= Time.deltaTime;
            }
        }
        lerpedColor = Color.Lerp(Color.yellow, Color.red, suspiciousTimer/suspiciousTimerLimit);
        enemy.meshRendererFlag.material.color = lerpedColor;
    }
}