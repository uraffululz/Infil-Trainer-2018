using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmManager : MonoBehaviour {

	public PlayerMove pMove;
	public GameObject laserParent;

	public enum boxStatus {dormant, inProgress, solved, failed, unsolved};
	public boxStatus bStat;


	void Awake () {
		pMove = GameObject.Find ("Player").GetComponent<PlayerMove> ();
		laserParent = GameObject.Find ("LaserParent");

	}


	void OnEnable () {
		pMove.allowMove = false;
		bStat = boxStatus.inProgress;
	}


	void Start () {
		
	}
	

	void Update () {
		if (bStat == boxStatus.inProgress) {
			
		} else if (bStat == boxStatus.solved) {
			
		} else if (bStat == boxStatus.failed) {
			
		} else if (bStat == boxStatus.unsolved) {
			
		}
	}
}
