using UnityEngine;
using System.Collections;

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


	// Use this for initialization
	void Start()
	{

		//get reference to the StealthClock
		clock = GetComponentInChildren<StealthClock>();

		//get reference to AIManager
		aiMan = AIManager.instance;
		

	}
	
	// Update is called once per frame
	void Update()
	{

		//if the player won or lost the mini-game or is not hidden from ai
		if(clock.gameOver || !aiMan.checkForPlayer())
		{

			//if the mini-game is running
			if(clock.isActiveAndEnabled)
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


				}

				//deactivate mini-game
				gameObject.GetComponentInChildren<GameObject>().SetActive(false);


			}

		}
		//if player is being searched for and in a hiding spot
		else if((aiMan.checkChasing() > 0)/* && player.isHiding()*/)
		{

			//if mini-game isn't running
			if(!clock.isActiveAndEnabled)
			{

				//start mini-game
				transform.GetChild(0).gameObject.SetActive(true);

			}

		}
		//mini-game is running, no AI searching or player is found by AI or player is not in a hiding spot
		else if(clock.isActiveAndEnabled && (aiMan.checkChasing() < 1 || !aiMan.checkForPlayer()/* || !player.isHiding*/))
		{

			//deactivate mini-game
			transform.GetChild(0).gameObject.SetActive(false);

		}

	}

}
