using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour {

	LevelManager levMan;
	GameObject rotControl;
	Transform camObject;
	Rigidbody rb;

	[SerializeField] Text actionText;

	[SerializeField] bool reachedNothing = true;

	public bool allowMove = true;

	enum Stances {standing, crawling};
	Stances myStance;

	string currentSurface;

	bool lerping = false;
	float rotTimeToNewSurface = 0f;



	void Start () {
		camObject = transform.GetChild (0);
		rb = gameObject.GetComponent<Rigidbody> ();
		levMan = GameObject.Find("LevelManager").GetComponent<LevelManager>();

		//actionText = GameObject.Find("ActionText").GetComponent<Text>();

		myStance = Stances.standing;

		rotControl = new GameObject("RotationControl");
		rotControl.transform.position = transform.position;
		rotControl.transform.Rotate(rotControl.transform.right, -90f, Space.Self);
		rotControl.transform.parent = transform;
	}
	

	void Update () {
		if (allowMove) {
			if (myStance == Stances.standing) {
				Move (2.0f * Time.deltaTime);
			} else if (myStance == Stances.crawling) {
				Move (0.5f * Time.deltaTime);
			}

			Rotate();
			ChangeStance();
			ReachingRaycast();

			if (lerping) {
				StickToSurface();
			}

			if (reachedNothing == false) {
				actionText.gameObject.SetActive(true);
			}
			else {
				actionText.gameObject.SetActive(false);
				actionText.text = "";
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
		if (Input.mousePosition != null && Input.mousePosition.x > 0.0f && Input.mousePosition.x < camObject.GetComponent<Camera> ().pixelWidth
			&& Input.mousePosition.y > 0.0f && Input.mousePosition.y < camObject.GetComponent<Camera> ().pixelHeight) {

			if (Input.mousePosition.x <= camObject.GetComponent<Camera> ().pixelWidth * 0.4f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelWidth / Input.mousePosition.x;
				transform.Rotate (-Vector3.up * rotSpeed * Time.deltaTime);
			} else if (Input.mousePosition.x >= camObject.GetComponent<Camera> ().pixelWidth * 0.6f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelWidth / (camObject.GetComponent<Camera>().pixelWidth - Input.mousePosition.x);
				transform.Rotate (Vector3.up * rotSpeed * Time.deltaTime);
			}

//TODO Clean up this clamped rotation, to get rid of the jittering at the top and bottom
			//Look Vertical
			//Down
			if (Input.mousePosition.y <= camObject.GetComponent<Camera> ().pixelHeight * 0.4f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelHeight / Input.mousePosition.y;
				camObject.transform.Rotate (Vector3.right * rotSpeed * Time.deltaTime);
			}
			//Up
			else if (Input.mousePosition.y >= camObject.GetComponent<Camera> ().pixelHeight * 0.6f) {
				float rotSpeed = camObject.GetComponent<Camera> ().pixelHeight / (camObject.GetComponent<Camera>().pixelHeight - Input.mousePosition.y);
				camObject.transform.Rotate (-Vector3.right * rotSpeed * Time.deltaTime);
			}

			float camRotxMin = Mathf.Min(camObject.transform.localEulerAngles.x, -90.0f);
			float camRotxMax = Mathf.Max(camObject.transform.localEulerAngles.x, 90.0f);
			float camRot = Mathf.Clamp(camObject.transform.localEulerAngles.x, camRotxMin, camRotxMax);
			camObject.transform.localEulerAngles = new Vector3(camRot, 0f, 0f);
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


	void ReachingRaycast() {
		Ray reachForward = new Ray(transform.position, transform.forward);

		Debug.DrawRay(transform.position, transform.forward, Color.red);
		float reachDist = 0.5f;

		ChangeIncline(reachForward, reachDist);
		ActivateObject(reachForward, reachDist);
	}


	void ChangeIncline (Ray reachFor, float reachDistance) {
		RaycastHit reachedFor;
		
		if (Physics.Raycast(reachFor, out reachedFor, reachDistance)) {
			reachedNothing = false;

			//CHANGE INCLINE
			if (reachedFor.collider.CompareTag("Floor") ||
				reachedFor.collider.CompareTag("Wall") ||
				reachedFor.collider.CompareTag("Ceiling")) {
				actionText.text = "Press E to change incline";
				//print("Press E key to change incline");

				if (Input.GetKeyDown(KeyCode.E)) {
					Physics.gravity = -reachedFor.normal * 9.8f;

					myStance = Stances.crawling;
					currentSurface = reachedFor.collider.tag;

					lerping = true;
				}
			}
		}
		else {
			reachedNothing = true;
		}
	}


	void ActivateObject(Ray reachRay, float reachDistance) {
		RaycastHit reachedFor;

		if (Physics.Raycast(reachRay, out reachedFor, reachDistance)) {
			reachedNothing = false;

			switch (reachedFor.collider.tag) {
				case "Door":
					actionText.text = "Press E to open door";
					//print("Press E key to open door");

					if (Input.GetKeyDown(KeyCode.E)) {
						if (reachedFor.collider.gameObject.GetComponent<LockManager>() != null) {
							reachedFor.collider.gameObject.GetComponent<LockManager>().enabled = true;
						}
					}
					break;
				case "DisplayCase":
					actionText.text = "Press E to open display case";

					//print("Press E key to open display case");

					if (Input.GetKeyDown(KeyCode.E)) {
						if (reachedFor.collider.gameObject.GetComponent<PuzzleManager>() != null) {
							reachedFor.collider.gameObject.GetComponent<PuzzleManager>().enabled = true;
						}
					}
					break;
				case "AlarmBox":
					if (reachedFor.collider.GetComponent<AlarmManager>() != null) {
						if (!reachedFor.collider.GetComponent<AlarmManager>().lasersAlreadyDisabled || LevelManager.timerState == LevelManager.TimerOn.timerActivated) {
							actionText.text = "Press E to open alarm box";

							//print("Press E key to open Alarm Box");

							if (Input.GetKeyDown(KeyCode.E)) {
								reachedFor.collider.gameObject.GetComponent<AlarmManager>().enabled = true;
							}
						}
					}
					break;
				default:

					break;
			}
		}
		else {
			reachedNothing = true;
		}
	}


	void StickToSurface () {
		//THE LERP
		if (lerping && rotTimeToNewSurface < 1.0f) {
			rotTimeToNewSurface += Time.deltaTime * 1.5f;
			Vector3 rotPole = transform.position - Physics.gravity;

			rotControl.transform.LookAt(rotPole, transform.forward);

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotControl.transform.up, rotControl.transform.forward), rotTimeToNewSurface);

			rotControl.transform.localRotation = Quaternion.Euler(Vector3.right * -90);
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
