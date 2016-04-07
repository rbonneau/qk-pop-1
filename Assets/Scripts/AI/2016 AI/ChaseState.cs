﻿using UnityEngine;
using System.Collections;

public class ChaseState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    private float chaseTimer;

    public ChaseState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
        Chase();
    }

    public void OnTriggerEnter(Collider col)
    {

    }

    public void ToPatrolState()
    {

    }

    public void ToChaseState()
    {
        Debug.Log("Cant transition into itself");
    }

    public void ToGuardState()
    {

    }

    public void ToDazedState()
    {
        enemy.currentState = enemy.dazedState;
        chaseTimer = 0f;
        enemy.moveSpeed = 0f;
    }

    public void ToDistractedState(Transform distractedPoint)
    {

    }

    public void ToSearchingState()
    {
        enemy.currentState = enemy.searchingState;
        chaseTimer = 0f;
        enemy.moveSpeed = enemy.patrolSpeed;
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

    public void ToPointSearchState(float minAngle, float maxAngle, float turnSpeed, int searchCount)
    {

    }

    private void Look()
    {
        RaycastHit hit;
        //Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;

        if (Vector3.Angle(enemy.chaseTarget.position - enemy.transform.position, enemy.transform.forward) < enemy.sightAngle)
        {
            if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
            {
                enemy.seesTarget = true;
                enemy.chaseTarget = hit.transform;
            }
            else
            {
                enemy.seesTarget = false;
            }
        }
        else
        {
            enemy.seesTarget = false;
            ToSearchingState();
        };
    }

    private void Chase()
    {

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
        enemy.meshRendererFlag.material.color = Color.red;
        enemy.navMeshAgent.destination = enemy.chaseTarget.position;
        enemy.navMeshAgent.Resume ();
    }
}
