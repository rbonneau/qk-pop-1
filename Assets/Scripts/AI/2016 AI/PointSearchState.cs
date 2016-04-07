using UnityEngine;
using System.Collections;

public class PointSearchState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    public Vector3 currentAngle;
    private Vector3 firstAngle;
    private Vector3 secondAngle;
    private int searchCase = 0;
    private int loopNumber = 0;
    private Vector3 offsetAngle;

    public int searchLoops = 2;
    public GameObject spotToSearch;

    public PointSearchState(StatePatternEnemy statePatternEnemy)
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
        searchCase = 0;
        loopNumber = 0;
        enemy.moveSpeed = enemy.patrolSpeed;
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
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
    }

    public void ToSuspiciousState()
    {
        searchCase = 0;
        loopNumber = 0;
        enemy.seesTarget = false;
        enemy.currentState = enemy.suspiciousState;
    }

    public void ToKOState()
    {

    }

    public void ToWalkState()
    {

    }

    public void ToPointSearchState(float minAngle, float maxAngle, float turnSpeed, int searchCount)
    {
        Debug.Log("Cant transition into itself");

    }

    private void Look()
    {
        RaycastHit hit;
        if (Vector3.Angle(enemy.player.transform.position - enemy.transform.position, enemy.transform.forward) < enemy.sightAngle)
        {
            if (Physics.Raycast(enemy.transform.position, enemy.player.transform.position - enemy.transform.position, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
            //check if player is hidden - if not hidden run below else do nothing (player is not seen)
            {
                enemy.chaseTarget = hit.transform;
                //if enemy is alert type
                //ToChaseState();
                ToSuspiciousState();
            }
        }
    }

    private void Search()
    {
        firstAngle = new Vector3(0, enemy._minAngle, 0);
        secondAngle = new Vector3(0, enemy._maxAngle, 0);
        enemy.meshRendererFlag.material.color = Color.blue;
        enemy.navMeshAgent.Stop();
        if (loopNumber <= enemy._searchCount)
        {
            switch (searchCase)
            {
                case 0:
                    {
                        /*
                        //THIS METHOD ROTATES AT A CONSTANT SPEED HOWEVER CANNOT PROPERLY DETERMINE NEGATIVE ANGLES OR ANYTHING PAST 360
                        currentAngle = firstAngle * Time.deltaTime * enemy._turnSpeed;
                        enemy.transform.Rotate(currentAngle);
                        if (Mathf.Abs(enemy.transform.rotation.y) >= Mathf.Abs(firstAngle.y - 2) /100 )
                        {
                            searchCase++;
                        }
                        */

                        //THIS METHOD PROPERLY READS ANY ANGLE POSITVE OR NEGATIVE AS WELL AS ANYTHING ABOVE 360. hOWEVER IT SLOWS DOWN NEAR THE END OF THE ROTATION SIGNIFICANLY
                        //POSSIBLE SOLUTION IS TO TAKE THE INPUT ANGLE, INCREASE IT BY X AMOUNT THEN CHECK FOR THE ORIGINAL VALUE
                        //Problem occurs when determing whether the angle is coming from clockwise/counter. 
                        currentAngle = Vector3.Lerp(currentAngle, firstAngle, Time.deltaTime * enemy._turnSpeed);
                        
                        /*
                        //moving clockwise in positive
                        if (currentAngle.y < firstAngle.y && firstAngle.y > 0)
                        {
                            // add offset to the angle
                            Debug.Log("Clockwise in positive");
                        }
                        //moving clockwise in negative
                        if ( currentAngle.y < firstAngle.y && firstAngle.y < 0)
                        {
                            // subtract offset from angle
                            Debug.Log("Clockwise in negative");
                        }

                        //moving counter in positive
                        if (currentAngle.y > firstAngle.y && firstAngle.y > 0)
                        {
                            Debug.Log("Counter in positive");
                        }

                        //moving counter in negative
                        if (currentAngle.y > firstAngle.y && firstAngle.y < 0)
                            {
                            Debug.Log("Counter in negative");
                        }
                        */
                        if (Mathf.Abs(currentAngle.y) >= Mathf.Abs(firstAngle.y) - 3 && (Mathf.Abs(currentAngle.y) <= Mathf.Abs(firstAngle.y) + 3))
                        {
                            searchCase++;
                        }
                        enemy.transform.eulerAngles = currentAngle;
                        
                        break;
                    }
                case 1:
                    {
                        /*
                        currentAngle = secondAngle * Time.deltaTime * enemy._turnSpeed;
                        enemy.transform.Rotate(currentAngle);
                        if (Mathf.Abs(enemy.transform.rotation.y) >= Mathf.Abs(secondAngle.y - 2) / 100)
                        {
                            searchCase++;
                            searchCase = 0;
                        }
                        */
                        
                        currentAngle = Vector3.Lerp(currentAngle, secondAngle, Time.deltaTime * enemy._turnSpeed);
                        if (Mathf.Abs(currentAngle.y) >= Mathf.Abs(secondAngle.y) - 3 && (Mathf.Abs(currentAngle.y) <= Mathf.Abs(secondAngle.y) + 3))
                        {
                            loopNumber++;
                            searchCase = 0;
                        }
                        enemy.transform.eulerAngles = currentAngle;
                        
                        break;
                    }
            }
        }
        else
        {
            ToPatrolState();
        }
    }


    //go from current angle to first angle
    //go from first angle to second angle
    //if there is a loop repeat for that number of times
}
