using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

	//Scene Object and Component References
	LevelManager levelManager;
	MyRoomData roomData;

	//Beam Variables
	bool laserActivated = false;


	void Awake () {
		//Initialize references
		levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
		roomData = transform.parent.GetComponentInParent<MyRoomData>();
	}


	void Start () {
		
	}
	

	void Update () {
		
	}


	void OnTriggerEnter (Collider other) {
		//If the game is ongoing, and the player touches this beam, start the level's alarm timer countdown
		if (other.gameObject.tag == "Player") {
			if (roomData.myBuildState == MyRoomData.myRoomBuildState.finished && laserActivated == false && LevelManager.timerState == LevelManager.TimerOn.timerDeactivated) {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
			}
		}
	}
}
