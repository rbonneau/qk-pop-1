using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Debug = FFP.Debug;

public class StealthClock : MonoBehaviour
{
    /*!
        \file  StealthClock.cs
	    \brief  This class attaches to a clock face.
	
        In order for the minigame success/fail conditions to work properly,
	    the clock face y axis must be inverted so the y axis is pointing down
        (y rotation = 180, z rotation = 180).
	*/

    //reference to the player
    Transform player;

    //distance from player to search for enemies
	public float searchSize = 20f;
    

    //reference to the AIManager
	private AIManager aiMan;

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

    //number of successes necessary for a win
    private int easySuccess = 3;
    private int mediumSuccess = 3;
    private int hardSuccess = 3;
    private int hellSuccess = 3;

    //number of failures necessary for a loss
    private int easyFail = 1;
    private int mediumFail = 1;
    private int hardFail = 1;
    private int hellFail = 1;

    //speed of clock hand in rpms
    public int easySpeed = 20;
    private int mediumSpeed = 20;
    private int hardSpeed = 20;
    private int hellSpeed = 40;

    //layer for red/green lines for WatchCamera to display
    private int stealthLayer = 10;

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

    //reference to an empty gameObject will be the parent of the red/green lines
    private Transform lineParent;

	//length of lines that make up red/green areas
	private float _lineLength;
	//degrees in a circle
	private int _degrees = 360;

	//starting point of each line, above center of clock face
	public Vector3 startPos;

	//array of empty objects to hold line renderers, one object/line for each degree
	public GameObject[] lines;
	
    //hand of clock, child of this gameObject
	private GameObject _clockHand;

	void Awake()
	{

        //pause the player camera
        PoPCamera.State = Camera_2.CameraState.Pause;
//        QK_Character_Movement.Instance._moveState = QK_Character_Movement.CharacterState.Wait;

        //get the location of the player
		player = GameObject.FindGameObjectWithTag("Player").transform;

        if(player == null)
        {

            //end the game if a reference to the player can't be found
            endGame("StealthClock.Awake(): player == null.");

        }
        
        //reference to AIManager
		aiMan = AIManager.instance;

        //check for existance of AIManager
        if(aiMan == null)
        {

            //end the game if AIManager can't be found
//TESTING
            endGame("StealthClock.Awake(): aiMan == null.");
//END TESTING

        }

//TESTING
        //alert all enemies to player
        alertEnemies();
//END TESTING
        
        //get reference to empty gameObject parent of lines
        lineParent = transform.FindChild("lineManager").transform;
		
		//set difficulty
		setDifficulty();

//TESTING
		Debug.Log("player", "StealthClock Awake() complete");
//END TESTING

	}

	// Use this for initialization
	void Start()
	{

		//initialize line parameters
		_lineLength = transform.localScale.x / 3.2f;
		startPos = transform.position;
        //should set line y distance above clock face based on size of clock
		startPos = new Vector3(startPos.x, startPos.y + transform.lossyScale.y * 2.0f, startPos.z);
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

		//check to see if player is found
		hideCheck();

        //is button pressed
//TESTING
        if(Input.GetKeyDown("f"))
//        if(InputManager.input.isActionPressed())
//END TESTING
		{

			if(_startDegree < _endDegree)
			{
				//check for success/failure
				if((startDegree <= _clockHand.transform.localEulerAngles.y) && (_clockHand.transform.localEulerAngles.y <= endDegree))
				{
                    
//TESTING
                    Debug.Log("player", "SUCCESS: " + _clockHand.transform.localEulerAngles.y);
                    Debug.Log("player", "startDegree: " + startDegree);
                    Debug.Log("player", "endDegree: " + endDegree);
//END TESTING                    
                    
                    //increment success count
                    _currentSuccess++;

					//check to see if zones need to be reset
					if(_currentSuccess < maxSuccess)
						{
                            
                            //reset the red and green zones
							setZones();

						}


                }
                else
				{

//TESTING
                    Debug.Log("player", "FAIL: " + _clockHand.transform.localEulerAngles.y);
                    Debug.Log("player", "startDegree: " + startDegree);
                    Debug.Log("player", "endDegree: " + endDegree);
//END  TESTING

                    //increment fail count
                    _currentFail++;

                    if(_currentFail < maxFail)
                    {

                        //reset the red and green zones
                        setZones();

                    }


				}

			}
			//endDegree <= startDegree
			else
			{
				if((_startDegree <= _clockHand.transform.localEulerAngles.y) || (_clockHand.transform.localEulerAngles.y <= endDegree))
				{

                    //increment success count
					_currentSuccess++;

					//check to see if zones need to be reset
					if(_currentSuccess < maxSuccess)
					{

                        //reset the zones
						setZones();

					}

				}
				else
				{

                    //increment fail count
					_currentFail++;

				}

			}

			//check for win condition
			if(_currentSuccess >= maxSuccess)
			{

                //enemies return to normal
                aiMan.resumePatrol();

                //deactivate lines
                deactivateLines();

                //deactivate self
                endGame("StealthClockUpdate(): minigame success.");

			}
			else if(_currentFail >= maxFail)
			{

                //alert enemies to player position
                alertEnemies();

                //deactivate lines
                deactivateLines();

                //deactivate self
                endGame("StealthClock.Update(): mini game fail.");

			}

		}

	}

    /*!
        \brief Creates a line renderer for each object in the lines array.

        Creates a line renderer for each object in the lines array. Each line starts
        in the the center of the clock face and a moves outward, one line for each 
        degree of the clock face. Line widths start at a point and expand as they
        move away from the center of the clock face to form a continuous circle.

        \return void
    */
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

            //set the parent object of the line gameObject
            lines[i].transform.SetParent(lineParent);

            //set layer to stealthMiniGame
            lines[i].layer = stealthLayer;

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


    /*! \brief Sets proper difficulty level based on enemies and resets the red and green zones.
        
        Just calls setDifficulty() and setColors().

        \return void
    */
	void setZones()
	{

		setDifficulty();
		setColors();

	}

	/*! \brief  Sets the minigame difficulty.
    
        Sets the location and size of green area, successes needed to win, fails allowed
        before losing, and speed of clock hand based on the number of guards presently
        searching for player.

        Calls getEndDegree(int randomStart, int range)

        \return void
    */
	void setDifficulty()
	{

        //current count of guards searching for player
        int tempGuards = 0;

        //check for existence of enemies
		if(aiMan.AiChildren == null)
		{

			//exit minigame
//TESTING
			endGame("StealthClock.SetDifficulty(): aiMan.AIChildren == null.");
//END TESTING

		}
		else
		{

            //call AI manager for suspicious guards
            tempGuards = aiMan.checkChasing();

            //check for guards looking for player
            if (tempGuards < 1)
            {

                //exit minigame
//TESTING
                endGame("StealthClock.SetDifficulty(): no AI chasing");
//END TESTING
            }

		}

//TESTING
//AI
//        _numberOfGuards = tempGuards;
		_numberOfGuards = 1;
//END TESTING

		//choose random angle to start the green zone
		_startDegree = Random.Range(0, 360);

		//1 guard
		if(_numberOfGuards < 2)
		{

			_endDegree = getEndDegree(_startDegree, _easySize);
			maxSuccess = easySuccess;
			maxFail = easyFail;
			_clockSpeed = easySpeed;

		}
		//2-3 guards
		else if(_numberOfGuards < 4)
		{

			_endDegree = getEndDegree(_startDegree, _mediumSize);
			maxSuccess = mediumSuccess;
			maxFail = mediumFail;
			_clockSpeed = mediumSpeed;

		}
		//4-9 guards
		else if(_numberOfGuards < 10)
		{

			_endDegree = getEndDegree(_startDegree, _hardSize);
			maxSuccess = hardSuccess;
			maxFail = hardFail;
			_clockSpeed = hardSpeed;

		}
		//10 or more guards
		else
		{

			_endDegree = getEndDegree(_startDegree, _hellSize);
			maxSuccess = hellSuccess;
			maxFail = hellFail;
			_clockSpeed = hellSpeed;

		}

	}

    /*!
	    \brief  Calculates the end degree of the green zone.
        
	    \param randomStart the start degree of the green area 0-359
        \param range the size of the green area in degrees

        Called by setDifficulty()

        /return int
	*/
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

			//check for same start and end degrees
			if(randomStart == end)
			{

				//just set to easy range
				_startDegree = 0;
				end = _startDegree + _easySize;
				
				Debug.Log("player", "StealthClock.getEndDegree: start == end.");

			}

		}
		
		return end;

	}

    /*!
	    \brief Sets the colors for each line renderer that make up the red and green zones.
	    
        Cycles through each object in the lines array and sets its line renderer to the proper
        color based on _startDegree and _endDegree

        \return void
    */
    void setColors()
	{

		//set green and red with reduced opacity
		Color colorG = new Color(0.0f, 1.0f, 0.0f, 0.99f);
		Color colorR = new Color(1.0f, 0.0f, 0.0f, 0.99f);

		LineRenderer lRend;

		//set line colors accordingly
		//check start and end boundaries
		//green area fills counterclockwise from startDegree to endDegree
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

    /*!
        \brief  Deactivates all of the lines created by StealthClock.

        Iterates through the lines[] array and deactivates each game object.

        \return void
    */
    void deactivateLines()
    {

        for(int i = 0; i < lines.Length; i++)
        {

            if(lines[i] != null)
            {

                lines[i].SetActive(false);

            }
        }

    }

	/*!
		\brief Checks the aiManager to see if the player is hidden from enemies.
		
		Ends the minigame if the player is not in a hiding spot or if an AI known to the AIManager has found the player.
		
		\return void
	*/
	void hideCheck()
	{

        //check for player being in a hiding spot


		//if the player is found/not hidden
		if(aiMan.checkChasing() > 0)
		{

            //deactivate the miniGame
            endGame("StealthClock.hideCheck(): Player discovered by AI.");
			
		}
		else
		{

			//check for at least one enemy looking for player
			for(int i = 0; i < aiMan.AiChildren.Length; i++)
			{

				//if searching for player
				if(aiMan.AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "ChasingPlayer")
				{

                    endGame("StealthClock.hideCheck(): player found by AI.");

				}
				
			}

		}

	}

    /*!
        \brief Deactivates the minigame.

        Deactivates the miniGame parent gameObject and reactivates the main camera.

        \return void
    */
    void endGame()
    {
        
        //allow camera movement
        PoPCamera.instance.Reset();

        //deactivate miniGame gameObject
        transform.parent.gameObject.SetActive(false);
    }


    /*!
        \brief  Outputs a message to the debug log and deactivates the miniGame.

        \param message a message passed as a string to add to the debug log.

        Outputs the string \param message to the debug log and calls endGame().

        \return void
    */
    void endGame(string message)
    {

        //output debug message
        Debug.Log("player", message + " Exiting miniGame.");

        //deactivate miniGame gameObject
		endGame();
        
    }

    void alertEnemies()
    {

        if (aiMan.AiChildren != null)
        {

            for (int i = 0; i < aiMan.AiChildren.Length; i++)
            {

                //set AI to chase state
                aiMan.AiChildren[i].GetComponent<IEnemyState>().ToChaseState();

            }

        }

    }

}
