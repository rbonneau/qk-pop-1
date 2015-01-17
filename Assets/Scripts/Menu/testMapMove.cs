﻿//this should only be used in the HUDtest scene to test the minimap and compass movement


using UnityEngine;
using System.Collections;

public class testMapMove : MonoBehaviour {
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	void Update() {
		if (Camera.main.GetComponent<HUDMaster> ().hudActive) {
				Camera.main.GetComponent<HUDCam> ().updateMapPos (transform.position.x + 55, transform.position.z + 340);
		}

		if (Input.GetKey ("q")) {
			transform.Rotate(Vector3.down * Time.deltaTime * speed* 10);
		}
		if (Input.GetKey ("e")) {
			transform.Rotate(Vector3.up * Time.deltaTime * speed*10);
		}
		CharacterController controller = GetComponent<CharacterController>();
		if (controller.isGrounded) {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetButton("Jump"))
				moveDirection.y = jumpSpeed;
			
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}
}