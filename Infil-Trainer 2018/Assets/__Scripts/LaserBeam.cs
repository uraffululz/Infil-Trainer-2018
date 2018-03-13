using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

	RoomBuilder roomBuild;
	LaserParent laserParent;


	void Awake () {
		roomBuild = GameObject.Find ("LevelManager").GetComponent<RoomBuilder> ();
		laserParent = gameObject.GetComponentInParent<LaserParent> ();
	}


	void Start () {
		
	}
	

	void Update () {
		
	}


	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			print ("You touched a laser");
			if (roomBuild.buildProgress == RoomBuilder.BuildingStates.done) {
				laserParent.timerState = LaserParent.TimerOn.timerActivated;
				print ("Timer Activated");
			}
		}
	}
}
