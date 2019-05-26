using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinBox : MonoBehaviour {

	LevelManager levMan;


	void Start () {
		levMan = GameObject.Find("LevelManager").GetComponent<LevelManager>();
	}


	void Update () {
		
	}


	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			levMan.Win();
		}
	}
}
