using UnityEngine;
using System.Collections;

public class DazedState : IEnemyState
{

    private float dazeTimer;

    private readonly StatePatternEnemy enemy;

    public DazedState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Dazed();
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        dazeTimer = 0f;
        enemy.moveSpeed = enemy.patrolSpeed;
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

    public void ToSuspiciousState()
    {

    }

    public void ToSearchingState()
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

    private void Dazed()
    {
        dazeTimer += Time.deltaTime;
        if (dazeTimer > 6f)
        {
            ToPatrolState();
        }
        enemy.meshRendererFlag.material.color = Color.gray;
    }
}
