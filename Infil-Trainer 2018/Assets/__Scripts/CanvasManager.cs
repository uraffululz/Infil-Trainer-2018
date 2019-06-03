using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

	GameObject levMan;

	public GameObject canvas;
	[SerializeField] Text scoreText;
	[SerializeField] Text stopWatchText;

	public static float stopWatchTime = 0f;
	string stopWatchDisplay;
	float minutes;
	float seconds;
	float milliseconds;
	public static bool isTimerActive = true;

	string parStopWatchDisplay;
	public static float parMinutes;
	float parSeconds = 00;
	float parMilliseconds = 00;

	public int score = 0;


	void Awake () {
		levMan = GameObject.Find("LevelManager");
		canvas = transform.Find("Canvas").gameObject;
	}


	void Start () {
		stopWatchTime = 0;
		stopWatchText.text = "00:00:00";

		parMinutes = levMan.GetComponent<LevelManager>().levelParTime * 2;
		parStopWatchDisplay = ("<color=red>" + parMinutes.ToString("00") + ":" + parSeconds.ToString("00") + ":" + parMilliseconds.ToString("00") + "</color>");
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

		stopWatchDisplay = (minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00"));

		stopWatchText.text = stopWatchDisplay + "<color=black><size=40> || </size></color>" + parStopWatchDisplay;
	}


	public void AddToScore (int scoreIncrease){
		score += scoreIncrease;
		scoreText.text = ("$" + score);
	}
}
