using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockManager : MonoBehaviour {

	LevelManager levMan;
	CanvasManager cMan;

	PlayerMove pMove;

	LaserManager lasMan;
	string isTimerActive;

	public enum lockState {dormant, inProgress, unsolved, solved, failed};
	public lockState solveState; 

	int lockInt;
	public enum whichLock {rotaryDial, tumblerLock};
	public whichLock lockChoice;

	//Rotary Dial Lock References
	public GameObject rotaryPlate;
	public GameObject rotaryDial;



	void Awake () {
		levMan = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		cMan = GameObject.Find("CanvasManager").GetComponent<CanvasManager> ();
		pMove = GameObject.FindWithTag("Player").GetComponent<PlayerMove> ();
		lasMan = GameObject.Find ("LaserParent").GetComponent<LaserManager> ();

		//Choose which puzzle is attached to the Display Case
		lockInt = Random.Range (0, 1); //Add a higher range as I add more lock puzzles
		if (lockInt == 0) {
			lockChoice = whichLock.rotaryDial;
		}
	}


	void OnEnable () {
		solveState = lockState.inProgress;

		//Get the current state of the Laser Countdown Timer (to return to later),
		//then deactivate it while the player tries to solve the puzzle
		isTimerActive = LevelManager.timerState.ToString();
		LevelManager.timerState = LevelManager.TimerOn.timerDeactivated;

		if (lockChoice == whichLock.rotaryDial) {
			gameObject.AddComponent<Lock_RotaryDial>();
			//gameObject.GetComponent<Lock_RotaryDial> ().enabled = true;

		} else if (lockChoice == whichLock.tumblerLock) {

		}
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
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
				Destroy (this);
			}

			pMove.allowMove = true;

			if (isTimerActive == "timerActivated") {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
			}
		}
	}
}
