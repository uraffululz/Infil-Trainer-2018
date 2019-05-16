using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_GlassCutter : MonoBehaviour {

	PuzzleManager puzzMan;
	[SerializeField] Camera mainCam;

	[SerializeField] GameObject glass;
	[SerializeField] GameObject line;
	int lineNum = 12;
	float segAngle = 0.0f;

	public List<GameObject> lineSegments = new List<GameObject>() {};
	public List<GameObject> cutSegments = new List<GameObject> () {};
	public List<GameObject> crackedSegments = new List<GameObject> () {};

	GameObject glassCam;
	Camera camCom;

	public float cutTimer = 0.5f;
	public float crackTimer = -0.5f;

	int timesFailed = 0;

	enum puzzleState {cutting, solved, failed, unsolved};
	puzzleState puzzState;


	void Awake () {
		puzzMan = gameObject.GetComponentInParent<PuzzleManager> ();


		GlassSetup ();
		LineSetup ();
	}


	void OnEnable () {
		puzzState = puzzleState.cutting;

		mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		GlassCamera();
		mainCam.enabled = false;
		camCom.enabled = true;
	}


	void Start () {
		//mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		GlassCamera();

	}


	void Update () {
		if (puzzState == puzzleState.cutting) {
			Cutting ();
		} else if (puzzState == puzzleState.solved) {
			GlassWin ();
		} else if (puzzState == puzzleState.failed) {
			GlassFail ();
		} else if (puzzState == puzzleState.unsolved) {
			GlassUnsolved ();
		}

		//Leave the puzzle "unsolved" for now, and allow the player to return later (DO NOT DESTROY EVERYTHING)
		if (Input.GetKeyDown(KeyCode.Q)) {
			puzzState = puzzleState.unsolved;
		}
	}

	void GlassSetup () {
		Vector3 puzzleOffset = new Vector3 (0.0f, -100.0f, 0.0f);
		if (GameObject.Find ("GlassPane") != null) {
			//puzzleOffset.x += 5.0f;
			puzzleOffset.y -= 10.0f * timesFailed;
		}

		glass = Instantiate (glass, transform.position + puzzleOffset, Quaternion.identity, transform);
		glass.name = "GlassPane";
	}


	void LineSetup () {
		for (int i = 0; i < lineNum; i++) {
			line = Instantiate (line, glass.transform.position + (Vector3.up * 0.045f), Quaternion.identity, glass.transform);
			line.GetComponent<MeshRenderer> ().material.color = Color.gray;
			line.name = "GlassLine";
			lineSegments.Add (line);
			line.transform.RotateAround (glass.transform.position, Vector3.forward, segAngle);
			segAngle += 30.0f;
		}
		//print (lineSegments.Count);
	}


	Camera GlassCamera () {
		if (glassCam == null) {
			glassCam = new GameObject();
			glassCam.name = "GlassCam";
			camCom = glassCam.AddComponent<Camera>();
			camCom.CopyFrom(mainCam);
			camCom.transform.rotation = Quaternion.FromToRotation(mainCam.transform.rotation.eulerAngles, Vector3.zero);

			glassCam.transform.position = glass.transform.position + Vector3.back * 0.1f;
			glassCam.transform.parent = glass.transform;

			camCom.enabled = false;

			return camCom;
		}
		else {
			return camCom;
		}
		
	}


	void Cutting () {
		if (glassCam.GetComponent<Camera> () != null && Input.mousePosition != null) {
			Ray cutter = glassCam.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit cutHit;

			if (Physics.Raycast (cutter, out cutHit, 10)) {
				print (cutHit.collider.gameObject.name);
				if (cutHit.collider.gameObject.name == "GlassLine") {
					cutTimer -= 1.0f * Time.deltaTime;

					if (cutTimer <= 0.0f) {
						cutHit.collider.gameObject.GetComponent<MeshRenderer> ().material.color = Color.blue;
						lineSegments.Remove (cutHit.collider.gameObject);
						cutSegments.Add (cutHit.collider.gameObject);
						cutHit.collider.gameObject.name = "CutLine";

					}
				} else if (cutHit.collider.gameObject.name == "CutLine") {
					cutTimer -= 1.0f * Time.deltaTime;
					if (cutTimer < crackTimer) {
						cutSegments.Remove (cutHit.collider.gameObject);
						crackedSegments.Add (cutHit.collider.gameObject);
					} 
				}
					
				if (crackedSegments.Count > 0) {
					print ("You fail");
					puzzState = puzzleState.failed;
				} else if (cutSegments.Count == lineNum && crackedSegments.Count == 0) {
					print ("You win");
					puzzState = puzzleState.solved;
				}
			} else {
				cutTimer = 0.5f;
			}
		}
	}


	void DestroyLines() {
		foreach (var line in lineSegments) {
			Destroy (line);
		}
		foreach (var cutLine in cutSegments) {
			Destroy (cutLine);
		}
		foreach (var crackedLine in crackedSegments) {
			Destroy (crackedLine);
		}
			
		lineSegments.Clear ();
		cutSegments.Clear();
		crackedSegments.Clear();
	}


	void GlassWin () {
		Destroy (glass);
		mainCam.enabled = true;
		puzzMan.solveState = PuzzleManager.puzzleState.solved;
		Destroy (this);
	}


	void GlassFail () {
		if (timesFailed < 2) {
			print ("Puzzle FAILED! Press SPACE TO RETRY");
			if (Input.GetKeyDown (KeyCode.Space)) {
				timesFailed++;
				DestroyLines ();
				ResetLines ();
				puzzState = puzzleState.cutting;
			}
		} else if (timesFailed >= 2) {
			DestroyLines ();
			Destroy (glass);
			mainCam.enabled = true;
			puzzMan.solveState = PuzzleManager.puzzleState.failed;
			Destroy (this);
		}
	}


	void GlassUnsolved () {
		camCom.enabled = false;
		mainCam.enabled = true;
		puzzMan.solveState = PuzzleManager.puzzleState.unsolved;
		this.enabled = false;
	}


	void ResetLines () {
		LineSetup();
		foreach (var line in lineSegments) {
			line.GetComponent<MeshRenderer> ().material.color = Color.gray;
		}

		mainCam.enabled = false;
		camCom.enabled = true;

		cutTimer = 0.5f;
	}
}
