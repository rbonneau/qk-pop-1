using UnityEngine;
using System.Collections;

public interface IEnemyState
{

    void UpdateState();

    void OnTriggerEnter(Collider other);

    void ToPatrolState();

    void ToChaseState();

    void ToGuardState();

    void ToDazedState();

    void ToDistractedState();

    void ToSuspiciousState();

    void ToSearchingState();

    void ToKOState();

    void ToWalkState();

}