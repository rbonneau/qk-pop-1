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
        enemy.moveSpeed = 5f;
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

    public void ToDistractedState()
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

    private void Dazed()
    {
        dazeTimer += Time.deltaTime;
        if (dazeTimer > 3f)
        {
            ToPatrolState();
        }
        enemy.meshRendererFlag.material.color = Color.gray;
    }
}
