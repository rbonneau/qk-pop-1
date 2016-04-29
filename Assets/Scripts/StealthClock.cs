using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Debug = FFP.Debug;

public class StealthClock : MonoBehaviour
{
    /*!
        \file  StealthClock.cs
	    \brief  Runs the mini-game.
        
        This class attaches to the clockFace.
        In order for the minigame success/fail conditions to work properly,
	    the clock face y axis must be inverted so the y axis is pointing down
        (y rotation = 180, z rotation = 180).
	*/

    //reference to the player
    Transform player;

    //distance from player to search for enemies
//	public float searchSize = 20f;          /*!<distance from player to */
    
	//reference to the StealthGameManager
	private StealthGameManager stealthMan;

    //reference to the AIManager
	private AIManager aiMan;

	//true if the game has been won or lost
	private bool _gameOver;

    public bool gameOver                    /*!<returns true if the mini-game has ended*/
    {
        get
        {
            return _gameOver;
        }
    }

    //true if player wins mini-game
    private bool _win;

    public bool win                         /*!<returns true if player wins the mini-game*/
    {
        get
        {
            return _win;
        }
    }

    //true if player loses mini-game
    private bool _fail;

	public bool fail                        /*!<returns true if player loses the mini-game*/
    {
		get
		{
			return _fail;
		}
	}

	//start of green area in degrees from x axis
	private int _startDegree;

    public float startDegree                /*!<start degree of the green zone moving counterclockwise across clock face*/
    {
        get
        {
            return _startDegree;
        }
    }

    //end of green area in degrees from x axis
    private int _endDegree;

	public float endDegree                  /*!<end degree of the green zone moving counterclockwise across clock face*/
    {
		get
		{
			return _endDegree;
		}
	}

	//clock hand speed multiplyer, used by StealthHand object
	public float _clockSpeed = 1f;          /*!<clock hand speed multiplier, this should be private*/

    public float clockSpeed                 /*!<clock hand speed multiplier*/
    {
		get
		{
			return _clockSpeed;
		}
	}
    
	//success/fail counts
	private int _currentSuccess;
	private int _currentFail;

	//success/fail limits
	private int _maxSuccess;
	private int _maxFail;

    //layer for red/green lines for WatchCamera to display
    private int stealthLayer = 10;

	//width of red/green lines start is center of circle, end is edge of circle
	private float _startWidth;
	private float _endWidth;

    //reference to an empty gameObject will be the parent of the red/green lines
    private Transform lineParent;

	//length of lines that make up red/green areas
	private float _lineLength;
	//degrees in a circle
	private int _degrees = 360;

	//starting point of each line, above center of clock face
	public Vector3 startPos;                /*!<starting point of each line, should be the center of the clock face*/

    //array of empty objects to hold line renderers, one object/line for each degree
    public GameObject[] lines;              /*!<array of empty objects to hold line renderers, one object/line for each degree*/

    //hand of clock, child of this gameObject
    private GameObject _clockHand;

	void Awake()
	{

        //get the location of the player
		player = GameObject.FindGameObjectWithTag("Player").transform;

        if(player == null)
        {

            //end the game if a reference to the player can't be found
            endGame("StealthClock.Awake(): player == null.");

        }
        
		//get reference to StealthGameManager
		stealthMan = StealthGameManager.Instance;

		//check for existance of StealthGameManager
		if(stealthMan == null)
		{

			//end the game if StealthGameManager can't be found
			endGame("StealthClock.Awake(): stealthMan == null");

		}

        //get reference to AIManager
		aiMan = AIManager.instance;

        //check for existance of AIManager
        if(aiMan == null)
        {

            //end the game if AIManager can't be found
            endGame("StealthClock.Awake(): aiMan == null.");

        }
        
        //get reference to empty gameObject parent of lines
        lineParent = transform.FindChild("lineManager").transform;

        //array of empty gameobjects to hold a single line renderer each
        lines = new GameObject[_degrees];

    }

	void OnEnable()
	{
        
        //initialize win/loss flags
        _gameOver = false;
		_win = false;
		_fail = false;
        //set current game stats to zero
		_currentSuccess = 0;
		_currentFail = 0;

        //initialize line parameters
        _lineLength = transform.localScale.x / 3.2f;
		startPos = transform.position;
        //should set line y distance above clock face based on size of clock
		startPos = new Vector3(startPos.x, startPos.y + transform.lossyScale.y * 2.0f, startPos.z);
		_startWidth = 0f;
		_endWidth = 0.63f * transform.localScale.x * Mathf.PI / 360f;

		//array of empty gameobjects to hold a single line renderer each
//		lines = new GameObject[_degrees];

		//initialize lines for arc
		lineSetup();

		//get reference to clockHand
		_clockHand = GetComponentInChildren<StealthHand>().gameObject;

        //set red and green zones accordingly
        setZones();

    }


    void OnDisable()
    {

        deactivateLines();

    }

    // Update is called once per frame
    void Update()
	{
        
        //is button pressed while game is not paused
//TESTING
        if(Input.GetKeyDown("f") && !GameHUD.Instance.pauseMenu.activeInHierarchy)
//        if(InputManager.input.isActionPressed() && !GameHUD.Instance.pauseMenu.activeInHierarchy)
//END TESTING
		{

			if(_startDegree < _endDegree)
			{
				//check for success/failure
				if((startDegree <= _clockHand.transform.localEulerAngles.y) && (_clockHand.transform.localEulerAngles.y <= endDegree))
				{
                    
//TESTING
                    Debug.Log("player", "SUCCESS: " + _clockHand.transform.localEulerAngles.y);
                    Debug.Log("player", "startDegree: " + _startDegree);
                    Debug.Log("player", "endDegree: " + endDegree);
//END TESTING                    
                    
                    //increment success count
                    _currentSuccess++;

					//check to see if zones need to be reset
					if(_currentSuccess < _maxSuccess)
						{
                            
                            //reset the red and green zones
							resetZones();

						}

                }
                else
				{

//TESTING
                    Debug.Log("player", "FAIL: " + _clockHand.transform.localEulerAngles.y);
                    Debug.Log("player", "startDegree: " + _startDegree);
                    Debug.Log("player", "endDegree: " + endDegree);
//END  TESTING

                    //increment fail count
                    _currentFail++;

                    if(_currentFail < _maxFail)
                    {

                        //reset the red and green zones
                        resetZones();

                    }


				}

			}
			//_endDegree <= _startDegree
			else
			{
				if((_startDegree <= _clockHand.transform.localEulerAngles.y) || (_clockHand.transform.localEulerAngles.y <= endDegree))
				{
                    
//TESTING
                    Debug.Log("player", "SUCCESS: " + _clockHand.transform.localEulerAngles.y);
                    Debug.Log("player", "startDegree: " + _startDegree);
                    Debug.Log("player", "endDegree: " + endDegree);
//END TESTING                    

                    //increment success count
                    _currentSuccess++;

					//check to see if zones need to be reset
					if(_currentSuccess < _maxSuccess)
					{

                        //reset the zones
						resetZones();

					}

				}
				else
				{


//TESTING
                    Debug.Log("player", "FAIL: " + _clockHand.transform.localEulerAngles.y);
                    Debug.Log("player", "startDegree: " + _startDegree);
                    Debug.Log("player", "endDegree: " + endDegree);
//END  TESTING

                    //increment fail count
                    _currentFail++;

				}

			}

			//check for win condition
			if(_currentSuccess >= _maxSuccess)
			{

                //enemies return to normal
                aiMan.resumePatrol();

                //deactivate lines
                deactivateLines();

				//set game win flag
				_win = true;

                //deactivate self
                endGame("StealthClockUpdate(): minigame success.");

			}
			else if(_currentFail >= _maxFail)
			{

                //deactivate lines
                deactivateLines();

				//set game loss flag
				_fail = true;

                //deactivate self
                endGame("StealthClock.Update(): mini game fail.");

			}

//TESTING
            Debug.Log("player", "StealthClock._maxSuccess: " + _maxSuccess);
            Debug.Log("player", "StealthClock._maxFail: " + _maxFail);
//END TESTING

        }
//TESTING
//        Debug.Log("player", "Update(): completed");
//END TESTING

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

            //x and z coordinates for line ends
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
//TESTING
        Debug.Log("player", "StealthClock.lineSetup(): completed");
//END TESTING
    }


    /*! \brief Sets proper difficulty level based on enemies and resets the red and green zones.
        
        Just calls setDifficulty() and setColors().

        \return void
    */
    void setZones()
	{

		setDifficulty();
		setColors();
//TESTING
        Debug.Log("player", "StealthClock.setZones(): completed");
//END TESTING

    }

    void resetZones()
    {

        deactivateLines();
        setZones();

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
        
		//choose random angle to start the green zone
		_startDegree = Random.Range(0, (_degrees - 1));


        //check for range
        if (_startDegree < 0)
        {

            _startDegree = 0;

        }

        //find end degree
        _endDegree = _startDegree + stealthMan.areaSize;

//TESTING
        Debug.Log("player", "StealthClock.setDifficulty():  _endDegree (" + _endDegree +") = _startDegree (" + _startDegree + ") + stealthMan.areaSize (" + stealthMan.areaSize +")");
//END TESTING
        
        //adjust for upper bound
        _endDegree %= _degrees;

//TESTING
        Debug.Log("player", "StealthClock.setDifficulty(): _endDegree (" + _endDegree + ") %= _degrees (" + _degrees + ")");
//END TESTING
        
        //check for same start and end degrees
        if (_startDegree == _endDegree)
        {

//TESTING
            Debug.Log("player", "StealthClock.setDifficulty(): start == end.");
            Debug.Log("player", "StealthClock.setDifficulty(): _startDegree: " + _startDegree);
            Debug.Log("player", "StealthClock.setDifficulty(): _endDegree: " + _endDegree);
//END TESTING

            //just set to an easy range
            _startDegree = 0;
            _endDegree = _startDegree + 45;

        }

        //set game variables
//        _endDegree = getEndDegree(_startDegree, stealthMan.areaSize);
		_maxSuccess = stealthMan.maxSuccesses;
		_maxFail = stealthMan.maxFails;
		_clockSpeed = stealthMan.handSpeed;

	}

    /*!
	    \brief  Calculates the end degree of the green zone.
        
	    \param randomStart the start degree of the green area 0-359
        \param range the size of the green area in degrees

        Called by setDifficulty()

        /return int
	*//*
    int getEndDegree(int randomStart, int range)
	{

        //the last degree that will be green, this will be returned
		int end;

		//check for range
		if(randomStart < 0)
		{

			randomStart = 0;

		}

		//find end degree
		end = randomStart + range;

		//adjust for upper bound
		end %= _degrees;

		//check for same start and end degrees
		if(randomStart == end)
		{

			//just set to an easy range
			_startDegree = 0;
			end = _startDegree + 45;
				
			Debug.Log("player", "StealthClock.getEndDegree: start == end.");

		}
		
		return end;

	}
*/
    /*!
	    \brief Sets the colors for each line renderer that make up the red and green zones.
	    
        Cycles through each object in the lines array and sets its line renderer to the proper
        color based on _startDegree and _endDegree. Currently red lines are appearing yellow
        in the Unity editor for some reason.

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
//		for(int i = 0; i < lines.Length; i++)
//		{

//			lRend = lines[i].GetComponent<LineRenderer>();

		if(_startDegree < _endDegree)
		{
            for (int i = 0; i < lines.Length; i++)
            {

                lRend = lines[i].GetComponent<LineRenderer>();

                if ((_startDegree <= i) && (i <= _endDegree))
                {

                    //set to green
                    lRend.SetColors(colorG, colorG);
//                    lRend.material.color = colorG;
                }
                else
                {

                    //set to red
                    lRend.SetColors(colorR, colorR);
//                    lRend.material.color = colorR;
                }

                lines[i].SetActive(true);

            }

		}
		else
		{
            for (int i = 0; i < lines.Length; i++)
            {

                lRend = lines[i].GetComponent<LineRenderer>();

                if ((_startDegree <= i) || (i <= _endDegree))
                {

                    //set to green
                    lRend.SetColors(colorG, colorG);
//                    lRend.material.color = colorG;
                }
                else
                {

                    //set to red
                    lRend.SetColors(colorR, colorR);
//                    lRend.material.color = colorR;
                }

                lines[i].SetActive(true);
            }

		}

//		}
//TESTING
        Debug.Log("player", "StealthClock.setColors(): completed");
//END TESTING
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
        \brief Deactivates the minigame.

        Deactivates the miniGame parent gameObject and reactivates the main camera.

        \return void
    */
    void endGame()
    {
        
        //allow camera movement
        PoPCamera.instance.Reset();

		//
		_gameOver = true;

        //deactivate miniGame gameObject
//        transform.parent.gameObject.SetActive(false);

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

}

/*!
    \brief Checks the aiManager to see if the player is hidden from enemies.

    Ends the minigame if the player is not in a hiding spot or if an AI known to the AIManager has found the player.

    \return void
*/
/*	void hideCheck()
	{

        //check for player being in a hiding spot
        if(!QK_Character_Movement.Instance.isHidden)
        {
            endGame("StealthClock.hideCheck(): Player not hidden.");
        }
//TESTING
        Debug.Log("player", "StealthClock.hideCheck: player is hidden");
//END TESTING
        //if the player is found/not hidden
        if (aiMan.checkChasing() > 0)
		{
//TESTING
            Debug.Log("player", "StealthClock.hideCheck(): at least one chasing");
//END TESTING
            //deactivate the miniGame
            endGame("StealthClock.hideCheck(): Player discovered by AI.");
			
		}
		else
		{
//TESTING
            Debug.Log("player", "StealthClock.hideCheck(): player hidden, no chasing");
//END TESTING
            //check for at least one enemy looking for player
            for (int i = 0; i < aiMan.AiChildren.Length; i++)
			{

				//if searching for player
				if(aiMan.AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "ChasingPlayer")
				{

                    endGame("StealthClock.hideCheck(): player found by AI.");

				}
				
			}

        }
//TESTING
        Debug.Log("player", "StealthClock.hideCheck(): completed");
//END TESTING

    }
*/
