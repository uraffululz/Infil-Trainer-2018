using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {

	LevelManager levMan;
	CanvasManager cMan;
	AlarmManager alarmMan;

	PlayerMove pMove;

	string isTimerActive;

	public enum puzzleState {dormant, inProgress, unsolved, solved, failed};
	public puzzleState solveState;

	int puzzleInt;
	public enum whichPuzzle {glassCutter, pressurePlate};
	public whichPuzzle puzzleChoice;

	Puzzle_GlassCutter glassPuzz;
	public GameObject glassPane;
	public GameObject glassLine;

	GameObject myTreasure;
	int myWorth = 1000;


	void Awake () {
		levMan = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
		cMan = GameObject.Find ("CanvasManager").GetComponent<CanvasManager> ();

		//Choose which puzzle is attached to the Display Case
		puzzleInt = Random.Range (0, 1);
		if (puzzleInt == 0) {
			puzzleChoice = whichPuzzle.glassCutter;
			//Destroy(GetComponent<Puzzle_PressurePlate>();
		}/* else if (puzzleInt == 1) {
			puzzleChoice = whichPuzzle.pressurePlate;
			Destroy(GetComponent<Puzzle_GlassCutter>();
		}*/
	}


	void Start() {
		pMove = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
		alarmMan = transform.parent.parent.Find("AlarmBox").GetComponent<AlarmManager>();

		myTreasure = GetComponent<DisplayCaseTreasure>().selectedTreasure;
	}


	void OnEnable () {
		solveState = puzzleState.inProgress;

		//Get the current state of the Laser Countdown Timer (to hold as a variable to return to later),
		//then deactivate it while the player tries to solve the puzzle
		isTimerActive = LevelManager.timerState.ToString();
		LevelManager.timerState = LevelManager.TimerOn.timerDeactivated;

		if (puzzleChoice == whichPuzzle.glassCutter) {
			if (glassPuzz == null) {
				glassPuzz = gameObject.AddComponent<Puzzle_GlassCutter>();
			}
			else {
				glassPuzz.enabled = true;
			}
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
				//Remove this display case's treasure from the list of acquired treasures/pickups
				levMan.GetComponent<LevelBuilder>().levelTreasures.Remove(myTreasure);
				Destroy(myTreasure);
				cMan.AddToScore(myWorth);

				Destroy (this);
			} else if (solveState == puzzleState.failed) {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
				Destroy(this);
			}

			pMove.allowMove = true;

			if (isTimerActive == "timerActivated") {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
			}
		}
	}
}
