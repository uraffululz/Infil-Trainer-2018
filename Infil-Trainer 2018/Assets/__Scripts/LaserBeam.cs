using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

	LevelManager levelManager;
	MyRoomData roomData;
	//LaserManager laserMngr;

	bool laserActivated = false;


	void Awake () {
		levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
		roomData = transform.parent.GetComponentInParent<MyRoomData>();
		//laserMngr = transform.parent.GetComponentInParent<LaserManager>();
	}


	void Start () {
		
	}
	

	void Update () {
		
	}


	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			if (roomData.myBuildState == MyRoomData.myRoomBuildState.finished && laserActivated == false && LevelManager.timerState == LevelManager.TimerOn.timerDeactivated) {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
			}
		}
	}
}
