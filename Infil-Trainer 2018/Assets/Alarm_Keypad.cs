using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm_Keypad : MonoBehaviour {

	AlarmManager alarmMan;

	enum padStatus {choosing, solved, failed, unsolved};
	padStatus padStat;


	void Awake () {
		alarmMan = gameObject.GetComponent<AlarmManager> ();

		SetNumbers ();
		SetupButtons ();
	}


	void OnEnable () {
		padStat = padStatus.choosing;
	}


	void Start () {
		
	}
	

	void Update () {
		if (padStat == padStatus.choosing) {
			ChooseButton ();
		} else if (padStat == padStatus.solved) {
			alarmMan.bStat = AlarmManager.boxStatus.solved;
		} else if (padStat == padStatus.failed) {
			alarmMan.bStat = AlarmManager.boxStatus.failed;
		} else if (padStat == padStatus.unsolved) {
			alarmMan.bStat = AlarmManager.boxStatus.unsolved;
		}
	}


	void SetNumbers () {

	}


	void SetupButtons () {

	}


	void ChooseButton () {

	}
}
