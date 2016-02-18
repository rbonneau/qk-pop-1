using UnityEngine;
using System.Collections;

public class StealthHand : MonoBehaviour
{

	//green area on clock face between startDegree and endDegree
	public float startDegree;
	public float endDegree;
	public float quatTest;
	public float currentY;
	public float currentWorldY;



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
//	GameObject clockFace;
	StealthClock clockFace;

	// Use this for initialization
	void Start()
	{

		//initialize success/fail counts
		currentSuccess = 0;
		currentFail = 0;

		//get the clock face
		clockFace = transform.GetComponentInParent<StealthClock>();

		//set green borders
		startDegree = clockFace.startDegree;
		endDegree = clockFace.endDegree;

//START TEMP
		//find/get number of enemies nearby
//		enemiesNearby = 1;
		enemiesNearby = clockFace.numberOfGuards;
		if(enemiesNearby < 1)
		{
			enemiesNearby = 1;
			Debug.Log("StealthHand: enemiesNearby < 1");
		}

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
//		transform.RotateAround(clockFace.transform.position, clockFace.transform.up, Time.deltaTime * speed);
		
		transform.RotateAround(transform.parent.position, transform.parent.transform.up, -Time.deltaTime * -speed);

//		currentY = transform.eulerAngles.y;
//		transform.eulerAngles.y;
		currentY = transform.localEulerAngles.y;

		currentWorldY = transform.rotation.eulerAngles.y;

		quatTest = Quaternion.Inverse(transform.rotation).eulerAngles.y;

		if(Input.GetButtonDown("Jump"))
		{

			if(startDegree < endDegree)
			{
				//check for success/failure
				if((startDegree <= transform.localEulerAngles.y) && (transform.localEulerAngles.y <= endDegree))
				{
					currentSuccess++;
					print("currentSuccess: " + currentSuccess);
					print("startDegree: " + startDegree);
					print("transform.localEulerAngles.y: " + transform.localEulerAngles.y);
					print("endDegree: " + endDegree);

				}
				else
				{
					currentFail++;
					print("currentFail: " + currentFail);
					print("startDegree: " + startDegree);
					print("transform.localEulerAngles.y: " + transform.localEulerAngles.y);
					print("endDegree: " + endDegree);
				}
/*
			}
			else
			{
				if((startDegree <= transform.rotation.y) || (transform.rotation.y <= endDegree))
				{

					currentSuccess++;

				}
				else
				{

					currentFail++;

				}
*/
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

	}

}
