using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_GlassCutter : MonoBehaviour {

	PuzzleManager puzzMan;

	[SerializeField] GameObject glass;
	[SerializeField] GameObject line;
	int lineNum = 12;
	float segAngle = 30.0f;

	List<GameObject> lineSegments = new List<GameObject>() {};
	List<GameObject> cutSegments = new List<GameObject> () {};
	List<GameObject> crackedSegments = new List<GameObject> () {};

	GameObject glassCam;
	Camera camCom;

	public float cutTimer = 0.5f;
	public float crackTimer = 1.0f;


	void Awake () {
		puzzMan = gameObject.GetComponentInParent<PuzzleManager> ();
	}


	void Start () {
		GlassSetup ();
		GlassCamera ();
	}
	

	void Update () {
		Cutting ();
	}

	void GlassSetup () {
		glass = Instantiate (glass, new Vector3 (0.0f, -100.0f, 0.0f), Quaternion.identity, transform);

		for (int i = 0; i < lineNum; i++) {
			line = Instantiate (line, glass.transform.position + (Vector3.up * 0.045f), Quaternion.identity, glass.transform);
			line.name = "GlassLine";
			lineSegments.Add (line);
			line.transform.RotateAround (glass.transform.position, Vector3.forward, segAngle);
			segAngle += 30.0f;
		}
		print (lineSegments.Count);
	}


	Camera GlassCamera () {
		glassCam = new GameObject ();
		glassCam.name = "GlassCam";
		camCom = glassCam.AddComponent<Camera>();
		camCom.CopyFrom (Camera.main);
		camCom.transform.rotation = Quaternion.FromToRotation (Camera.main.transform.rotation.eulerAngles, Vector3.zero);

		glassCam.transform.position = glass.transform.position + Vector3.back * 0.1f;
		glassCam.transform.parent = glass.transform;

		//Camera.main.enabled = false;
		camCom.enabled = true;

		//camCom = glassCam.GetComponent<Camera> ();
		return camCom;
	}


	void Cutting () {
		if (glassCam.GetComponent<Camera> () != null && Input.mousePosition != null) {
			Ray cutter = glassCam.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			//print (Input.mousePosition);
			RaycastHit cutHit;

			if (Physics.Raycast (cutter, out cutHit, 10)) {
				//print ("Raycast hit something");
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
					if (cutTimer < -crackTimer) {
						cutSegments.Remove (cutHit.collider.gameObject);
						crackedSegments.Add (cutHit.collider.gameObject);
					} 
				}

				print (cutSegments.Count);

				if (crackedSegments.Count > 0) {
					print ("You fail");
					DestroyEverything ();
					GlassFail ();
				} else if (cutSegments.Count == lineNum && crackedSegments.Count == 0) {
					print ("You win");
					DestroyEverything ();
					GlassWin ();
				}
			} else {
				cutTimer = 0.5f;
				crackTimer = 1.0f;
			}
		}
	}


	void DestroyEverything() {
		foreach (var line in lineSegments) {
			Destroy (line);
		}
		foreach (var cutLine in cutSegments) {
			Destroy (cutLine);
		}
		foreach (var crackedLine in crackedSegments) {
			Destroy (crackedLine);
		}
		Destroy (glass);
		Destroy (camCom);
	}


	void GlassWin () {
		Camera.main.enabled = true;
		puzzMan.solveState = PuzzleManager.puzzleState.solved;
		Destroy (this);
	}


	void GlassFail () {
		//camCom.enabled = false;
		Camera.main.enabled = true;
		puzzMan.solveState = PuzzleManager.puzzleState.failed;
		Destroy (this);
	}
}
