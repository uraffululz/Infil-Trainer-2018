using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserParent : MonoBehaviour {

	RoomBuilder roomBuild;

	GameObject nodeParent;
	GameObject receiverParent;
	GameObject beamParent;

	[SerializeField] GameObject node;
	[SerializeField] GameObject receiver;
	[SerializeField] GameObject beam;

	public int spawnCount;
	Vector3[] spawnPoints;
	public List<Vector3> nodeOnBounds;
	List<int> nodePoints;
	public List<Vector3> recOnBounds;
	List<int> recPoints;

	List<GameObject> Nodes;
	List<GameObject> Receivers;
	List<GameObject> Beams;

	//Laser Timer Countdown Variables
	public float laserTimer = 30.0f;
	public enum TimerOn {timerDeactivated, timerActivated};
	public TimerOn timerState;



	void Awake () {
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
		spawnCount = (roomBuild.roomWidth * roomBuild.roomDepth) / 2;

		nodePoints = new List<int> ();
		recPoints = new List<int> ();

		nodeOnBounds = new List<Vector3> ();
		recOnBounds = new List<Vector3> ();

		for (int i = 0; i < spawnCount; i++) {
			spawnPoints = new Vector3[] {
				/*floor*/new Vector3 (Random.Range (0.0f, (float)roomBuild.roomWidth - 0.5f), 0.0f, Random.Range (0.0f, (float)roomBuild.roomDepth - 0.5f)),
				/*-xWall*/new Vector3 (-0.5f, Random.Range (0.0f, 3.0f), Random.Range (0.0f, roomBuild.roomDepth - 0.5f)),
				/*xWall*/new Vector3 (roomBuild.roomWidth - 0.5f, Random.Range (0.0f, 3.0f), Random.Range (0.0f, roomBuild.roomDepth - 0.5f)),
				/*-zWall*/new Vector3 (Random.Range (0.0f, roomBuild.roomWidth - 0.5f), Random.Range (0.0f, 3.0f), -0.5f),
				/*zWall*/new Vector3 (Random.Range (0.0f, roomBuild.roomWidth - 0.5f), Random.Range (0.0f, 3.0f), roomBuild.roomDepth - 0.5f),
				/*ceiling*/new Vector3 (Random.Range (-0.5f, (float)roomBuild.roomWidth - 0.5f), 3.0f, Random.Range (0.0f, (float)roomBuild.roomDepth - 0.5f))
			};

			int chosenNodePoint = Random.Range (0, spawnPoints.Length);
			nodePoints.Add (chosenNodePoint);
			nodeOnBounds.Add (spawnPoints[chosenNodePoint]);

			int chosenRecPoint = Random.Range (0, spawnPoints.Length);
			recPoints.Add (chosenRecPoint);
			recOnBounds.Add (spawnPoints [chosenRecPoint]);
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

			if (recPoints[i] == nodePoints[i]) {
				print ("Node and receiver on same surface. Re-rolling..." + recPoints[i]);
				if (recPoints[i] <= 0) {
					newRecPoint = Random.Range (1, spawnPoints.Length);
				} else if (recPoints[i] >= spawnPoints.Length - 1) {
					newRecPoint = Random.Range (0, spawnPoints.Length - 1);
				} else {
					newRecPoint = recPoints[i]+1;
				}
				recOnBounds [i] = spawnPoints [newRecPoint];
			}

/*TODO This \/ will still make the new receiver overlap a previously spawned node (as the newRecPoint doesn't equal a new random spawnPoint),
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
}
