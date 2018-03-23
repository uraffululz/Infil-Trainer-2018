using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock_RotaryDial : MonoBehaviour {

	LockManager lockMan;
	Camera mainCam;

	[SerializeField] GameObject plate;
	[SerializeField] GameObject dial;

	enum whichDir {right, left};
	whichDir thisDir;

	GameObject rotCam;
	Camera camCom;

	public int dialNum1;
	public int dialNum2;
	public int dialNum3;
	//bool comboNum1Reached = false;
	//bool comboNum2Reached = false;
	//bool comboNum3Reached = false;

	int dialRotSpeed = 15;
	public float dialAngle = 0.0f;

	float comboStayTimer = 2.0f;
	int puzzleAttempts = 0;

	public enum lockState {findCombo1, findCombo2, findCombo3, unlocked, failed, unsolved};
	public lockState currentState;


	void Awake () {
		lockMan = gameObject.GetComponentInParent<LockManager> ();
		mainCam = GameObject.FindWithTag ("MainCamera").GetComponent<Camera> ();

		RotarySetup ();
		NumberSetup ();
		RotaryCamera ();
	}


	void OnEnable () {
		mainCam.enabled = false;
		camCom.enabled = true;

		//Reset the lock to the first position
		//comboNum1Reached = false;
		//comboNum2Reached = false;
		//comboNum3Reached = false;
		currentState = lockState.findCombo1;
	}


	void Start () {
		
	}
	

	void Update () {
		RotateDial ();

		if (Input.GetKeyDown(KeyCode.Q)) {
			//This leaves the puzzle "unsolved" (and no longer "inProgress" for now). Later on, there will be alternate states for: 
			currentState = lockState.unsolved;
		} else if (Input.GetKeyDown(KeyCode.R)) {
			//"solved", meaning the player succeeded in solving the puzzle within the allotted number of attempts 
			currentState = lockState.unlocked;
		} else if (Input.GetKeyDown(KeyCode.F)) {
			//"failed", meaning the player failed to solve the puzzle within the allotted number of attempts 
			currentState = lockState.failed;
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
		Vector3 lockOffset = new Vector3 (0.0f, -200.0f, 0.0f);
		if (GameObject.Find ("BackPlate") != null) {
			lockOffset.x += 5.0f;
			lockOffset.y -= 10.0f * puzzleAttempts;
		}

		plate = Instantiate (plate, lockOffset, Quaternion.identity, transform);
		plate.name = "BackPlate";

		dial = Instantiate (dial, lockOffset + Vector3.right * 0.2f, Quaternion.identity, plate.transform);
		dial.name = "RotaryDial";
	}


	void NumberSetup () {
		dialNum1 = Random.Range (10, 350);
		dialNum2 = Random.Range (10, 350);
		dialNum3 = Random.Range (10, 350);

/*TODO Add the 3 selected numbers to an array/list?
 * 
*/
	}


	Camera RotaryCamera () {
		rotCam = new GameObject ();
		rotCam.name = "RotaryCam";
		camCom = rotCam.AddComponent<Camera>();
		camCom.CopyFrom (mainCam);
		camCom.transform.rotation = Quaternion.FromToRotation (mainCam.transform.rotation.eulerAngles, Vector3.zero);

		rotCam.transform.position = plate.transform.position + Vector3.back * 1.0f;
		rotCam.transform.parent = plate.transform;

		//mainCam.enabled = false;
		camCom.enabled = false;

		//camCom = glassCam.GetComponent<Camera> ();
		return camCom;
	}


	void RotateDial () {
		//Use input to rotate Dial
		dial.transform.Rotate (Vector3.back, Input.GetAxis ("Horizontal") * dialRotSpeed * Time.deltaTime);
		//Get Dial rotation angle #(between 0 and 360)
		dialAngle = dial.transform.rotation.eulerAngles.z;

/*TODO Use left-right keyboard arrow inputs to rotate lock (Start puzzle by rotating to the right)
 * If the correct number is landed on, indicate to the player that the lock should be rotated in the opposite direction
 * If the correct number is passed, or the player rotates the lock in the wrong direction, the puzzle attempt is failed
*/
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
			dialFailed ();
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
			dialFailed ();
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
			dialFailed ();
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
		puzzleAttempts++;
		print ("Dial angle: " + dialAngle + "/ Attempts Left: " + puzzleAttempts);
/**/
		if (puzzleAttempts < 3) {
//TODO Set up input for RETRY. For now, it's just automatic, and that leads to failures stacking quickly
			dial.transform.Rotate(0.0f, 0.0f, -dialAngle);
			//dialAngle = 0.0f;
			currentState = lockState.findCombo1;
		} else if (puzzleAttempts >= 3) {
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
		dial.transform.Rotate(0.0f, 0.0f, -dialAngle);
		lockMan.solveState = LockManager.lockState.unsolved;
		this.enabled = false;
	}


	void DestroyLockSetup () {
		Destroy (plate);
		Destroy (dial);
	}


	void DetermineRotDir () {
		if (thisDir == whichDir.right) {
			if (Input.GetAxis ("Horizontal") < 0.0f) {
				dialFailed ();
			}
		} else if (thisDir == whichDir.left) {
			if (Input.GetAxis ("Horizontal") > 0.0f) {
				dialFailed ();
			}
		}
	}
}
