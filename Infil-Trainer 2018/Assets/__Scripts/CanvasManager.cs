using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

	public GameObject canvas;
	[SerializeField] Text scoreText;

	[SerializeField] int score = 0;


	void Awake () {
		canvas = transform.GetChild(0).gameObject;
	}


	void Start () {
		
	}
	

	void Update () {
		
	}


	public void AddToScore (int scoreIncrease){
		score += scoreIncrease;
		scoreText.text = ("Score: " + score);
	}
}
