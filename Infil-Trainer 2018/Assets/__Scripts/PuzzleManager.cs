using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {

	ScoreTracker sTrack;

	PlayerMove pMove;

	LaserParent lasPar;
	string isTimerActive;

	public enum puzzleState {dormant, inProgress, unsolved, solved, failed};
	public puzzleState solveState;

	int puzzleInt;
	public enum whichPuzzle {glassCutter, pressurePlate};
	public whichPuzzle puzzleChoice;


	void Awake () {
		sTrack = GameObject.Find ("LevelManager").GetComponent<ScoreTracker> ();
		pMove = GameObject.Find ("Player").GetComponent<PlayerMove> ();

		//Choose which puzzle is attached to the Display Case
		puzzleInt = Random.Range (0, 1);
		if (puzzleInt == 0) {
			puzzleChoice = whichPuzzle.glassCutter;
		}/* else if (puzzleInt == 1) {
			puzzleChoice = whichPuzzle.pressurePlate;
		}*/
	}


	void OnEnable () {
		solveState = puzzleState.inProgress;

		//Get the current state of the Laser Countdown Timer (to return to later),
		//then deactivate it while the player tries to solve the puzzle
		lasPar = GameObject.Find ("LaserParent").GetComponent<LaserParent> ();
		isTimerActive = lasPar.timerState.ToString();
		lasPar.timerState = LaserParent.TimerOn.timerDeactivated;
		//print (isTimerActive);//This is just the last part, either "timerActivated" or "timerDeactivated"

		if (puzzleChoice == whichPuzzle.glassCutter) {
			gameObject.GetComponent<Puzzle_GlassCutter> ().enabled = true;

		} else if (puzzleChoice == whichPuzzle.pressurePlate) {

		}
	}


	void Start () {
		
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
				sTrack.score += 1000;
				Destroy (this);
			} else if (solveState == puzzleState.failed) {
				lasPar.timerState = LaserParent.TimerOn.timerActivated;
				Destroy (this);
			}

			pMove.allowMove = true;

			if (isTimerActive == "timerActivated") {
				lasPar.timerState = LaserParent.TimerOn.timerActivated;
			}
		}
			
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
