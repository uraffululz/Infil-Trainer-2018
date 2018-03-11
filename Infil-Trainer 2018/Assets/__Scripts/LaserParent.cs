using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserParent : MonoBehaviour {

	RoomBuilder roomBuild;
	int rW;
	int rD;
	float rH;

	GameObject nodeParent;
	GameObject receiverParent;
	GameObject beamParent;

	[SerializeField] GameObject node;
	[SerializeField] GameObject receiver;
	[SerializeField] GameObject beam;

	public int spawnCount;
	Vector3[] spawnPoints;
	public List<Vector3> nodeOnBounds;
	List<int> nodeInts;
	public List<Vector3> recOnBounds;
	List<int> recInts;

	List<GameObject> Nodes;
	List<GameObject> Receivers;
	List<GameObject> Beams;

	//Laser Timer Countdown Variables
	public float laserTimer = 30.0f;
	public enum TimerOn {timerDeactivated, timerActivated};
	public TimerOn timerState;



	void Awake () {
		//Just initializing and populating this here so it can be used BEFORE it's populated in SpawnPos(), since it's a fixed size (6) anyway
		spawnPoints = new Vector3[]{Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};

		Nodes = new List<GameObject> ();
		Receivers = new List<GameObject> ();
		Beams = new List<GameObject> ();
		SetRoomBounds ();

		timerState = TimerOn.timerDeactivated;
	}

	void Start () {
		SpawnParents ();
		SpawnNodes ();
		SpawnReceivers ();
		SpawnBeams ();
	}
	

	void Update () {
		TimerCountdown ();
	}


	void SetRoomBounds () {
		roomBuild = GameObject.Find ("LevelManager").GetComponent<RoomBuilder> ();

		rW = roomBuild.roomWidth;
		rD = roomBuild.roomDepth;
		rH = roomBuild.roomHeight;
		spawnCount = (rW * rD) / 2;

		nodeInts = new List<int> ();
		recInts = new List<int> ();

		nodeOnBounds = new List<Vector3> ();
		recOnBounds = new List<Vector3> ();

		for (int i = 0; i < spawnCount; i++) {
			/*int chosenNodeInt = Random.Range (0, spawnPoints.Length);
			nodeInts.Add (chosenNodeInt);
			Vector3 chosenNodePos = SpawnPos (chosenNodeInt);
			nodeOnBounds.Add (chosenNodePos);

			int chosenRecInt = Random.Range (0, spawnPoints.Length);
			Vector3 chosenRecPos = SpawnPos (chosenRecInt);
			recInts.Add (chosenRecInt);
			recOnBounds.Add (chosenRecPos);
			*/

/*These lines \/ are a condensed version of ^This Shit. I'm just keeping those lines above in case I just fucked something up
* If no errors are called on this section of code, then it worked, and I can eventually delete ^This Shit
*/
			nodeInts.Add (Random.Range (0, spawnPoints.Length));
			nodeOnBounds.Add (SpawnPos (nodeInts[i]));

			recInts.Add (Random.Range (0, spawnPoints.Length));
			recOnBounds.Add (SpawnPos (recInts[i]));
		}
	}


	void SpawnParents () {
		nodeParent = new GameObject ();
		nodeParent.name = "NodeParent";
		nodeParent.transform.parent = gameObject.transform;

		receiverParent = new GameObject ();
		receiverParent.name = "ReceiverParent";
		receiverParent.transform.parent = gameObject.transform;

		beamParent = new GameObject ();
		beamParent.name = "BeamParent";
		beamParent.transform.parent = gameObject.transform;

	}


	void SpawnNodes () {
		for (int i = 0; i < spawnCount; i++) {
			node = Instantiate (node, nodeOnBounds[i], Quaternion.identity, nodeParent.transform);
			Nodes.Add (node);
		}
	}


	void SpawnReceivers ()
	{
		for (int i = 0; i < spawnCount; i++) {
			int newRecPoint;

			if (recInts[i] == nodeInts[i]) {
				print ("Node and receiver on same surface. Re-rolling..." + recInts[i]);
				if (recInts[i] <= 0) {
					newRecPoint = Random.Range (1, spawnPoints.Length);
				} else if (recInts[i] >= spawnPoints.Length - 1) {
					newRecPoint = Random.Range (0, spawnPoints.Length - 1);
				} else {
					newRecPoint = recInts[i]+1;
				}
				recOnBounds [i] = SpawnPos(newRecPoint);
			}
/*This \/ may be DONE*/
/*TODO This \/ will still make the new receiver overlap a previously spawned receiver (as the newRecPoint doesn't equal a totally new random spawnPoint),
* but for now it's better than nothing
*/
			receiver = Instantiate (receiver, recOnBounds[i], Quaternion.identity, receiverParent.transform);
			Receivers.Add (receiver);
			receiver.GetComponent<MeshRenderer> ().material.color = Color.black;
		}
		nodeOnBounds.Clear ();
	}


	void SpawnBeams () {
		for (int l = 0; l < spawnCount; l++) {
			float laserScaleTotal = (Receivers [l].transform.position - Nodes [l].transform.position).magnitude;
			Vector3 laserPos = (Nodes [l].transform.position + Receivers [l].transform.position) / 2;

			beam = Instantiate (beam, laserPos, Quaternion.identity, beamParent.transform);
			Beams.Add (beam);
			beam.gameObject.transform.localScale = new Vector3 (0.01f, 0.01f, laserScaleTotal);
			beam.gameObject.transform.rotation = Quaternion.LookRotation (beam.transform.position - Receivers [l].transform.position);
		}
	}


	void TimerCountdown () {
		if (timerState == LaserParent.TimerOn.timerActivated) {
			laserTimer -= Time.deltaTime;
			if (laserTimer <= 0.0f) {
				print ("Time ran out. Game over");
			}
		}
	}


	public Vector3 SpawnPos (int spawnPointInt) {
		float posOffset = 0.5f;
		spawnPoints = new Vector3[] {
			/*floor*/new Vector3 (Random.Range (0.0f, (float)rW - posOffset), 0.0f, Random.Range (0.0f, (float)rD - posOffset)),
			/*-xWall*/new Vector3 (-posOffset, Random.Range (0.0f, rH), Random.Range (0.0f, rD - posOffset)),
			/*xWall*/new Vector3 (rW - posOffset, Random.Range (0.0f, rH), Random.Range (0.0f, rD - posOffset)),
			/*-zWall*/new Vector3 (Random.Range (0.0f, rW - posOffset), Random.Range (0.0f, rH), -posOffset),
			/*zWall*/new Vector3 (Random.Range (0.0f, rW - posOffset), Random.Range (0.0f, rH), rD - posOffset),
			/*ceiling*/new Vector3 (Random.Range (-posOffset, (float)rW - posOffset), rH, Random.Range (0.0f, (float)rD - posOffset))
		};
		Vector3 spawnPosition = spawnPoints[spawnPointInt];
		return spawnPosition;
	}
}
