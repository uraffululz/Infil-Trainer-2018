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


	void Awake () {
		sTrack = GameObject.Find ("LevelManager").GetComponent<ScoreTracker> ();
		pMove = GameObject.Find ("Player").GetComponent<PlayerMove> ();

	}


	void OnEnable () {
		solveState = puzzleState.inProgress;

		//Get the current state of the Laser Countdown Timer (to return to later),
		//then deactivate it while the player tries to solve the puzzle
		lasPar = GameObject.Find ("LaserParent").GetComponent<LaserParent> ();
		isTimerActive = lasPar.timerState.ToString();
		lasPar.timerState = LaserParent.TimerOn.timerDeactivated;
		//print (isTimerActive);//This is just the last part, either "timerActivated" or "timerDeactivated"
	}


	void Start () {
		//print ("Starting puzzle sequence"); //Just wanted to make sure that Start() is IN FACT only called ONCE EVER

/*TODO 	Spawn a prefab of the chosen puzzle (Glass-cutter/Pressure-plate)L
 * Make a (GameObject[] whichPuzzle {glassCutter, pressurePlate}) and set up the parents/prefabs.
 * Move it out of view of the level entirely (Vector3.down * 100, or something like that).
 * Spawn a camera down near the puzzle.
*/




/*Began method used by www.github.com/radichc/Lockpick_Unity (searched Google for "Unity fallout lockpick", top result on Reddit),
 * but I really don't know what a lot of this means. Going to try something else for now
 * 
		RenderTexture background = new RenderTexture (Screen.width, Screen.height, 16);
		Camera currentCam = Camera.main;

		int mask = currentCam.cullingMask;
		int UILayerBitMask = 1 << LayerMask.NameToLayer ("UI");
		currentCam.cullingMask = ~0 & mask ^ UILayerBitMask;
		currentCam.targetTexture = background;
		currentCam.Render ();
		currentCam.cullingMask = mask;
		currentCam.targetTexture = null;
*/
	}
	

	void Update () {
		//While the player is trying to solve the puzzle...
		if (solveState == puzzleState.inProgress) {
			//Stop the player moving (because I may be using the same inputs)
			pMove.allowMove = false;
		} else {
			if (solveState == puzzleState.unsolved) {
				print ("Puzzle Left Unsolved");
				this.enabled = false;
			} else if (solveState == puzzleState.solved) {
				sTrack.score += 1000;
				Destroy (this);
			} else if (solveState == puzzleState.failed) {
//TODO If the player fails the puzzle, does the Laser Countdown Timer activate (regardless of its previous state)?
				Destroy (this);
			}
			pMove.allowMove = true;

			if (isTimerActive == "timerActivated") {
				lasPar.timerState = LaserParent.TimerOn.timerActivated;
			}
		}
			

		//print ("Press Q to EXIT the puzzle");
		if (Input.GetKeyDown(KeyCode.Q)) {
/*This leaves the puzzle "unsolved" (and no longer "inProgress" for now). Later on, there will be alternate states for: */
			solveState = puzzleState.unsolved;
		} else if (Input.GetKeyDown(KeyCode.R)) {
/* "solved", meaning the player succeeded in solving the puzzle within the allotted number of attempts */
			solveState = puzzleState.solved;
		} else if (Input.GetKeyDown(KeyCode.F)) {
/* "failed", meaning the player failed to solve the puzzle within the allotted number of attempts */
			solveState = puzzleState.failed;
		}
	}
}
