using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_GlassCutter : MonoBehaviour {

	PuzzleManager puzzMan;

	[SerializeField] GameObject glass;
	[SerializeField] GameObject line;
	int lineNum = 12;
	float segAngle = 30.0f;

	List<GameObject> lineSegments = new List<GameObject>();

	float cutTimer = 1.0f;
	float crackTimer = 2.0f;


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
			lineSegments.Add (line);
			line.transform.RotateAround (glass.transform.position, Vector3.forward, segAngle);
			segAngle += 30.0f;
		}
	}


	void GlassCamera () {
		GameObject glassCam = new GameObject ();
		Camera camCom = glassCam.AddComponent<Camera>();
		camCom.CopyFrom (Camera.main);

		glassCam.transform.position = glass.transform.position + Vector3.back * 0.1f;
		glassCam.transform.parent = glass.transform;

	}


	void Cutting () {
		
	}
}
