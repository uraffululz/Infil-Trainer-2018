using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	Transform camObject;
	Rigidbody rb;

	enum Stances {standing, crawling};
	Stances myStance;
	string currentSurface;


	void Awake () {
		
	}


	void Start () {
		camObject = transform.GetChild (0);
		rb = gameObject.GetComponent<Rigidbody> ();

		myStance = Stances.standing;
	}
	

	void Update () {
		if (myStance == Stances.standing) {
			Move (1.0f * Time.deltaTime);
			Rotate ();
			ChangeStance ();
			ChangeIncline ();
		} else if (myStance == Stances.crawling) {
			Move (0.5f * Time.deltaTime);
			Rotate ();
			ChangeStance ();
			ChangeIncline ();
		}
	}


	void Move (float moveSpeed) {

		if (Input.GetAxis("Horizontal") != 0.0) {
			rb.MovePosition(transform.position + (transform.right * moveSpeed * Input.GetAxis("Horizontal")));
		}
		if (Input.GetAxis("Vertical") != 0.0f) {
			rb.MovePosition(transform.position + (transform.forward * moveSpeed * Input.GetAxis("Vertical")));
		}
	}


	void Rotate () {
		if (Input.mousePosition != null &&
			Input.mousePosition.x > 0.0f && Input.mousePosition.x < (float)camObject.GetComponent<Camera> ().pixelWidth
		    && Input.mousePosition.y > 0.0f && Input.mousePosition.y < (float)camObject.GetComponent<Camera> ().pixelHeight) {

			if (Input.mousePosition.x <= camObject.GetComponent<Camera> ().pixelWidth * 0.4f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelWidth / Input.mousePosition.x;
				transform.Rotate (-Vector3.up * rotSpeed * Time.deltaTime);
			} else if (Input.mousePosition.x >= camObject.GetComponent<Camera> ().pixelWidth * 0.6f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelWidth / (800 - Input.mousePosition.x);
				transform.Rotate (Vector3.up * rotSpeed * Time.deltaTime);
			}

			if (Input.mousePosition.y <= camObject.GetComponent<Camera> ().pixelHeight * 0.4f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelHeight / Input.mousePosition.y;
				camObject.transform.Rotate (Vector3.right * rotSpeed * Time.deltaTime);
			} else if (Input.mousePosition.y >= camObject.GetComponent<Camera> ().pixelHeight * 0.6f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelHeight / (600 - Input.mousePosition.y);
				camObject.transform.Rotate (-Vector3.right * rotSpeed * Time.deltaTime);
			}
		}
	}

	void ChangeStance () {
		if (myStance == Stances.standing) {
			Vector3 playerScale = Vector3.one * 0.5f;
			transform.localScale = playerScale;

			if (Input.GetKeyDown(KeyCode.LeftControl)) {
				myStance = Stances.crawling;
			}
		} else if (myStance == Stances.crawling) {
			Vector3 playerScale = new Vector3 (0.5f, 0.25f, 0.5f);
			transform.localScale = playerScale;

			if (Input.GetKeyDown(KeyCode.LeftControl)) {
				myStance = Stances.standing;

				if (currentSurface == "Wall" || currentSurface == "Ceiling") {
					FallToFloor ();
				}
			}
		}
	}


	void ChangeIncline () {
		Ray reachForward = new Ray (transform.position, transform.forward);
		RaycastHit reachedForward;
		Debug.DrawRay (transform.position, transform.forward, Color.red);

		if (Physics.Raycast(reachForward, out reachedForward, 1.0f)) {
			if (reachedForward.collider.CompareTag ("Floor") ||
				reachedForward.collider.CompareTag ("Wall") ||
				reachedForward.collider.CompareTag ("Ceiling")) {
				print ("Press E key to change incline");

				if (Input.GetKeyDown(KeyCode.E)) {
					StickToSurface (-reachedForward.normal * 9.8f, reachedForward);
					myStance = Stances.crawling;
					currentSurface = reachedForward.collider.tag;
					print ("Current Surface: " + currentSurface);
				}
			}
		}
	}


	void StickToSurface (Vector3 gravDir, RaycastHit reachedSurface) {
		Physics.gravity = gravDir;
		transform.rotation = Quaternion.FromToRotation (transform.up, reachedSurface.normal) * rb.rotation;
	}


	void FallToFloor () {
		//Change direction of gravity to WORLD DOWN and rotate player so he lands feet-down
		Physics.gravity = Vector3.down * 9.8f;
		transform.rotation = Quaternion.FromToRotation (transform.up, Vector3.up) * rb.rotation;
	}
}
