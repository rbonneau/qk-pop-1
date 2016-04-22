using UnityEngine;
using System.Collections;
using Debug = FFP.Debug;

public class StealthGameManager : MonoBehaviour
{

    /*!
		\file StealthGameManager.cs
		\brief This class manages the stealth mini-game gameObjects.

		StealthGameManger should be attached to an empty gameObject that is the parent of the mini-game objects.
        It monitors game conditions and activates and deactivates the mini-game components accordingly. This class
        is a singleton.
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
    public int easyGuards = 2;      /*!<max number of guards searching for player for the mini-game to be easy difficulty*/
    public int mediumGuards = 4;    /*!<max number of guards searching for player for the mini-game to be medium difficulty*/
    public int hardGuards = 10;     /*!<max number of guards searching for player for the mini-game to be hard difficulty*/

    //size of green area in degrees
    public int easySize = 45;       /*!<size of green area for easy difficulty*/
    public int mediumSize = 30;     /*!<size of green area for medium difficulty*/
    public int hardSize = 20;       /*!<size of green area for hard difficulty*/
    public int hellSize = 10;       /*!<size of green area for hell difficulty*/

    //number of successes necessary for a win
    public int easySuccess = 3;     /*!<number of successes necessary for a win with easy difficulty*/
    public int mediumSuccess = 3;   /*!<number of successes necessary for a win with medium difficulty*/
    public int hardSuccess = 3;     /*!<number of successes necessary for a win with hard difficulty*/
    public int hellSuccess = 3;     /*!<number of successes necessary for a win with hell difficulty*/

    //number of failures necessary for a loss
    public int easyFail = 1;        /*!<number of failures necessary for a loss with easy difficulty*/
    public int mediumFail = 1;      /*!<number of failures necessary for a loss with medium difficulty*/
    public int hardFail = 1;        /*!<number of failures necessary for a loss with hard difficulty*/
    public int hellFail = 1;        /*!<number of failures necessary for a loss with hell difficulty*/

    //speed of clock hand in rpms
    public int easySpeed = 20;      /*!<speed of clock hand in rpms with easy difficulty*/
    public int mediumSpeed = 20;    /*!<speed of clock hand in rpms with medium difficulty*/
    public int hardSpeed = 20;      /*!<speed of clock hand in rpms with hard difficulty*/
    public int hellSpeed = 40;      /*!<speed of clock hand in rpms with hell difficulty*/

    //settings that the mini-game will use
    public int areaSize;            /*!<current size of green area in degrees that will be used by the mini-game*/
    public int maxSuccesses;        /*!<current number of successes necessary to win the mini-game*/
    public int fails;               /*!<current number of fails necessary to win the mini-game*/
    public int handSpeed;           /*!<current speed of the mini-game clock hand*/

    // Use this for initialization
    void Start()
	{

		//get reference to the StealthClock
		clock = GetComponentInChildren<StealthClock>();

		//get reference to AIManager
		aiMan = AIManager.instance;

        //reference to player
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //deactivate mini-game
        foreach(Transform child in transform)
        {

            //deactivate children
            child.gameObject.SetActive(false);

        }

        //check for clock existence
        if(clock == null)
        {
            Debug.Log("player", "clock = null");
        }
        else
        {
            Debug.Log("player", "StealthGameManager.Start(): clock.isActiveAndEnabled = " + clock.isActiveAndEnabled);
        }

    }
	
	// Update is called once per frame
	void Update()
	{

        //if the player won or lost the mini-game or is not hidden from ai
        if(clock.isActiveAndEnabled && (clock.gameOver || !QK_Character_Movement.Instance.isHidden))
        {
            
			//check for win
			if(clock.win)
			{

				//reset searching AI
				aiMan.resumePatrol();
                Debug.Log("player", "mini-game won, ai resuming patrol");

			}
			//check for loss
			else if(clock.fail)
			{

                //notify AI of player location
                aiMan.resumeChase();
                Debug.Log("player", "mini-game lost, ai resuming chase");

            }

			//deactivate mini-game
            foreach(Transform child in transform)
            {

                //deactivate children
                child.gameObject.SetActive(false);

            }

            //allow player to move
            resumeMovement();

        }
		//if button pressed and player is being searched for and in a hiding spot, and the game is not paused
        else if(!clock.isActiveAndEnabled && Input.GetKeyDown("f") && (aiMan.numberChasing > 0) && QK_Character_Movement.Instance.isHidden && !GameHUD.Instance.pauseMenu.activeInHierarchy)
        {

            //if mini-game isn't running
            if(!clock.isActiveAndEnabled)
			{

                //calculate mini-game difficulty
                if(chooseDifficulty())
                {

                    //start mini-game
                    foreach(Transform child in transform)
                    {

                        //activate children
                        child.gameObject.SetActive(true);

						//stop player movement
						stopMovement();

                    }

                }
                else
                {

                    //output error
                    Debug.Log("player", "StealthGameManager.chooseDifficulty() == false");

                }

			}

		}
        //mini-game is running, no AI searching or player is not in a hiding spot
		else if(clock.isActiveAndEnabled && (aiMan.checkChasing() < 1 || !QK_Character_Movement.Instance.isHidden))
        {
            
            //deactivate mini-game
            transform.GetChild(0).gameObject.SetActive(false);

			//resume movement
			resumeMovement();
            
		}

//TESTING
//        else
//        {
/*            if(clock == null)
            {
                Debug.Log("player", "StealthMiniGame.Update(): (clock = null) " + (clock == null));
            }
            else
            {
                Debug.Log("player", "clock.isActiveAndEnabled = " + clock.isActiveAndEnabled);
            }
//            Debug.Log("player", "aiMan.checkChasing() = " + aiMan.checkChasing());
//            Debug.Log("player", "aiMan.numberChasing() = " + aiMan.numberChasing);
//            Debug.Log("player", "!aiMan.checkForPlayer() = " + !aiMan.checkForPlayer());
//            Debug.Log("player", "!QK_Character_Movement.Instance.isHidden " + !QK_Character_Movement.Instance.isHidden);
        
        }
*/
//END TESTING
    }

    /*
        \brief  Chooses difficulty of the mini-game

        Checks for exitence of enemies actively searching for the player. Chooses the difficulty of the
        mini-game based on the number of searching enemies. If there are no enemies, nothing is set and
        returns false.

        \return bool, true if difficulty was successfully chosen, false otherwise
    */
    private bool chooseDifficulty()
    {
//TESTING
        Debug.Log("player", "StealthGameManager.chooseDifficulty: started");
//END TESTING
        //current count of guards searching for player
        //        int tempGuards = 0;

        //number of suspicious guards
        int _numberOfGuards;

        //check for existence of enemies
        if(aiMan.AiChildren == null)
        {
            
            //no searching guards, difficulty not set
            return false;

        }
        else
        {

            //call AI manager for suspicious guards
            _numberOfGuards = aiMan.checkChasing();

            //check for guards looking for player
            if(_numberOfGuards < 1)
            {

                //no guards searching for player
                Debug.Log("player", "StealthGameManager.ChooseDifficulty(): _numberOfGuards < 1");
                return false;

            }

            //set mini-game difficulty variables
            //easy
            if(_numberOfGuards < easyGuards)
            {

                //_endDegree = getEndDegree(_startDegree, _easySize);
                areaSize = easySize;
                maxSuccesses = easySuccess;
                fails = easyFail;
                handSpeed = easySpeed;

            }
            //medium
            else if(_numberOfGuards < mediumGuards)
            {

                //_endDegree = getEndDegree(_startDegree, _mediumSize);
                areaSize = mediumSize;
                maxSuccesses = mediumSuccess;
                fails = mediumFail;
                handSpeed = mediumSpeed;

            }
            //hard
            else if(_numberOfGuards < hardGuards)
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

    /*
        \brief  Resume camera and player movement

        Resets camera to allow for movement. Should allow player movement but can't due to set
        accessor being inaccessible.

        \return void
    */
	private void resumeMovement()
	{

		//allow camera movement
		PoPCamera.instance.Reset();
		
		//allow player movement
		QK_Character_Movement.Instance._moveState = CharacterStates.Normal;

	}

    /*
        \brief  Stop camera and player movement

        Pauses camera movement.Should keep player from moving while the mini-game is active. Can't
        stop player movement due to set accessor being inaccessible.

        \return void
    */
	private void stopMovement()
	{

		//freeze camera
		PoPCamera.State =  Camera_2.CameraState.Pause;
		
		//freeze movement
      QK_Character_Movement.Instance._moveState = CharacterStates.Wait;

	}

}
