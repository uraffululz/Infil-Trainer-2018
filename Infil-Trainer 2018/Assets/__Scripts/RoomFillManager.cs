using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFillManager : MonoBehaviour {

	//GameObject References
	GameObject levMan;
	GameObject player;

	//My Room Variables
	MyRoomData roomdata;

	//Filler Parent References (Spawned Later \/)
	GameObject caseParent;
	GameObject pickupParent;

	//Spawn Placement Variables
	[SerializeField] List<Vector3> fillerPositions = new List<Vector3>();

	//Prefab References To Spawn
	[SerializeField] GameObject displayCase;
	[SerializeField] GameObject alarmBox;
	[SerializeField] List<GameObject> pickupPrefabs; //The List of smaller pickup prefabs (coins, gems, etc.), to be spawned randomly

	[SerializeField] List<GameObject> alarmBoxes; //The list of Alarm Boxes in the level

	int caseCount = 2;
	int pickupCount = 2;



	void Awake() {
		//Initialize references
		levMan = transform.parent.gameObject;
		roomdata = gameObject.GetComponent<MyRoomData>();

		//Spawn parents to contain the various prefabs to be spawned, to keep the hierarchy clean
		SpawnFillerParents();
	}


	void Start () {
		//Initialize references
		player = GameObject.FindWithTag("Player");

		//Spawn various prefabs
		SpawnDisplayCases();
		SpawnPickups();
		SpawnAlarmBox();
	}


	void SpawnFillerParents() {
		caseParent = new GameObject("CaseParent");
		caseParent.transform.parent = transform;

		pickupParent = new GameObject("PickupParent");
		pickupParent.transform.parent = transform;
	}


	void SpawnDisplayCases() {
		for (int c = 0; c < caseCount; c++) {
			bool spawnFuckedUp = false;

			Vector3 casePos = roomdata.myFloorTiles[Random.Range(0, roomdata.myFloorTiles.Count)].transform.position;
			GameObject newCase = Instantiate(displayCase, casePos, Quaternion.identity, caseParent.transform);

			if (fillerPositions.Contains(casePos)) {
				spawnFuckedUp = true;
			}

			foreach (GameObject blocker in roomdata.beamBlockers) {
				if (blocker.GetComponent<BoxCollider>().bounds.Intersects(newCase.GetComponent<BoxCollider>().bounds)) {
					spawnFuckedUp = true;
				}
			}

			if (spawnFuckedUp) {
				Destroy(newCase);
				c--;
			}
			else {
				//If the current Display Case is successfully spawned
				roomdata.beamBlockers.Add(newCase);

				GameObject myCaseTreasure = newCase.GetComponent<DisplayCaseTreasure>().selectedTreasure;
				levMan.GetComponent<LevelBuilder>().levelTreasures.Add(myCaseTreasure);
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

			if (fillerPositions.Contains(casePos)) {
				spawnFuckedUp = true;
			}
		
			foreach (GameObject blocker in roomdata.beamBlockers) {
				if (blocker.GetComponent<BoxCollider>().bounds.Intersects(myAlarmBox.GetComponent<BoxCollider>().bounds)) {
					spawnFuckedUp = true;
				}
			}

			if (spawnFuckedUp) {
				Destroy(myAlarmBox);
				abc--;
			}
			else {
				roomdata.beamBlockers.Add(myAlarmBox);
				alarmBoxes.Add(myAlarmBox);
				myAlarmBox.name = "AlarmBox";
			}

			fillerPositions.Add(casePos);
		}
	}


	void SpawnPickups() {
		for (int p = 0; p < pickupCount; p++) {
			bool spawnFuckedUp = false;

			Vector3 pickupPos = roomdata.myFloorTiles[Random.Range(0, roomdata.myFloorTiles.Count)].transform.position;

			GameObject newPickup = Instantiate(pickupPrefabs[Random.Range(0, pickupPrefabs.Count)], pickupPos, Quaternion.identity, pickupParent.transform);

			if (fillerPositions.Contains(pickupPos)
							|| newPickup.GetComponent<Collider>().bounds.Intersects(player.GetComponent<CapsuleCollider>().bounds)) {
				spawnFuckedUp = true;
			}

			foreach (GameObject blocker in roomdata.beamBlockers) {
				if (blocker.GetComponent<BoxCollider>().bounds.Intersects(newPickup.GetComponent<Collider>().bounds)) {
					spawnFuckedUp = true;
				}
			}

			if (spawnFuckedUp) {
				Destroy(newPickup);
				p--;
			}
			else {
				//If the Pickup is successfully spawned
				//roomdata.beamBlockers.Add(newPickup);
				levMan.GetComponent<LevelBuilder>().levelTreasures.Add(newPickup);
				newPickup.transform.position += (Vector3.up * 0.5f);
			}

			fillerPositions.Add(pickupPos);
		}
	}
}
