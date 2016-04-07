using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]	                //!<Automaticly Make a navMeshAgent on this game object when this script is applied
[RequireComponent(typeof(Rigidbody))]                       //!<Automaticaly make a rigid body on this object
[EventVisible]
/*
This Script is attached to Each AI. all of the states an AI can be in are linked to this script.
Update and OnTrigger in this script will run the updateState/OnTriggerstate inside the script of its current state. Ex: If the AI's currentState is set to Patrol State, on update it will run the updateState in the patrolState script.


*/
[System.Serializable]

public class StatePatternEnemy : MonoBehaviour
{
    public float moveSpeed;                            //!<float to adjust the movement speed of the AI
    public float patrolSpeed;
    public float chaseSpeed;                          //!<float to  adjust the chasing speed of the AI
    public float searchingTurnSpeed = 180f;                 //!<float to adjust how fast the AI turns when in the searching state
    public float searchingDuration = 4f;                    //!<float to adjust how long the AI stays in the searching state
    public float sightRange = 20f;                          //!<float to adjust how far the AI can see
    public float sightAngle = 20f;                          //!<float to adjust the sight angle of the AI
    public float suspiciousCheckRange = 10f;
    public MeshRenderer meshRendererFlag;                   //!<Used only for showing the gradual change of the AI state
    public int current_preset = 0;                          //!<preset setting for the AI editor draw from
    public bool customType = false;                         //!<bool used in the ai editor for custom AI types to be created
    public bool seesTarget;                                 //!<bool to determine if the AI can see the player or not
    public GameObject player;                               //!<stores the player so it can be referenced easily

    //Path Variables
    public List<GameObject> Pathways;                       //!<List of the paths the AI uses. Paths are gameobjects with a list of vectors 3s that the AI uses to as waypoints. See AI's editor for paths.
    public List<int> PathType;                              //!<Specifies the Type of path the AI will use. To point, Back and forth, Loop, On guard.
    public List<int> nofLoops;                              //!<Number of loops the AI will do on a certain path. Each path has its own nofLoops.
    public List<bool> infinite;                             //!<Sets the number of loops to infinite for an AI that patrols indefinitely.
    private bool looping;                                   //!<Bool to set the AI to the loop type of path.
    public int LoopCount = 1;                               //!<Int to track the number of times looped.
    public bool BacknForth;                                 //!<Bool to set the AI to the back and forth type of path
    public bool back = false;                               //!<Bool to determine if it is going back on its path during the back and forth loop.
    public int PathwayListLength;                           //!<Int to store the number of checkpoints on a path.
    public GameObject Path;                                 //!<Stores the current path of the AI so it's checkpoints can be accessed.
    public int PathwayCount = 0;                            //!<Int for tracking which path the AI is on
    public int CheckpointCount = 0;                         //! Int for tracking which checkpoint the AI is on
    public bool enemy;                                      
    public Vector3 navPoint = new Vector3(0, 0, 0);         //!<Contains the point to move in the navmesh
    public Transform noiseLoc;
    public float _minAngle;
    public float _maxAngle;
    public float _turnSpeed;
    public int _searchCount;


    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public IEnemyState currentState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public GuardState guardState;
    [HideInInspector] public DazedState dazedState;
    [HideInInspector] public DistractedState distractedState;
    [HideInInspector] public SearchingState searchingState;
    [HideInInspector] public SuspiciousState suspiciousState;
    [HideInInspector] public KOState koState;
    [HideInInspector] public WalkState walkState;
    [HideInInspector] public PointSearchState pointSearchState;
    [HideInInspector] public NavMeshAgent navMeshAgent;

    private void Awake()
    {
        chaseState = new ChaseState(this);
        patrolState = new PatrolState(this);
        guardState = new GuardState(this);
        dazedState = new DazedState(this);
        distractedState = new DistractedState(this);
        searchingState = new SearchingState(this);
        suspiciousState = new SuspiciousState(this);
        koState = new KOState(this);
        walkState = new WalkState(this);
        pointSearchState = new PointSearchState(this);

        navMeshAgent = GetComponent<NavMeshAgent>();
    }



    // Use this for initialization
    void Start ()
    {
        /*
        Set default state of the AI (walking, patroling, guarding)
        set current state to default state
        */
        player = GameObject.FindGameObjectWithTag("Player");
        Path = Pathways[0];
        AIPath CheckpointScript = Path.GetComponent<AIPath>();
        navPoint = CheckpointScript.getPoints()[0];
        currentState = patrolState;
        moveSpeed = patrolSpeed; //sets the current state
        //THIS might not be needed maybe idk lets find out navPoint = CheckpointScript.getPoints()[CheckpointCount];
    }
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log(currentState);
        navMeshAgent.speed = moveSpeed;
        currentState.UpdateState(); //calls the update of the current state
        if (currentState == distractedState)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
            {
                currentState = searchingState;
            }
        }
	}

    void OnTriggerEnter(Collider col)
    {
        /*
        if the player is not sneaking or undetectable in some form
            OnTriggerEnter
            (will AI detect player if they get in a certain range in any direction?)
            (should this ^ be placed in each individual script?)
                (wont be needed for some states such as dazed or ko)

        */

            currentState.OnTriggerEnter(col); // calls OnTriggerEnter in the current State
    }
}
