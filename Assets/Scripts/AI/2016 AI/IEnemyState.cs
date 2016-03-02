using UnityEngine;
using System.Collections;

public interface IEnemyState
{

    void UpdateState();

    void OnTriggerEnter(Collider col);

    void ToPatrolState();

    void ToChaseState();

    void ToGuardState();

    void ToDazedState();

    void ToDistractedState(Transform distractedPoint);

    void ToSuspiciousState();

    void ToSearchingState();

    void ToKOState();

    void ToWalkState();

}