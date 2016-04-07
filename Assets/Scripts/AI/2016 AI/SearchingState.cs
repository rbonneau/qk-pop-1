using UnityEngine;
using System.Collections;

public class SearchingState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    private float searchTimer;
    private float distanceTo = 0f;
    public GameObject spotToSearch;

    public SearchingState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
        Search();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent("PlayNoise") != null)
        {
            enemy.noiseLoc = col.gameObject.transform;
            enemy.currentState = enemy.distractedState;
        }
    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        searchTimer = 0f;
        enemy.moveSpeed = enemy.patrolSpeed;
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        searchTimer = 0f;
        enemy.moveSpeed = enemy.chaseSpeed;
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
        Debug.Log("Cant transition into itself");
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
        if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {

            enemy.chaseTarget = hit.transform;
            ToChaseState();
        }
    }

    private void Search()
    {
        //when the AI can no longer find the player, they will spin around checking surroundings for the player
        enemy.meshRendererFlag.material.color = Color.yellow;
        enemy.navMeshAgent.Stop();
        enemy.transform.Rotate(0, enemy.searchingTurnSpeed * Time.deltaTime, 0);
        searchTimer += Time.deltaTime;
        if (searchTimer >= enemy.searchingDuration)
        {
            /*
            if (enemy.player.GetComponent<Hiding>().Hidden == true)
            {
                Collider[] hidingSpots = Physics.OverlapSphere(enemy.transform.position, enemy.sightRange);
                foreach (Collider hidingSpot in hidingSpots)
                {
                    if (hidingSpot.gameObject.tag == "Hay" && Vector3.Distance(enemy.transform.position, hidingSpot.transform.position) < distanceTo)
                    {
                        distanceTo = Vector3.Distance(enemy.transform.position, hidingSpot.transform.position);
                        spotToSearch = hidingSpot.gameObject;
                    }
                }
                if (spotToSearch != null)
                {
                    enemy.chaseTarget = spotToSearch.transform;
                }
                
            }
            */
            //ToDefaultState();
            ToPatrolState();
        }

    }

}
