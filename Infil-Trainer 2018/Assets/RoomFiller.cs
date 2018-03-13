using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFiller : MonoBehaviour {

	RoomBuilder roomBuild;

	public GameObject player;
	GameObject pickupParent;
	[SerializeField] GameObject coin;
	[SerializeField] GameObject gem;

	List<Vector3> PickupPositions = new List<Vector3> ();
	public List<Collider> beamBlockers = new List<Collider>();


	void Awake () {
		roomBuild = GameObject.Find ("LevelManager").GetComponent<RoomBuilder> ();
		pickupParent = GameObject.Find ("PickupParent");

		SpawnPlayer ();
		SpawnPickups ();
	}


	void Start () {

	}

	
	void Update () {
		
	}


	void SpawnPlayer () {
		Vector3 playerPos = new Vector3 (roomBuild.roomWidth / 2, 0.5f, 0.5f);
		player = Instantiate (player, playerPos, Quaternion.identity);
		player.name = "Player";
		beamBlockers.Add (player.GetComponent<CapsuleCollider> ());

		//Setup Main Camera
		GameObject camEmpty = new GameObject ();
		camEmpty.name = "CamEmpty";
		camEmpty.transform.position = player.transform.position;
		camEmpty.transform.parent = player.transform;
		Camera playCam = camEmpty.AddComponent<Camera> ();
		playCam.nearClipPlane = 0.001f;

	}


	void SpawnPickups() {
		int howRich = (int)(roomBuild.roomDepth * roomBuild.roomWidth) / 10;
		GameObject[] pickups = new GameObject[] { coin, gem };

		for (int i = 0; i < howRich; i++) {
			Vector3 spawnPos = new Vector3 (Random.Range (0, roomBuild.roomWidth), 0.3f, Random.Range (0, roomBuild.roomDepth));
			if (!PickupPositions.Contains (spawnPos)) {
				GameObject spawnedPickup = Instantiate (pickups [Random.Range (0, pickups.Length)],
					spawnPos, Quaternion.identity, pickupParent.transform);
				PickupPositions.Add (spawnPos);
				beamBlockers.Add (spawnedPickup.GetComponent<Collider>());
			} else {
				//If spawnedPickup tried to spawn in same place as another spawnedPickup, try again
				i--;
			}
		}
	}
}
