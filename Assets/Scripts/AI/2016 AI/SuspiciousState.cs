using UnityEngine;
using System.Collections;

public class SuspiciousState : IEnemyState
{
    private readonly StatePatternEnemy enemy;

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
    /*
        sus greater than 0
            
    */
    }
}