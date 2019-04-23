using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour {
	GameObject rotControl;
	Transform camObject;
	Rigidbody rb;

	public bool allowMove = true;

	enum Stances {standing, crawling};
	Stances myStance;

	string currentSurface;

	bool lerping = false;
	float rotTimeToNewSurface = 0f;


	void Awake () {
		
	}


	void Start () {
//TODO Alternatively, I could just make this camera a component of the player GameObject, if I can keep its vertical offset
		camObject = transform.GetChild (0);
		rb = gameObject.GetComponent<Rigidbody> ();

		myStance = Stances.standing;

		rotControl = new GameObject("RotationControl");
		rotControl.transform.position = transform.position;
		rotControl.transform.Rotate(rotControl.transform.right, -90f, Space.Self);
		rotControl.transform.parent = transform;
	}
	

	void Update () {
		Ray reachForward = new Ray (transform.position, transform.forward);

		if (allowMove) {
			Rotate();

			if (myStance == Stances.standing) {
				Move (2.0f * Time.deltaTime);
			} else if (myStance == Stances.crawling) {
				Move (0.5f * Time.deltaTime);
			}
			ChangeStance();
			ChangeIncline(reachForward);

			if (lerping) {
				StickToSurface();

			}
		}
	}

	void OnCollisionEnter (Collision dat) {
		if (dat.collider.gameObject.name == "LevelEnd") {
			SceneManager.LoadScene ("TestScene");
		}
	}


	void Move (float moveSpeed) {
		if (Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f) {
			Vector3 playerPos = transform.position;
			transform.position = playerPos + (transform.right * moveSpeed * Input.GetAxis("Horizontal") + transform.forward * moveSpeed * Input.GetAxis("Vertical"));
		}
	}


	void Rotate () {
		if (Input.mousePosition != null &&
			Input.mousePosition.x > 0.0f && Input.mousePosition.x < camObject.GetComponent<Camera> ().pixelWidth
		    && Input.mousePosition.y > 0.0f && Input.mousePosition.y < camObject.GetComponent<Camera> ().pixelHeight) {

			if (Input.mousePosition.x <= camObject.GetComponent<Camera> ().pixelWidth * 0.3f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelWidth / Input.mousePosition.x;
				transform.Rotate (-Vector3.up * rotSpeed * Time.deltaTime);
			} else if (Input.mousePosition.x >= camObject.GetComponent<Camera> ().pixelWidth * 0.7f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelWidth / (camObject.GetComponent<Camera>().pixelWidth - Input.mousePosition.x);
				transform.Rotate (Vector3.up * rotSpeed * Time.deltaTime);
			}

//TODO Clean up this clamped rotation, to get rid of the jittering at the top and bottom
			//Look Vertical
			//Down
			if (Input.mousePosition.y <= camObject.GetComponent<Camera> ().pixelHeight * 0.3f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelHeight / Input.mousePosition.y;
				camObject.transform.Rotate (Vector3.right * rotSpeed * Time.deltaTime);
				float camRotxMin = Mathf.Min(camObject.transform.localEulerAngles.x, -90.0f);
				float camRotxMax = Mathf.Max(camObject.transform.localEulerAngles.x, 90.0f);
				float camRot = Mathf.Clamp(camObject.transform.localEulerAngles.x, camRotxMin, camRotxMax);
				camObject.transform.localEulerAngles = new Vector3(camRot, 0f, 0f);
			}
			//Up
			else if (Input.mousePosition.y >= camObject.GetComponent<Camera> ().pixelHeight * 0.7f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelHeight / (camObject.GetComponent<Camera>().pixelHeight - Input.mousePosition.y);
				camObject.transform.Rotate (-Vector3.right * rotSpeed * Time.deltaTime);
				float camRotxMin = Mathf.Min(camObject.transform.localEulerAngles.x, -90.0f);
				float camRotxMax = Mathf.Max(camObject.transform.localEulerAngles.x, 90.0f);
				float camRot = Mathf.Clamp(camObject.transform.localEulerAngles.x, camRotxMin, camRotxMax);
				camObject.transform.localEulerAngles = new Vector3(camRot, 0f, 0f);
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


	void ChangeIncline (Ray reachFor) {
		RaycastHit reachedFor;

		float reachDist = 0.5f;
		Debug.DrawRay (transform.position, transform.forward, Color.red);

		if (Physics.Raycast(reachFor, out reachedFor, reachDist)) {
			//CHANGE INCLINE
			if (reachedFor.collider.CompareTag ("Floor") ||
				reachedFor.collider.CompareTag ("Wall") ||
				reachedFor.collider.CompareTag ("Ceiling")) {
				print ("Press E key to change incline");

				if (Input.GetKeyDown(KeyCode.E)) {
					Physics.gravity = -reachedFor.normal * 9.8f;

					myStance = Stances.crawling;
					currentSurface = reachedFor.collider.tag;

					lerping = true;

					print ("Current Surface: " + currentSurface);
				}
			}
			//OPEN DOORS
			else if (reachedFor.collider.CompareTag ("Door")) {
				print ("Press E key to open door");

				if (Input.GetKeyDown(KeyCode.E)) {
					if (reachedFor.collider.gameObject.GetComponent<LockManager> () != null) {
						reachedFor.collider.gameObject.GetComponent<LockManager> ().enabled = true;
					}
				}
			}
			//OPEN DISPLAY CASES
			else if (reachedFor.collider.CompareTag ("DisplayCase")) {
				print ("Press E key to open display case");

				if (Input.GetKeyDown(KeyCode.E)) {
					if (reachedFor.collider.gameObject.GetComponent<PuzzleManager> () != null) {
						reachedFor.collider.gameObject.GetComponent<PuzzleManager> ().enabled = true;
						print ("The display case is open");
					}
				}
			}
			//OPEN THE ALARM BOX
			else if (reachedFor.collider.CompareTag("AlarmBox")) {
				print ("Press E key to open Alarm Box");
				if (Input.GetKeyDown(KeyCode.E)) {
					if (reachedFor.collider.gameObject.GetComponent<AlarmManager>() != null) {
						reachedFor.collider.gameObject.GetComponent<AlarmManager> ().enabled = true;
						print ("The Alarm Box is open");
					}
				}
			}
		}
	}


	void StickToSurface () {

		//Vector3 rotFrom = transform.up;
		//Vector3 rotTo = reachedSurface.normal;
		//Vector3 rotTo = -Physics.gravity;

		//float zLerpAngleCorrect = Vector3.Angle(rotFrom, rotTo);

		//print (reachedSurface.normal);

//TODO Incorporate "The lerp" into this rotation to make the player's rotation less jarring
//Probably need a coroutine
		//transform.rotation = Quaternion.FromToRotation (transform.up, reachedSurface.normal) * rb.rotation; 

		//Quaternion targetRot = Quaternion.AngleAxis (-90.0f, transform.right) * rb.rotation;

//FAILED ATTEMPT GRAVEYARD
		//Vector3 thisRot = Vector3.RotateTowards (transform.up, reachedSurface.normal, Time.deltaTime * 0.1f, 0.0f);
		//transform.rotation = Quaternion.LookRotation (thisRot);

		

		if (lerping && rotTimeToNewSurface < 1.0f) {
			rotTimeToNewSurface += Time.deltaTime * 1.5f;
			Vector3 rotPole = transform.position - Physics.gravity;

			rotControl.transform.LookAt(rotPole, transform.forward);

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotControl.transform.up, rotControl.transform.forward), rotTimeToNewSurface);

			rotControl.transform.localRotation = Quaternion.Euler(Vector3.right * -90);

			//float rotPoleAngle = Vector3.Angle(transform.forward, rotPole);

			//print(rotPoleAngle);

			//transform.rotation = Quaternion.Euler(Vector3.RotateTowards(transform.up, rotPole, rotTimeToNewSurface, 1));

			//transform.rotation = Quaternion.Lerp(Quaternion.Euler(transform.right), Quaternion.Euler(transform.position - Physics.gravity), rotTimeToNewSurface);

			//transform.Rotate(-90 * (Time.deltaTime * 2), 0, -zLerpAngleCorrect * (Time.deltaTime * 2), Space.Self);




		//transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, t);

		//transform.rotation = Quaternion.Lerp (Quaternion.LookRotation (transform.forward, transform.up), 
			//Quaternion.LookRotation (transform.up, reachedSurface.normal), 1.0f);
			//rb.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (reachedSurface.normal), t);

		}
		else if (rotTimeToNewSurface >= 1.0f) {
			rotTimeToNewSurface = 0;
			lerping = false;

			rotControl.transform.localRotation = Quaternion.Euler(Vector3.right * -90);

		}

	}


	void FallToFloor () {
		//Change direction of gravity to WORLD DOWN and rotate player so he lands feet-down
		Physics.gravity = Vector3.down * 9.8f;
		transform.rotation = Quaternion.FromToRotation (transform.up, Vector3.up) * rb.rotation;
	}
}
