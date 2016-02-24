using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StealthClock : MonoBehaviour
{
	/*
	this class attaches to a clock face
	in order for the minigame success/fail conditions to work properly,
	the clock face y axis must be inverted so the y axis is pointing down (y rotation = 180, z rotation = 180)
	*/

	Transform player;
	public float searchSize = 20f;

	public List<Enemy> enemyList = new List<Enemy>();

	//start of green area in degrees from x axis
	private int _startDegree;
	//end of green area in degrees from x axis
	private int _endDegree;

	public float startDegree
	{
		get
		{
			return _startDegree;
		}
	}

	public float endDegree
	{
		get
		{
			return _endDegree;
		}
	}

	//clock hand speed multiplyer, used by StealthHand object
	public float _clockSpeed = 1f;

	public float clockSpeed
	{
		get
		{
			return _clockSpeed;
		}
	}

	//size of green area in degrees
	private int _easySize = 45;
	private int _mediumSize = 30;
	private int _hardSize = 20;
	private int _hellSize = 10;

	//success/fail counts
	private int _currentSuccess;
	private int _currentFail;

	//success/fail limits
	public int maxSuccess;
	public int maxFail;

	//width of red/green lines start is center of circle, end is edge of circle
	private float _startWidth;
	private float _endWidth;

	//number of guards near player
	private int _numberOfGuards;

	public int numberOfGuards
	{
		get
		{
			return _numberOfGuards;
		}
	}

	//length of lines that make up red/green areas
	private float _lineLength;
	//degrees in a circle
	private int _degrees = 360;

	//starting point of each line, above center of clock face
	public Vector3 startPos;

	//array of empty objects to hold line renderers, one object/line for each degree
	public GameObject[] lines;
	
	private GameObject _clockHand;

	void Awake()
	{

		//get the location of the player
		player = GameObject.FindGameObjectWithTag("Player").transform;

		//check for nearby enemies that are searching and not chasing player
//AI

//TEMP
//		_numberOfGuards = 1;
//TEMP
		
		Collider[] enemyColliders = Physics.OverlapSphere(player.position, searchSize);

		for(int i = 0; i < enemyColliders.Length; i++)
		{

			//check for enemy tag
//			if(enemyColliders[i].CompareTag("enemy"))
			{

				//if enemy is searching for player
//AI				if(enemyColliders[i].GetComponent<AIMain>().enemyCurrentState == enemy.searchingState)
//AI				{

					//add to list
//					enemyList.Add(enemyColliders[i].GetComponent<Enemy>());

//AI				}
			}
		}

		//get the number of guards actively searching for player
//		_numberOfGuards = enemyList.Count;
		
		//set difficulty
		setDifficulty();

		//set red and green zones
//		setColors();


//TESTING
		Debug.Log("StealthClock Awake() complete");
//END TESTING

	}

	// Use this for initialization
	void Start()
	{

		//initialize line parameters
		_lineLength = transform.localScale.x / 2f;
		startPos = transform.position;
		startPos = new Vector3(startPos.x, startPos.y + 0.1f, startPos.z);
		_startWidth = 0f;
		_endWidth = transform.localScale.x * Mathf.PI / 360f;

		//array of empty gameobjects to hold a single line renderer each
		lines = new GameObject[_degrees];

		//initialize lines for arc
		lineSetup();
		
		//set red and green zones accordingly
		setColors();

		//get reference to clockHand
		_clockHand = GetComponentInChildren<StealthHand>().gameObject;
		
	}

	// Update is called once per frame
	void Update()
	{

		//check for at least 1 suspicous enemy
//AI
		//check for no enemies chasing player
//AI

//should be if(Input.GetButtonDown("Action"))
		if(Input.GetButtonDown("Jump"))
		{

			if(_startDegree < _endDegree)
			{
				//check for success/failure
				if((startDegree <= _clockHand.transform.localEulerAngles.y) && (_clockHand.transform.localEulerAngles.y <= endDegree))
				{
					_currentSuccess++;

//TESTING
					print("SUCCESS: " + _clockHand.transform.localEulerAngles.y);
//					print("currentSuccess: " + _currentSuccess);
//					print("localEulerAngles: " + _clockHand.transform.localEulerAngles.y);
					print("startDegree: " + startDegree);
					print("endDegree: " + endDegree);
//END TESTING
					//check to see if zones need to be reset
					if(_currentSuccess < maxSuccess)
						{

							setZones();

						}

				}
				else
				{
					_currentFail++;

//TESTING
					print("FAIL: " + _clockHand.transform.localEulerAngles.y);
//					print("currentFail: " + _currentFail);
//					print("localEulerAngles: " + _clockHand.transform.localEulerAngles.y);
					print("startDegree: " + startDegree);
					print("endDegree: " + endDegree);
//END  TESTING

				}

			}
			//endDegree <= startDegree
			else
			{
				if((_startDegree <= _clockHand.transform.localEulerAngles.y) || (_clockHand.transform.localEulerAngles.y <= endDegree))
				{

					_currentSuccess++;

					//check to see if zones need to be reset
					if(_currentSuccess < maxSuccess)
					{

						setZones();

					}

				}
				else
				{

					_currentFail++;

				}

			}

			//check for win condition
			if(_currentSuccess >= maxSuccess)
			{

				//enemies return to normal
				foreach(Enemy guard in enemyList)
				{

//AI				guard.enemyCurrentState = enemy.patrolState;

				}

			}
			else if(_currentFail >= maxFail)
			{

				//alert enemies to player position
				foreach(Enemy guard in enemyList)
				{

//AI				guard.enemyCurrentState = clockFace.chaseState;

				}

			}

		}

	}
	
	void lineSetup()
	{

		//create a line renderer for each object in the array
		for(int i = 0; i < lines.Length; i++)
		{

			float lineX;
			float lineZ;
			Vector3 linePos;

			//create the gameObject to hold the LineRenderer
			lines[i] = new GameObject();

			//set the position of each game object with line renderers
			lines[i].transform.position = startPos;

			//add a line renderer to the empty game object
			LineRenderer lRend = lines[i].AddComponent<LineRenderer>();
			//set line widths
			lRend.SetWidth(_startWidth, _endWidth);
			//set line renderer material
			lRend.material = new Material(Shader.Find("Particles/Additive"));

			//only one line per game object
			lRend.SetVertexCount(2);
			
			//do math to point lines in correct directions, find end point for each line
			//initialize end point to current position of gameObject
			linePos = startPos;

			//find x and z position of end of line, y will remain the same
			lineX = startPos.x + _lineLength * Mathf.Cos(Mathf.Deg2Rad * i);
			linePos[0] = lineX;
			lineZ = startPos.z + _lineLength * Mathf.Sin(Mathf.Deg2Rad * i);
			linePos[2] = lineZ;

			//set start and end points for the line
			lRend.SetPosition(0, startPos);
			lRend.SetPosition(1, linePos);
			
		}
		
	}

	void setZones()
	{
		setDifficulty();
		setColors();
	}

	//sets the values for zone areas, successes needed to win, fails allowed before losing, and speed of clock hand
	//based on the number of guards presently searching for player
	void setDifficulty()
	{

		//call AI manager for suspicious guards
		_numberOfGuards = 1;

		//choose random angle to start the green zone
		_startDegree = Random.Range(0, 360);

		//1 guard
		if(_numberOfGuards < 2)
		{

			_endDegree = getEndDegree(_startDegree, _easySize);
			maxSuccess = 3;
			maxFail = 3;
			_clockSpeed = 5;

		}
		//2-3 guards
		else if(_numberOfGuards < 4)
		{

			_endDegree = getEndDegree(_startDegree, _mediumSize);
			maxSuccess = 3;
			maxFail = 2;
			_clockSpeed = 10;

		}
		//4-9 guards
		else if(_numberOfGuards < 10)
		{

			_endDegree = getEndDegree(_startDegree, _hardSize);
			maxSuccess = 3;
			maxFail = 1;
			_clockSpeed = 20;

		}
		//10 or more guards
		else
		{

			_endDegree = getEndDegree(_startDegree, _hellSize);
			maxSuccess = 3;
			maxFail = 1;
			_clockSpeed = 40;

		}

	}

	//called by setDifficulty()
	//takes 2 integers: the start degree of the green area 0-359 and the size of the green area in degrees
	int getEndDegree(int randomStart, int range)
	{

		int end;

		//check for range
		if(randomStart < 0)
		{

			randomStart = 0;

		}

		//find end degree
		end = randomStart + range;

		//check for upper bound
		while(end > 360)
		{

			//move end within bounds
			end = end - 360;

			//check for 
			if(randomStart == end)
			{

				//just set to easy range
				_startDegree = 0;
				end = _startDegree + _easySize;
				
				Debug.Log("StealthClock.getEndDegree: start == end");

			}

		}
		
		return end;

	}

	//sets the colors for each line renderer that make up the red/green zones
	void setColors()
	{

		//set green and red with reduced opacity
		Color colorG = new Color(0.0f, 1.0f, 0.0f, 0.9f);
		Color colorR = new Color(1.0f, 0.0f, 0.0f, 0.9f);

		LineRenderer lRend;

		//set line colors accordingly
		//check start and end boundaries
		//green area fills upward from startDegree to endDegree
		for(int i = 0; i < lines.Length; i++)
		{

			lRend = lines[i].GetComponent<LineRenderer>();

			if(_startDegree < _endDegree)
			{

				if((_startDegree <= i) && (i <= _endDegree))
				{

					lRend.SetColors(colorG, colorG);
				}
				else
				{

					lRend.SetColors(colorR, colorR);

				}

			}
			else
			{
				if((_startDegree <= i) || (i <= _endDegree))
				{

					lRend.SetColors(colorR, colorG);

				}
				else
				{

					lRend.SetColors(Color.clear, colorR);

				}

			}

		}

	}

}
