using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserParent : MonoBehaviour {
	
	//GameObject Component References
	RoomBuilder roomBuild;
	int rW;
	int rD;
	float rH;
	RoomFiller roomFill;

	//MakeParent() Objects
	GameObject nodeParent;
	GameObject receiverParent;
	GameObject beamParent;

	//Prefab References
	[SerializeField] GameObject node;
	[SerializeField] GameObject receiver;
	[SerializeField] GameObject beam;

	//Laser Spawn-Point Variables and Lists
	public int spawnCount;
	Vector3[] spawnPoints = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
	List<int> nodeInts = new List<int>();
	public List<Vector3> nodeSpawns = new List<Vector3>();
	List<int> recInts = new List<int>();
	public List<Vector3> recSpawns = new List<Vector3>();

	//Laser GameObject Lists
//TODO Can be made private later, as I don't think they're referenced anywhere outside this script. Just Serialize them, then.
	public List<GameObject> Nodes = new List<GameObject>();
	public List<GameObject> Receivers = new List<GameObject>();
	public List<GameObject> Beams = new List<GameObject>();

	//Laser Countdown Timer Variables
	public float laserTimer = 30.0f;
	public enum TimerOn {timerDeactivated, timerActivated};
	public TimerOn timerState;
	bool colorLerpToRed = false;

	GameObject[] lights;


	void Awake () {
		lights = GameObject.FindGameObjectsWithTag ("Light");

		MakeParents ();
		GetSpawnDimensions ();
		SetSpawnPoints ();

		timerState = TimerOn.timerDeactivated;
	}


	void Start () {
		/*if (roomBuild.buildProgress == RoomBuilder.BuildingStates.building) {
			SpawnNodes ();
			SpawnReceivers ();
			SpawnBeams ();
		}*/
	}
	

	void Update () {
		if (roomBuild.buildProgress == RoomBuilder.BuildingStates.building) {
			SpawnNodes ();
			SpawnReceivers ();
			SpawnBeams ();
		} else {
			TimerCountdown ();
		}
	}


	void MakeParents () {
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


	void GetSpawnDimensions() {
		roomBuild = GameObject.Find ("LevelManager").GetComponent<RoomBuilder> ();

		rW = roomBuild.roomWidth;
		rD = roomBuild.roomDepth;
		rH = roomBuild.roomHeight;

		spawnCount = (rW * rD) / 2;

		roomFill = GameObject.Find ("LevelManager").GetComponent<RoomFiller>();
	}
		

	void SetSpawnPoints () {
		print ("Spawn count: " + spawnCount);

		for (int i = 0; i < spawnCount; i++) {
			nodeInts.Add (Random.Range (0, spawnPoints.Length));
			nodeSpawns.Add (SpawnPos (nodeInts [i]));

			recInts.Add (Random.Range (0, spawnPoints.Length));
			recSpawns.Add (SpawnPos (recInts [i]));
		}
	}


	void SpawnNodes () {
		for (int i = 0; i < spawnCount; i++) {
			print (i + " / " + nodeSpawns.Count);
			node = Instantiate (node, nodeSpawns[i], Quaternion.identity, nodeParent.transform);
			Nodes.Add (node);
		}
	}


	void SpawnReceivers ()
	{
		for (int i = 0; i < spawnCount; i++) {
			int newRecPoint;

			if (recInts[i] == nodeInts[i]) {
				//print ("Node and receiver on same surface. Re-rolling..." + recInts[i]);
				if (recInts[i] == 0) {
					newRecPoint = Random.Range (1, spawnPoints.Length);
				} else if (recInts[i] == spawnPoints.Length - 1) {
					newRecPoint = Random.Range (0, spawnPoints.Length - 1);
				} else {
/*TODO This \/ newRecPoint isn't really randomized. It is derivative and incremental.
 * Make it choose again, by making a List<int> of values between 0-5. Then, remove the value that matches recInts[i]
 * Then choose a random int from the remaining list values.
 */	
					newRecPoint = recInts[i] + 1;
				}
				recSpawns [i] = SpawnPos(newRecPoint);
			}
			receiver = Instantiate (receiver, recSpawns[i], Quaternion.identity, receiverParent.transform);
			Receivers.Add (receiver);
			receiver.GetComponent<MeshRenderer> ().material.color = Color.black;

			//Rotate paired Node and Receiver to "look" at each other
			Nodes [i].transform.LookAt (Receivers [i].transform.position);
			Receivers [i].transform.LookAt (Nodes [i].transform.position);
		}
		nodeInts.Clear ();
		recInts.Clear ();
		nodeSpawns.Clear ();
		recSpawns.Clear ();
	}


	void SpawnBeams () {
		int respawnCount = 0;

		for (int l = 0; l < spawnCount; l++) {
			float laserScaleTotal = (Receivers [0].transform.position - Nodes [0].transform.position).magnitude;
			Vector3 laserPos = (Nodes [0].transform.position + Receivers [0].transform.position) / 2;

			beam = Instantiate (beam, laserPos, Quaternion.identity, beamParent.transform);
			Beams.Add (beam);
			beam.gameObject.transform.localScale = new Vector3 (0.01f, 0.01f, laserScaleTotal);
			beam.gameObject.transform.rotation = Quaternion.LookRotation (beam.transform.position - Receivers [0].transform.position);

			bool beamBlocked = false;

			foreach (var blocker in roomFill.beamBlockers) {
				if (beamBlocked == false &&
				    blocker.GetComponent<Collider> ().bounds.Intersects (beam.GetComponent<BoxCollider> ().bounds)) {

					beamBlocked = true;
					print ("Laser beam hit " + blocker.name + ". Respawning...");
					Destroy (Nodes [0]);
					Destroy (Receivers [0]);
					Destroy (Beams [0]);

//TODO I may not need these\/ lines. They may just be leftovers. If any errors come up which may be related to their exclusion, I'll reconsider
					//Nodes.Add (node);
					//Receivers.Add (receiver);
					//Beams.Add (beam);

					respawnCount++;
				}
			}
			Nodes.RemoveAt (0);
			Receivers.RemoveAt (0);
			Beams.RemoveAt (0);
		}
		spawnCount = respawnCount;
		respawnCount = 0;

		if (spawnCount > 0) {
			Nodes.Clear ();
			Receivers.Clear ();
			Beams.Clear ();

			SpawnRetry ();
		} else {
			roomBuild.buildProgress = RoomBuilder.BuildingStates.done;
			print ("Room is done building");
		}
	}


	void SpawnRetry () {
/* TODO Could I, both here and above, have each method call the next progressively?
 * SetSpawnPoints() calls SpawnNodes(), which, during its "for loop", passes in the int "i" to SpawnReceivers(), etc.?
 * Would that actually save me any effort, time, or space?
 * If it doesn't hit any "out of array index" or "reference exception" errors, it could work.
 * I tried, and it did have errors, as it was trying to start one (or more) step(s) without finishing the previous step(s)
 * AT THE MOMENT it is still doing that, but it's not causing any errors.
 * It is still printing the "Beams respawned" line during and after 
 * (UPDATE: I think this was fixed by adjusting the Script Execution Order)
 * MAYBE, though, it would work as a Coroutine, which I might try later.
 * FOR NOW, THIS DEFINITELY WORKS
 */
		SetSpawnPoints ();
		//print ("Respawning Nodes, Receivers, and Beams");
		SpawnNodes ();
		SpawnReceivers ();
		SpawnBeams ();
	}


	void TimerCountdown () {
		if (timerState == TimerOn.timerActivated) {
			laserTimer -= Time.deltaTime;

//TOmaybeDO Lerp lights between Red and Black, instead of Red and White.
			foreach (var light in lights) {
				if (light.GetComponent<Light>().color.g > 0.9f) {
					colorLerpToRed = true;
				} else if (light.GetComponent<Light>().color.g < 0.1f) {
					colorLerpToRed = false;
				}

				if (colorLerpToRed) {
					light.GetComponent<Light> ().color = Color.Lerp (light.GetComponent<Light> ().color, Color.red, 1.0f * Time.deltaTime);
				} else {
					light.GetComponent<Light> ().color = Color.Lerp (light.GetComponent<Light> ().color, Color.white, 1.0f * Time.deltaTime);
				}	
/*				print ("RGB: " + light.GetComponent<Light> ().color.r + "/" +
					light.GetComponent<Light> ().color.g + "/" +
					light.GetComponent<Light> ().color.b);
*/
			}
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
