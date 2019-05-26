using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

	public GameObject canvas;
	[SerializeField] Text scoreText;
	[SerializeField] Text stopWatchText;
	float stopWatchTime;
	float minutes;
	float seconds;
	float milliseconds;
	public bool isTimerActive = true;

	[SerializeField] int score = 0;


	void Awake () {
		canvas = transform.Find("Canvas").gameObject;
	}


	void Start () {
		stopWatchTime = 0;
		stopWatchText.text = "00:00:00";
	}
	

	void Update () {
		if (isTimerActive) {
			IncrementStopWatch();
		}
	}


	void IncrementStopWatch() {
		stopWatchTime += Time.deltaTime;
		minutes = (int)(stopWatchTime / 60);
		seconds = (int)(stopWatchTime % 60);
		milliseconds = (int)((stopWatchTime * 60) % 60);

		stopWatchText.text = (minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00"));
	}


	public void AddToScore (int scoreIncrease){
		score += scoreIncrease;
		scoreText.text = ("$" + score);
	}
}
