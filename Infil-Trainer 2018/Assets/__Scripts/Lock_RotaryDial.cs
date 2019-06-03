using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock_RotaryDial : MonoBehaviour {

	LockManager lockMan;
	Camera mainCam;
	GameObject rotCam;
	Camera camCom;

	[SerializeField] GameObject platePrefab;
	[SerializeField] GameObject myPlate;
	[SerializeField] GameObject dialPrefab;
	[SerializeField] GameObject myDial;

	static int locksSpawned;

	enum whichDir {right, left};
	whichDir thisDir;

	public int dialNum1;
	public int dialNum2;
	public int dialNum3;

	int dialRotSpeed = 15;
	public float dialAngle = 0.0f;

	float comboStayTimer = 2.0f;
	int puzzleAttempts = 0;

	public enum lockState {findCombo1, findCombo2, findCombo3, unlocked, failed, unsolved};
	public lockState currentState;


	void Awake () {
		lockMan = gameObject.GetComponentInParent<LockManager> ();
		mainCam = GameObject.FindWithTag ("MainCamera").GetComponent<Camera> ();

		platePrefab = gameObject.GetComponent<LockManager>().rotaryPlate;
		dialPrefab = gameObject.GetComponent<LockManager>().rotaryDial;

		RotarySetup ();
		NumberSetup ();
		RotaryCameraSetup ();
	}


	void OnEnable () {
		mainCam.enabled = false;
		camCom.enabled = true;

		myDial.transform.Rotate(0.0f, 0.0f, -dialAngle);

		currentState = lockState.findCombo1;
	}
	

	void Update () {
		RotateDial ();

		if (Input.GetKeyDown(KeyCode.Q)) {
			//This leaves the puzzle "unsolved" (and no longer "inProgress" for now). Later on, there will be alternate states for: 
			currentState = lockState.unsolved;
		}
		if (Input.GetKeyDown (KeyCode.T)) {
			currentState = lockState.unlocked;
		}

//TODO Set up an arrow sprite to show player which direction to rotate dial
		if (currentState == lockState.findCombo1) {
			findingCombo1 ();
		} else if (currentState == lockState.findCombo2) {
			findingCombo2 ();
		} else if (currentState == lockState.findCombo3) {
			findingCombo3 ();
		} else if (currentState == lockState.unlocked) {
			dialSolved ();
		} else if (currentState == lockState.failed) {
			dialFailed ();
		} else if (currentState == lockState.unsolved) {
			dialUnsolved ();
		}
	}


	void RotarySetup () {
		//Vector3 lockOffset = new Vector3 (0.0f, -200.0f, 0.0f);
		//if (GameObject.Find ("BackPlate") != null) {
		//	lockOffset.x += 5.0f;
		//	lockOffset.y -= 10.0f * locksSpawned;
		//}

		myPlate = Instantiate (platePrefab, (transform.position + Vector3.up * -20), Quaternion.identity, transform);
		myPlate.name = "BackPlate";

		myDial = Instantiate (dialPrefab, (transform.position + Vector3.up * -20) + Vector3.right * 0.2f, Quaternion.identity, myPlate.transform);
		myDial.name = "RotaryDial";
	}


	void NumberSetup () {
		dialNum1 = Random.Range (10, 350);
		dialNum2 = Random.Range (10, 350);
		dialNum3 = Random.Range (10, 350);
	}


	void RotaryCameraSetup () {
		rotCam = new GameObject ();
		rotCam.name = "RotaryCam";
		camCom = rotCam.AddComponent<Camera>();
		camCom.CopyFrom (mainCam);
		camCom.farClipPlane = 1.1f;
		camCom.transform.rotation = Quaternion.FromToRotation (mainCam.transform.rotation.eulerAngles, Vector3.zero);

		rotCam.transform.position = myPlate.transform.position + Vector3.back * 1.0f;
		rotCam.transform.parent = myPlate.transform;

		camCom.enabled = false;
	}


	void RotateDial () {
		//Use input to rotate Dial
		myDial.transform.Rotate (Vector3.back, Input.GetAxis ("Horizontal") * dialRotSpeed * Time.deltaTime);
		//Get Dial rotation angle #(between 0 and 360)
		dialAngle = myDial.transform.rotation.eulerAngles.z;
	}


	void findingCombo1 () {
		thisDir = whichDir.right;
		DetermineRotDir ();

		if (/*comboNum1Reached == false && */(int)dialAngle <= dialNum1 + 5 && (int)dialAngle >= dialNum1 - 1) {
			print ("Getting Close to the right number");
//TODO Add vibration/sound to indicate that the dial is nearing the correct number
			if ((int)dialAngle <= dialNum1 + 1 && (int)dialAngle >= dialNum1 - 1) {
/*TODO Increase vibration/sound volume to indicate that the player has reached the correct number
 * If the player stops the dial in the correct position for a second, they will unlock the first combo number and be allowed to continue*/
				if (comboStayTimer > 0.0f) {
					comboStayTimer = comboStayTimer - 1.0f * Time.deltaTime;
				} else if (comboStayTimer <= 0.0f) {
					//comboNum1Reached = true;
					currentState = lockState.findCombo2;
					print ("You found the FIRST DIGIT");
				}
			}
		} else if ((int)dialAngle < dialNum1 - 1 && (int)dialAngle >= dialNum1 - 3) {
			puzzleAttempts++;
			currentState = lockState.failed;
		} else {
			comboStayTimer = 2.0f;
		}
	}


	void findingCombo2 () {
		thisDir = whichDir.left;
		DetermineRotDir ();

//TODO Does the dial need to pass 0 before going to the next digit? If so, just use a "passedZero" bool

		if (/*comboNum2Reached == false && */(int)dialAngle >= dialNum2 - 5 && (int)dialAngle <= dialNum2 + 1) {
			print ("Getting Close to the right number");
//TODO Add vibration/sound to indicate that the dial is nearing the correct number
			if ((int)dialAngle <= dialNum2 + 1 && (int)dialAngle >= dialNum2 - 1) {
/*TODO Increase vibration/sound volume to indicate that the player has reached the correct number
 * If the player stops the dial in the correct position for a second, they will unlock the first combo number and be allowed to continue*/
				if (comboStayTimer > 0.0f) {
					comboStayTimer = comboStayTimer - 1.0f * Time.deltaTime;
				} else if (comboStayTimer <= 0.0f) {
					//comboNum2Reached = true;
					currentState = lockState.findCombo3;
					print ("You found the SECOND DIGIT");
				}
			}
		} else if ((int)dialAngle > dialNum2 + 1 && dialAngle <= dialNum2 + 3) {
			puzzleAttempts++;
			currentState = lockState.failed;
		} else {
			comboStayTimer = 2.0f;
		}
	}


	void findingCombo3 () {
		thisDir = whichDir.right;
		DetermineRotDir ();

		if (/*comboNum3Reached == false && */(int)dialAngle <= dialNum3 + 5 && (int)dialAngle >= dialNum3 - 1) {
			print ("Getting Close to the right number");
//TODO Add vibration/sound to indicate that the dial is nearing the correct number
			if ((int)dialAngle <= dialNum3 + 1 && (int)dialAngle >= dialNum3 - 1) {
/*TODO Increase vibration/sound volume to indicate that the player has reached the correct number
 * If the player stops the dial in the correct position for a second, they will unlock the first combo number and be allowed to continue*/
				if (comboStayTimer > 0.0f) {
					comboStayTimer = comboStayTimer - (1.0f * Time.deltaTime);
				} else if (comboStayTimer <= 0.0f) {
					//comboNum3Reached = true;
					currentState = lockState.unlocked;
					print ("You found the THIRD DIGIT! DOOR UNLOCKED");
				}
			}
		} else if ((int)dialAngle < dialNum3 - 1 && (int)dialAngle >= dialNum3 - 3) {
			puzzleAttempts++;
			currentState = lockState.failed;
		} else {
			comboStayTimer = 2.0f;
		}
	}


	void dialSolved () {
		DestroyLockSetup ();
		mainCam.enabled = true;
		lockMan.solveState = LockManager.lockState.solved;
		Destroy (this);
	}


	void dialFailed () {
		if (puzzleAttempts < 3) {
			myDial.transform.Rotate (0.0f, 0.0f, -dialAngle);
			print ("Press SPACE TO RETRY" + " / Attempts Left: " + (3 - puzzleAttempts));
			if (Input.GetKeyDown (KeyCode.Space)) {
				currentState = lockState.findCombo1;
			}
		} else if (puzzleAttempts >= 3) {
			print ("Puzzle FAILED! DOOR LOCKED");
			DestroyLockSetup ();
			mainCam.enabled = true;
			lockMan.solveState = LockManager.lockState.failed;
//Upon doing this \/, the door CANNOT BE OPENED
			Destroy (this);
		}
	}


	void dialUnsolved () {
		camCom.enabled = false;
		mainCam.enabled = true;
		lockMan.solveState = LockManager.lockState.unsolved;
		this.enabled = false;
	}


	void DestroyLockSetup () {
		Destroy (myPlate);
		Destroy (myDial);
	}


	void DetermineRotDir () {
		if (thisDir == whichDir.right) {
			if (Input.GetAxis ("Horizontal") < 0.0f) {
				puzzleAttempts++;
				currentState = lockState.failed;
			}
		} else if (thisDir == whichDir.left) {
			if (Input.GetAxis ("Horizontal") > 0.0f) {
				puzzleAttempts++;
				currentState = lockState.failed;
			}
		}
	}
}
