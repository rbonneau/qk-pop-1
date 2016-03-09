using UnityEngine;
using System.Collections;
using Debug = FFP.Debug;

public class StealthGameManager : MonoBehaviour
{

	/*!
		\file StealthGameManager.cs
		\brief This class attaches to an empty gameObject that is the parent of the mini-game objects

		This class manages the stealth mini-game gameObjects. It monitors game conditions and activates
		and deactivates the mini-game components accordingly.
	*/

	//singleton instance
	private static StealthGameManager instance;

	//make this a singleton
	public static StealthGameManager Instance
	{
		get
		{
			return instance ?? (instance = GameObject.FindObjectOfType<StealthGameManager>());
		}
	}

	//reference to the stealthClock
	private StealthClock clock;

	//reference to the AIManager
	private AIManager aiMan;

    //reference to player
    Transform player;

    //maximum number of guards for difficulty level
    public int easyGuards = 2;
    public int mediumGuards = 4;
    public int hardGuards = 10;

    //size of green area in degrees
    public int easySize = 45;
    public int mediumSize = 30;
    public int hardSize = 20;
    public int hellSize = 10;

    //number of successes necessary for a win
    public int easySuccess = 3;
    public int mediumSuccess = 3;
    public int hardSuccess = 3;
    public int hellSuccess = 3;

    //number of failures necessary for a loss
    public int easyFail = 1;
    public int mediumFail = 1;
    public int hardFail = 1;
    public int hellFail = 1;

    //speed of clock hand in rpms
    public int easySpeed = 20;
    public int mediumSpeed = 20;
    public int hardSpeed = 20;
    public int hellSpeed = 40;

    //settings that the mini-game will use
    public int areaSize;
    public int maxSuccesses;
    public int fails;
    public int handSpeed;

    private int _numberOfGuards;

    // Use this for initialization
    void Start()
	{

		//get reference to the StealthClock
		clock = GetComponentInChildren<StealthClock>();

		//get reference to AIManager
		aiMan = AIManager.instance;

        //reference to player
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }
	
	// Update is called once per frame
	void Update()
	{
//TESTING
        Debug.Log("player", "got here");
        print("clock.gameOver " + clock.gameOver);
        print("aiMan.checkChasing() " + aiMan.checkChasing());
        print("QK_Character_Movement.Instance.isHidden " + QK_Character_Movement.Instance.isHidden);
//END TESTING
        //if the player won or lost the mini-game or is not hidden from ai
//		if(clock.gameOver || !aiMan.checkForPlayer())
        if(clock.gameOver || !QK_Character_Movement.Instance.isHidden)
		{
//TESTING
            Debug.Log("player", "got here");
//END TESTING
            //if the mini-game is running
            if (clock.isActiveAndEnabled)
			{

				//check for win
				if(clock.win)
				{

					//reset searching AI
					aiMan.resumePatrol();

				}
				//check for loss
				else if(clock.fail)
				{

                    //notify AI of player location
                    aiMan.resumeChase();


				}

				//deactivate mini-game
                foreach(Transform child in transform)
                {

                    //deactivate children
                    child.gameObject.SetActive(false);

                }

			}

		}
		//if player is being searched for and in a hiding spot
		else if((aiMan.checkChasing() > 0) && QK_Character_Movement.Instance.isHidden)
		{
//TESTING
            Debug.Log("player", "got here");
//END TESTING
            //if mini-game isn't running
            if (!clock.isActiveAndEnabled)
			{

                //calculate mini-game difficulty
                if(chooseDifficulty())
                {

                    //start mini-game
                    foreach(Transform child in transform)
                    {

                        //activate children
                        child.gameObject.SetActive(true);

                    }

                }
                else
                {

                    //output error
                    Debug.Log("player", "StealthGameManager.chooseDifficulty() == false");

                }

			}

		}
		//mini-game is running, no AI searching or player is found by AI or player is not in a hiding spot
		else if(clock.isActiveAndEnabled && (aiMan.checkChasing() < 1 || !aiMan.checkForPlayer() || !QK_Character_Movement.Instance.isHidden))
		{
//TESTING
            Debug.Log("player", "got here");
//END TESTING
            //deactivate mini-game
            transform.GetChild(0).gameObject.SetActive(false);

		}
        else
        {
//TESTING
            print("clock.isActiveAndEnabled = " + clock.isActiveAndEnabled);
            print("aiMan.checkChasing() = " + aiMan.checkChasing());
            print("!aiMan.checkForPlayer() = " + !aiMan.checkForPlayer());
            print("!QK_Character_Movement.Instance.isHidden " + !QK_Character_Movement.Instance.isHidden);
//END TESTING
        }

	}

    /*
        \brief  Chooses difficulty of the mini-game

        Checks for exitence of enemies actively searching for the player. Chooses the difficulty of the
        mini-game based on the number of searching enemies. If there are no enemies, nothing is set and
        returns false.

        \return bool, true if difficulty was successfully chosen, false otherwise
    */
    bool chooseDifficulty()
    {
//TESTING
        Debug.Log("player", "got here");
//END TESTING
        //current count of guards searching for player
        //        int tempGuards = 0;

        //check for existence of enemies
        if (aiMan.AiChildren == null)
        {
            
            //no searching guards, difficulty not set
            return false;

        }
        else
        {

            //call AI manager for suspicious guards
            _numberOfGuards = aiMan.checkChasing();

//TESTING
//AI
            //_numberOfGuards = 1;
//END TESTING

            //check for guards looking for player
            if (_numberOfGuards < 1)
            {

                //no guards searching for player
                Debug.Log("player", "StealthGameManager.ChooseDifficulty(): _numberOfGuards < 1");
                return false;

            }

            //set mini-game difficulty variables
            //easy
            if (_numberOfGuards < easyGuards)
            {

                //_endDegree = getEndDegree(_startDegree, _easySize);
                areaSize = easySize;
                maxSuccesses = easySuccess;
                fails = easyFail;
                handSpeed = easySpeed;

            }
            //medium
            else if (_numberOfGuards < mediumGuards)
            {

                //_endDegree = getEndDegree(_startDegree, _mediumSize);
                areaSize = mediumSize;
                maxSuccesses = mediumSuccess;
                fails = mediumFail;
                handSpeed = mediumSpeed;

            }
            //hard
            else if (_numberOfGuards < hardGuards)
            {

                //_endDegree = getEndDegree(_startDegree, _hardSize);
                areaSize = hardSize;
                maxSuccesses = hardSuccess;
                fails = hardFail;
                handSpeed = hardSpeed;

            }
            //hell
            else
            {

                //_endDegree = getEndDegree(_startDegree, _hellSize);
                areaSize = hellSize;
                maxSuccesses = hellSuccess;
                fails = hellFail;
                handSpeed = hellSpeed;

            }

            //difficulty chosen successfully
            return true;

        }

    }

}
