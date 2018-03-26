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

		gameObject.GetComponent<Alarm_Keypad> ().enabled = true;
	}


	void Start () {
		
	}
	

	void Update () {
		if (bStat == boxStatus.inProgress) {
			laserParent.GetComponent<LaserParent> ().timerState = LaserParent.TimerOn.timerDeactivated;

		} else {
			if (bStat == boxStatus.unsolved) {
//TOmaybeDO Start the countdown timer?
				this.enabled = false;
			} else if (bStat == boxStatus.solved) {
				laserParent.GetComponent<LaserParent> ().timerState = LaserParent.TimerOn.timerDeactivated;
				Destroy (laserParent);
				Destroy (this);
			} else if (bStat == boxStatus.failed) {
				laserParent.GetComponent<LaserParent> ().timerState = LaserParent.TimerOn.timerActivated;
				Destroy (this);
			}

			pMove.allowMove = true;
		}
	}
}
