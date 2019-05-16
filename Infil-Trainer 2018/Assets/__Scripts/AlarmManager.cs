using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmManager : MonoBehaviour {

	[SerializeField] LevelManager levMan;
	[SerializeField] MyRoomData myParentRoomData;
	public PlayerMove pMove;
	public GameObject laserParent;

	public enum boxStatus {dormant, inProgress, solved, failed, unsolved};
	public boxStatus bStat;

	public bool lasersAlreadyDisabled = false;

	//public int alarmsLeft = 0;


	void Awake () {
		levMan = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		myParentRoomData = transform.parent.GetComponent<MyRoomData>();
		pMove = GameObject.Find ("Player").GetComponent<PlayerMove> ();
		laserParent = transform.parent.Find("LaserParent").gameObject;

		//foreach (var blocker in myParentRoomData.beamBlockers) {
		//	if (blocker.GetComponent<PuzzleManager>() != null) {
		//		alarmsLeft++;
		//	}
		//}
	}


	void Start() {
		
		
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
				if (!lasersAlreadyDisabled) {
					Destroy(laserParent);
					lasersAlreadyDisabled = true;
					this.enabled = false;
				}
				else {
					this.enabled = false;
				}

				//AreAllAlarmsDisabled();
			} else if (bStat == boxStatus.failed) {
				LevelManager.timerState = LevelManager.TimerOn.timerActivated;
				Destroy (this);
			}

			pMove.allowMove = true;
		}
	}


	//public void AreAllAlarmsDisabled() {
	//	if (alarmsLeft == 0 && LevelManager.timerState == LevelManager.TimerOn.timerDeactivated) {
	//		Destroy(this);
	//	}
	//}
}
