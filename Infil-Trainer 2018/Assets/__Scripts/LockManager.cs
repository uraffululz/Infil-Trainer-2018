using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockManager : MonoBehaviour {

	ScoreTracker sTracker;

	PlayerMove pMove;

	LaserParent lasPar;
	string isTimerActive;

	public enum lockState {dormant, inProgress, unsolved, solved, failed};
	public lockState solveState; 

	int lockInt;
	public enum whichLock {rotaryDial, tumblerLock};
	public whichLock lockChoice;


	void Awake () {
		sTracker = GameObject.Find ("LevelManager").GetComponent<ScoreTracker> ();
		pMove = GameObject.Find ("Player").GetComponent<PlayerMove> ();

		//Choose which puzzle is attached to the Display Case
		lockInt = Random.Range (0, 1);
		if (lockInt == 0) {
			lockChoice = whichLock.rotaryDial;
		}
	}


	void OnEnable () {
		solveState = lockState.inProgress;

		//Get the current state of the Laser Countdown Timer (to return to later),
		//then deactivate it while the player tries to solve the puzzle
		lasPar = GameObject.Find ("LaserParent").GetComponent<LaserParent> ();
		isTimerActive = lasPar.timerState.ToString();
		lasPar.timerState = LaserParent.TimerOn.timerDeactivated;

		if (lockChoice == whichLock.rotaryDial) {
			gameObject.GetComponent<Lock_RotaryDial> ().enabled = true;

		} else if (lockChoice == whichLock.tumblerLock) {

		}
	}


	void Start () {
		
	}
	

	void Update () {
		//While the player is trying to solve the puzzle...
		if (solveState == lockState.inProgress) {
			//Stop the player moving (because I may be using the same inputs and/or moving the mouse)
			pMove.allowMove = false;
		} else {
			if (solveState == lockState.unsolved) {
				print ("Puzzle Left Unsolved");
				this.enabled = false;
			} else if (solveState == lockState.solved) {
				//Open the door
				gameObject.transform.rotation = 
					Quaternion.FromToRotation (transform.forward, transform.right * 90.0f);
				Destroy (this);
			} else if (solveState == lockState.failed) {
				lasPar.timerState = LaserParent.TimerOn.timerActivated;
				Destroy (this);
			}

			pMove.allowMove = true;

			if (isTimerActive == "timerActivated") {
				lasPar.timerState = LaserParent.TimerOn.timerActivated;
			}
		}
			if (Input.GetKeyDown(KeyCode.Q)) {
//This leaves the puzzle "unsolved" (and no longer "inProgress" for now). Later on, there will be alternate states for: 
					solveState = lockState.unsolved;
			} else if (Input.GetKeyDown(KeyCode.R)) {
//"solved", meaning the player succeeded in solving the puzzle within the allotted number of attempts 
				solveState = lockState.solved;
			} else if (Input.GetKeyDown(KeyCode.F)) {
//"failed", meaning the player failed to solve the puzzle within the allotted number of attempts 
				solveState = lockState.failed;
			}
	}
}
