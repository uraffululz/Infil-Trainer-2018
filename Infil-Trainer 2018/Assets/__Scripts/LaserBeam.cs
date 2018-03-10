using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

	LaserParent laserParent;


	void Awake () {
		laserParent = gameObject.GetComponentInParent<LaserParent> ();
	}


	void Start () {
		
	}
	

	void Update () {
		
	}


	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			print ("You touched a laser");
			laserParent.timerState = LaserParent.TimerOn.timerActivated;
		}
	}
}
