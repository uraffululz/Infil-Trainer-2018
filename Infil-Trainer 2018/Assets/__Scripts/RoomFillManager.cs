using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFillManager : MonoBehaviour {

	//My Room Variables
	MyRoomData roomdata;

	GameObject player;

	[SerializeField] List<Vector3> fillerPositions = new List<Vector3>();

	GameObject caseParent;
	GameObject pickupParent;

	[SerializeField] GameObject displayCase;
	[SerializeField] GameObject alarmBox;

	[SerializeField] List<GameObject> pickupPrefabs;

	int caseCount = 2;
	int pickupCount = 2;


	void Awake() {
		roomdata = gameObject.GetComponent<MyRoomData>();

		SpawnFillerParents();

	}


	void Start () {
		player = GameObject.FindWithTag("Player");

		SpawnDisplayCases();
		SpawnPickups();
		SpawnAlarmBox();

	}


	void Update () {
		
	}


	void SpawnFillerParents() {
		caseParent = new GameObject("CaseParent");
		caseParent.transform.parent = transform;

		pickupParent = new GameObject("PickupParent");
		pickupParent.transform.parent = transform;
	}


	void SpawnDisplayCases() {
//TODO Add each spawned case to roomData.beamBlockers

		for (int c = 0; c < caseCount; c++) {
			bool spawnFuckedUp = false;
			Vector3 casePos = roomdata.myFloorTiles[Random.Range(0, roomdata.myFloorTiles.Count)].transform.position;
			
			GameObject newCase = Instantiate(displayCase, casePos, Quaternion.identity, caseParent.transform);

			if (fillerPositions.Contains(casePos)) { //Maybe change to a while loop. ugh.
				spawnFuckedUp = true;
			}
			else {
				foreach (GameObject blocker in roomdata.beamBlockers) {
					if (blocker.GetComponent<BoxCollider>().bounds.Intersects(newCase.GetComponent<BoxCollider>().bounds)) {
						spawnFuckedUp = true;
					}
				}
			}

			if (spawnFuckedUp) {
				Destroy(newCase);
				c--;
			}
			else {
				roomdata.beamBlockers.Add(newCase);
			}

			fillerPositions.Add(casePos);
		}
	}


	void SpawnAlarmBox () {
		for (int abc = 0; abc < 1; abc++) {
			bool spawnFuckedUp = false;
			GameObject myWall = roomdata.myWallTiles[Random.Range(0, roomdata.myWallTiles.Count)];
			Vector3 casePos = myWall.transform.position + (Vector3.up * 0.7f);

			GameObject myAlarmBox = Instantiate(alarmBox, casePos, myWall.transform.rotation, gameObject.transform);

			if (fillerPositions.Contains(casePos)) { //Maybe change to a while loop. ugh.
				spawnFuckedUp = true;
			}
			else {
				foreach (GameObject blocker in roomdata.beamBlockers) {
					if (blocker.GetComponent<BoxCollider>().bounds.Intersects(myAlarmBox.GetComponent<BoxCollider>().bounds)) {
						spawnFuckedUp = true;
					}
				}
			}

			if (spawnFuckedUp) {
				Destroy(myAlarmBox);
				abc--;
			}
			else {
				roomdata.beamBlockers.Add(myAlarmBox);
				myAlarmBox.name = "AlarmBox";
			}

			fillerPositions.Add(casePos);
		}
	}


	void SpawnPickups() {
		for (int p = 0; p < pickupCount; p++) {
			bool pickupSpawnFuckedUp = false;
			Vector3 pickupPos = roomdata.myFloorTiles[Random.Range(0, roomdata.myFloorTiles.Count)].transform.position;

			GameObject newPickup = Instantiate(pickupPrefabs[Random.Range(0, pickupPrefabs.Count)], pickupPos, Quaternion.identity, pickupParent.transform);

			if (fillerPositions.Contains(pickupPos)
							|| newPickup.GetComponent<Collider>().bounds.Intersects(player.GetComponent<CapsuleCollider>().bounds)) { //Maybe change to a while loop. ugh.
				pickupSpawnFuckedUp = true;
			}
			else {
				foreach (GameObject blocker in roomdata.beamBlockers) {
					if (blocker.GetComponent<BoxCollider>().bounds.Intersects(newPickup.GetComponent<Collider>().bounds)) {
						pickupSpawnFuckedUp = true;
					}
				}
			}

			if (pickupSpawnFuckedUp) {
				Destroy(newPickup);
				p--;
			}
			else {
				//roomdata.beamBlockers.Add(newPickup);
				newPickup.transform.position += (Vector3.up * 0.5f);
			}

			fillerPositions.Add(pickupPos);
		}
	}
}
