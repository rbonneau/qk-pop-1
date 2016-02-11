using UnityEngine;
using System.Collections;

public class StealthHand : MonoBehaviour
{

	//green area on clock face between startDegree and endDegree
	private float _startDegree;
	private float _endDegree;


	public int startDegree
	{
		get
		{
			return System.Convert.ToInt32(_startDegree);
		}
	}

	public int endDegree
	{
		get
		{
			return System.Convert.ToInt32(_endDegree);
		}
	}


	//number of successful presses before winning minigame
	private int maxSuccess;
	//current total of successful presses
	private int currentSuccess;

	//number of failed presses before losing minigame
	private int maxFail;
	//current total of failed presses
	private int currentFail;

	//number of enemies, affects clock speed
	private int enemiesNearby;
	//enemy distance to player, affects clock speed
	private float distanceToPlayer;
	//default speed of clock hand in degrees, 6f = 1rpm
	private float speed = 6f;

	//size of green area on clock, in degrees, affected by number of enemies and their distance to player
	private float areaSize;

	//StealthClock, the face of the clock, should be child of this StealthHand
	GameObject clockFace;

	// Use this for initialization
	void Start()
	{

		clockFace = transform.FindChild("StealthClock").gameObject;

//START TEMP
		//find/get number of enemies nearby
		enemiesNearby = 1;

		//set maxSuccess/maxFail
		//should eventually take into account number of searching guards and their distance to player
		maxSuccess = 3;
		maxFail = 3;

		//set rotation speed
		speed *= enemiesNearby;
//END TEMP

	}

	// Update is called once per frame
	void Update()
	{

		//rotate hand
		transform.RotateAround(clockFace.transform.position, clockFace.transform.up, Time.deltaTime * speed);
		
//		transform.RotateAround(transform.parent.position, transform.parent.transform.up, Time.deltaTime * speed);

		if(Input.GetButtonDown("action"))
		{

			//check for success/failure
			if((transform.rotation.y >= _startDegree) && (transform.rotation.y <= _endDegree))
			{
				currentSuccess++;
			}
			else
			{
				currentFail++;
			}

			if(currentSuccess >= maxSuccess)
			{

				//enemies return to normal


			}
			else if(currentFail >= maxFail)
			{

				//alert enemies to player position

				
			}

		}

		if(currentFail <= maxFail)
		{

			//do failure work

		}
		else if(currentSuccess >= maxSuccess)
		{

			//do successes work

		}

	}

}
