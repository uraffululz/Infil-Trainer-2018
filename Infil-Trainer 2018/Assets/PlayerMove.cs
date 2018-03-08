using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	Transform camObject;

	float moveSpeed;


	void Awake () {
	}


	void Start () {
		print (transform.childCount);
		camObject = transform.GetChild (0);
	}
	

	void Update () {
		Move ();
		Rotate ();
	}


	void Move () {
		moveSpeed = 1.0f * Time.deltaTime;

		if (Input.GetAxis("Horizontal") != 0.0) {
			transform.position = transform.position + Vector3.right * moveSpeed * Input.GetAxis("Horizontal");
		}
		if (Input.GetAxis("Vertical") != 0.0f) {
			transform.position = transform.position + Vector3.forward * moveSpeed * Input.GetAxis("Vertical");
		}
	}


	void Rotate () {
		
	}
}
