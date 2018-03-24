using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFiller : MonoBehaviour {

	RoomBuilder roomBuild;

	public GameObject player;
	GameObject pickupParent;

	[SerializeField] GameObject coin;
	[SerializeField] GameObject gem;
	[SerializeField] GameObject displayCase;

	[SerializeField] GameObject alarmBox;

	List<Vector3> fillerPositions = new List<Vector3> ();
	public List<Collider> beamBlockers = new List<Collider>();


	void Awake () {
		roomBuild = GameObject.Find ("LevelManager").GetComponent<RoomBuilder> ();
		pickupParent = GameObject.Find ("PickupParent");

		SpawnPlayer ();
		SpawnPickups ();
	}


	void Start () {
		SpawnAlarmBox ();

	}

	
	void Update () {
		
	}


	void SpawnPlayer () {
		Vector3 playerPos = new Vector3 (roomBuild.roomWidth / 2, 0.0f, 0.0f);
		player = Instantiate (player, playerPos + Vector3.up * 0.5f, Quaternion.identity);
		player.name = "Player";
		fillerPositions.Add (playerPos);
		beamBlockers.Add (player.GetComponent<CapsuleCollider> ());

		player.GetComponent<MeshRenderer> ().material.color = Color.blue;

		//Setup Main Camera
		GameObject camEmpty = new GameObject ();
		camEmpty.name = "CamEmpty";
		Vector3 camOffset = new Vector3 (0.0f, 0.3f, 0.0f);
		camEmpty.transform.position = player.transform.position + camOffset;
		camEmpty.transform.parent = player.transform;
		Camera playCam = camEmpty.AddComponent<Camera> ();
		playCam.tag = "MainCamera";
		playCam.clearFlags = CameraClearFlags.SolidColor;
		playCam.backgroundColor = Color.black;
		playCam.nearClipPlane = 0.001f;

	}


	void SpawnPickups() {
		GameObject[] pickups = new GameObject[] {coin, gem};

		int howRich = (int)(roomBuild.roomDepth * roomBuild.roomWidth) / 10;
		int displayCount = (int) (roomBuild.roomDepth * roomBuild.roomWidth)/20;

		for (int i = 0; i < displayCount + howRich; i++) {
			Vector3 spawnPos = new Vector3 (Random.Range (0, roomBuild.roomWidth), 0.0f, Random.Range (0, roomBuild.roomDepth-1));
			if (!fillerPositions.Contains (spawnPos)) {
				if (i < displayCount) {
					GameObject spawnedCase = Instantiate (displayCase, spawnPos, Quaternion.identity, pickupParent.transform);
					fillerPositions.Add (spawnPos);
					beamBlockers.Add (spawnedCase.GetComponent<BoxCollider>());
				} else {
					GameObject spawnedPickup = Instantiate (pickups [Random.Range (0, pickups.Length)],
						                          spawnPos, Quaternion.identity, pickupParent.transform);
					fillerPositions.Add (spawnPos);
					//beamBlockers.Add (spawnedPickup.GetComponent<Collider> ());
				}
			} else {
				//If spawnedPickup tried to spawn in same place as another spawnedPickup, try again
				i--;
			}
		}
	}

	void SpawnAlarmBox () {
		GameObject chosenWall = roomBuild.walls [Random.Range (1, roomBuild.walls.Length-1)];
		Vector3 boxPos = chosenWall.transform.position + (Vector3.up * 0.7f);
		alarmBox = Instantiate (alarmBox, boxPos, chosenWall.transform.rotation, gameObject.transform);
		alarmBox.name = "AlarmBox";
		beamBlockers.Add (alarmBox.GetComponent<BoxCollider>());

		alarmBox.GetComponent<MeshRenderer> ().material.color = Color.black;
	}
}
