﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_GlassCutter : MonoBehaviour {

	PuzzleManager puzzMan;
	Camera mainCam;

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
	public float crackTimer = -0.5f;


	void Awake () {
		puzzMan = gameObject.GetComponentInParent<PuzzleManager> ();
		mainCam = GameObject.FindWithTag ("MainCamera").GetComponent<Camera> ();

		GlassSetup ();
		GlassCamera ();
	}


	void OnEnable () {
		mainCam.enabled = false;
		camCom.enabled = true;
	}


	void Start () {
		
	}
	

	void Update () {
		Cutting ();
	}

	void GlassSetup () {
		Vector3 puzzleOffset = new Vector3 (0.0f, -100.0f, 0.0f);
		if (GameObject.Find ("GlassPane") != null) {
			puzzleOffset.x += 5.0f;
		}

		glass = Instantiate (glass, puzzleOffset, Quaternion.identity, transform);
		glass.name = "GlassPane";

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
		camCom.CopyFrom (mainCam);
		camCom.transform.rotation = Quaternion.FromToRotation (mainCam.transform.rotation.eulerAngles, Vector3.zero);

		glassCam.transform.position = glass.transform.position + Vector3.back * 0.1f;
		glassCam.transform.parent = glass.transform;

		//mainCam.enabled = false;
		camCom.enabled = false;

		//camCom = glassCam.GetComponent<Camera> ();
		return camCom;
	}


	void Cutting () {
		if (glassCam.GetComponent<Camera> () != null && Input.mousePosition != null) {
			Ray cutter = glassCam.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit cutHit;

			if (Physics.Raycast (cutter, out cutHit, 10)) {
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
				crackTimer = -0.5f;
			}
		}

		//Leave the puzzle "unsolved" for now, and allow the player to return later (DO NOT DESTROY EVERYTHING)
		if (Input.GetKeyDown(KeyCode.Q)) {
			GlassUnsolved ();
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
		mainCam.enabled = true;
		puzzMan.solveState = PuzzleManager.puzzleState.solved;
		Destroy (this);
	}


	void GlassFail () {
		mainCam.enabled = true;
		puzzMan.solveState = PuzzleManager.puzzleState.failed;
		Destroy (this);
	}


	void GlassUnsolved () {
		camCom.enabled = false;
		mainCam.enabled = true;
		puzzMan.solveState = PuzzleManager.puzzleState.unsolved;
		this.enabled = false;
	}
}
