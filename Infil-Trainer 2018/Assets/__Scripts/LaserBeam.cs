using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

	RoomBuilder roomBuild;
	LaserManager laserMngr;


	void Awake () {
		roomBuild = GameObject.Find ("LevelManager").GetComponent<RoomBuilder> ();
		laserMngr = gameObject.GetComponentInParent<LaserManager>();
	}


	void Start () {
		
	}
	

	void Update () {
		
	}


	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			//if (roomBuild.buildProgress == RoomBuilder.BuildingStates.done) {
			//	laserMngr.timerState = LaserParent.TimerOn.timerActivated;
				print ("Laser Countdown timer Activated");
			//}
		}
	}
}
