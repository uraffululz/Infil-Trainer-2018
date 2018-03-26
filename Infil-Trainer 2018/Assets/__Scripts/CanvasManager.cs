using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

	public GameObject canvas;

	public int score = 0;


	void Awake () {
		canvas = Instantiate (canvas, gameObject.transform) as GameObject;
	}


	void Start () {
		
	}
	

	void Update () {
		
	}
}
