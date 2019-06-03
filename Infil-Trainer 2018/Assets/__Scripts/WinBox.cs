using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinBox : MonoBehaviour {

	LevelManager levMan;

	[SerializeField] List<GameObject> alarmBoxes;


	void Start () {
		levMan = GameObject.Find("LevelManager").GetComponent<LevelManager>();
	}


	void Update () {
		
	}


	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			CanvasManager.isTimerActive = false;
			if (CanvasManager.stopWatchTime/60 > CanvasManager.parMinutes) {
				LevelManager.levelTimeBelowPar = false;
			}

			FindAlarmBoxes();
			levMan.Win(); //Can I just make this a static function in the LevelManager, and just call it like that? This should be the only place it's called from anyway
		}
	}


	void FindAlarmBoxes() {
		alarmBoxes = new List<GameObject>(GameObject.FindGameObjectsWithTag("AlarmBox"));

		int boxesWithLasersRemaining = 0;

		foreach (GameObject box in alarmBoxes) {
			if (!box.GetComponent<AlarmManager>().lasersAlreadyDisabled) {
				boxesWithLasersRemaining++;
			}
		}

		if (boxesWithLasersRemaining > 0) {
			LevelManager.allLasersDisabled = false;
		}
		else {
			LevelManager.allLasersDisabled = true;
		}
	}
}
