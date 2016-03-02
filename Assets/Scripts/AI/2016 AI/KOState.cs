using UnityEngine;
using System.Collections;

public class KOState : IEnemyState
{

    private readonly StatePatternEnemy enemy;

    public KOState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {

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
}
