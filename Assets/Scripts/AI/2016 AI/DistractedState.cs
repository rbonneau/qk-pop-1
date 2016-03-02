using UnityEngine;
using System.Collections;

public class DistractedState : IEnemyState
{
    private readonly StatePatternEnemy enemy;

    public DistractedState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Distracted();
    }

    public void OnTriggerEnter(Collider col)
    {

    }

    public void ToPatrolState()
    {

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

    public void Distracted ()
    {
        enemy.chaseTarget = enemy.noiseLoc;
        enemy.navMeshAgent.destination = enemy.chaseTarget.position;

    }
}
