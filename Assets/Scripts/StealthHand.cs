using UnityEngine;
//using System.Collections;

public class StealthHand : MonoBehaviour
{

    /*!
        \file   StealthHand.cs
        \brief  This class attaches to and rotates a clock hand.
        
        This script should be attached to a clock hand which is a child of
        a clock face with a StealthClock component.
    
        The clock hand rotates at a speed set by the clock face.
    */

    //default speed of clock hand in degrees, 6f = 1rpm
    private float _defaultSpeed = 6f;
	
	//StealthClock, the face of the clock, should be parent of this StealthHand
	StealthClock clockFace;

	// Use this for initialization
	void Start()
	{

//TESTING
		Debug.Log("StealthHand Start() beginning");
//END TESTING

		//get the clock face
		clockFace = transform.GetComponentInParent<StealthClock>();
		
		//start the hand at a random degree
		transform.RotateAround(transform.parent.position, transform.parent.transform.up, Random.Range(0, 360));

	}

	// Update is called once per frame
	void Update()
	{

		//rotate hand
		transform.RotateAround(transform.parent.position, transform.parent.transform.up, -Time.deltaTime * _defaultSpeed * clockFace.clockSpeed);

	}

}
