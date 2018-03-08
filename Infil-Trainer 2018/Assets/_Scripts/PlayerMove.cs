using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	Transform camObject;

	enum Stances {standing, crawling};
	Stances myStance;


	void Awake () {
		
	}


	void Start () {
		print (transform.childCount);
		camObject = transform.GetChild (0);

		myStance = Stances.standing;
	}
	

	void Update () {
		if (myStance == Stances.standing) {
			Move (1.0f * Time.deltaTime);
			Rotate ();
			ChangeStance ();
		} else if (myStance == Stances.crawling) {
			Move (0.5f * Time.deltaTime);
			Rotate ();
			ChangeStance ();
		}
	}


	void Move (float moveSpeed) {

		if (Input.GetAxis("Horizontal") != 0.0) {
			transform.position = transform.position + Vector3.right * moveSpeed * Input.GetAxis("Horizontal");
		}
		if (Input.GetAxis("Vertical") != 0.0f) {
			transform.position = transform.position + Vector3.forward * moveSpeed * Input.GetAxis("Vertical");
		}
	}


	void Rotate () {
		if (Input.mousePosition.x <= camObject.GetComponent<Camera>().pixelWidth * 0.4f) {
			float rotSpeed = camObject.GetComponent<Camera>().pixelWidth/Input.mousePosition.x;
			transform.Rotate (-Vector3.up * rotSpeed * Time.deltaTime);
			print (rotSpeed);

		} else if (Input.mousePosition.x >= camObject.GetComponent<Camera>().pixelWidth * 0.6f) {
			float rotSpeed = camObject.GetComponent<Camera>().pixelWidth/(800 - Input.mousePosition.x);
			transform.Rotate (Vector3.up * rotSpeed * Time.deltaTime);
			print (rotSpeed);

		}

		if (Input.mousePosition.y <= camObject.GetComponent<Camera>().pixelHeight * 0.4f) {
			float rotSpeed = camObject.GetComponent<Camera>().pixelHeight/Input.mousePosition.y;
			camObject.transform.Rotate (Vector3.right * rotSpeed * Time.deltaTime);
			print (rotSpeed);

		} else if (Input.mousePosition.y >= camObject.GetComponent<Camera>().pixelHeight * 0.6f) {
			float rotSpeed = camObject.GetComponent<Camera>().pixelHeight/(600 - Input.mousePosition.y);
			camObject.transform.Rotate (-Vector3.right * rotSpeed * Time.deltaTime);
			print (rotSpeed);

		}
	}

	void ChangeStance () {
		if (myStance == Stances.standing) {
			if (Input.GetKeyDown(KeyCode.LeftControl)) {
				myStance = Stances.crawling;
				Vector3 playerScale = new Vector3 (0.5f, 0.25f, 0.5f);
				transform.localScale = playerScale;
			}
		} else if (myStance == Stances.crawling) {
			if (Input.GetKeyDown(KeyCode.LeftControl)) {
				myStance = Stances.standing;
				Vector3 playerScale = Vector3.one * 0.5f;
				transform.localScale = playerScale;
			}
		}
		print (myStance);
	}
}
