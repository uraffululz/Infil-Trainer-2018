using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {

	LevelManager levMan;
	CanvasManager cMan;
	AlarmManager alarmMan;

	PlayerMove pMove;

	//LaserManager lasPar;
	string isTimerActive;

	public enum puzzleState {dormant, inProgress, unsolved, solved, failed};
	public puzzleState solveState;

	int puzzleInt;
	public enum whichPuzzle {glassCutter, pressurePlate};
	public whichPuzzle puzzleChoice;

	int myWorth = 1000;


	void Awake () {
		levMan = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
		cMan = GameObject.Find ("CanvasManager").GetComponent<CanvasManager> ();

		//lasPar = transform.parent.GetComponent<LaserManager>();

		//Choose which puzzle is attached to the Display Case
		puzzleInt = Random.Range (0, 1);
		if (puzzleInt == 0) {
			puzzleChoice = whichPuzzle.glassCutter;
		}/* else if (puzzleInt == 1) {
			puzzleChoice = whichPuzzle.pressurePlate;
		}*/
	}


	void Start() {
		pMove = GameObject.Find("Player").GetComponent<PlayerMove>();
		alarmMan = transform.parent.parent.Find("AlarmBox").GetComponent<AlarmManager>();
	}


	void OnEnable () {
		solveState = puzzleState.inProgress;

		//Get the current state of the Laser Countdown Timer (to return to later),
		//then deactivate it while the player tries to solve the puzzle
		isTimerActive = LevelManager.timerState.ToString();
		LevelManager.timerState = LevelManager.TimerOn.timerDeactivated;
		//print (isTimerActive);//This is just the last part, either "timerActivated" or "timerDeactivated"

		if (puzzleChoice == whichPuzzle.glassCutter) {
			gameObject.GetComponent<Puzzle_GlassCutter> ().enabled = true;
		} else if (puzzleChoice == whichPuzzle.pressurePlate) {
			//gameObject.GetComponent<Puzzle_PressurePlate> ().enabled = true;
		}
	}


	void Update () {
		//While the player is trying to solve the puzzle...
		if (solveState == puzzleState.inProgress) {
			//Stop the player moving (because I may be using the same inputs and/or moving the mouse)
			pMove.allowMove = false;
		} else {
			if (solveState == puzzleState.unsolved) {
				print ("Puzzle Left Unsolved");
				this.enabled = false;
			} else if (solveState == puzzleState.solved) {
				cMan.AddToScore(myWorth);
				//alarmMan.alarmsLeft--;
				//alarmMan.AreAllAlarmsDisabled();
				Destroy (this);
			} else if (solveState == puzzleState.failed) {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
				//alarmMan.alarmsLeft--;
				//alarmMan.AreAllAlarmsDisabled();
				Destroy(this);
			}

			pMove.allowMove = true;

			if (isTimerActive == "timerActivated") {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
			}
		}
	

//After I add the other puzzle(s) and get them working, I can delete this part
/*
		//print ("Press Q to EXIT the puzzle");
		if (Input.GetKeyDown(KeyCode.Q)) {
This leaves the puzzle "unsolved" (and no longer "inProgress" for now). Later on, there will be alternate states for: 
			solveState = puzzleState.unsolved;
		} else if (Input.GetKeyDown(KeyCode.R)) {
 "solved", meaning the player succeeded in solving the puzzle within the allotted number of attempts 
			solveState = puzzleState.solved;
		} else if (Input.GetKeyDown(KeyCode.F)) {
 "failed", meaning the player failed to solve the puzzle within the allotted number of attempts 
			solveState = puzzleState.failed;
		}
*/
	}
}
