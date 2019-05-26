using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmManager : MonoBehaviour {

	[SerializeField] LevelManager levMan;
	[SerializeField] MyRoomData myParentRoomData;
	PlayerMove pMove;
	public GameObject laserParent;

	public enum boxStatus {dormant, inProgress, solved, failed, unsolved};
	public boxStatus bStat;

	public bool lasersAlreadyDisabled = false;
	


	void Awake () {
		levMan = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		myParentRoomData = transform.parent.GetComponent<MyRoomData>();
		pMove = GameObject.FindWithTag("Player").GetComponent<PlayerMove> ();

		if (myParentRoomData.hasLasers) {
			laserParent = transform.parent.Find("LaserParent").gameObject;
		}
		else {
			lasersAlreadyDisabled = true;
		}
	}


	void OnEnable () {
		pMove.allowMove = false;
		bStat = boxStatus.inProgress;

		gameObject.GetComponent<Alarm_Keypad> ().enabled = true;
	}


	void Update () {
		if (bStat == boxStatus.inProgress) {
			LevelManager.timerState = LevelManager.TimerOn.timerDeactivated;
		} else {
			if (bStat == boxStatus.unsolved) {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
				this.enabled = false;
			} else if (bStat == boxStatus.solved) {
				levMan.DeactivateCurrentlyActiveTimer();
				if (laserParent != null && !lasersAlreadyDisabled) {
					Destroy(laserParent);
					lasersAlreadyDisabled = true;
				}
				this.enabled = false;
			}
			else if (bStat == boxStatus.failed) {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
				Destroy (this);
			}

			pMove.allowMove = true;
		}
	}
}
