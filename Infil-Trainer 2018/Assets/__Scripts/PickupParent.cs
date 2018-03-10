using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupParent : MonoBehaviour {

	RoomBuilder roomBuild;

	[SerializeField] GameObject coin;
	[SerializeField] GameObject gem;

	List<Vector3> PickupPositions;


	void Awake () {
		roomBuild = GameObject.Find ("LevelManager").GetComponent<RoomBuilder> ();

		PickupPositions = new List<Vector3> ();
	}


	void Start () {
		SpawnPickups ();
	}
	

	void Update () {
		
	}


	void SpawnPickups() {
		int howRich = (int)(roomBuild.roomDepth * roomBuild.roomWidth) / 10;
		GameObject[] pickups = new GameObject[] { coin, gem };

		for (int i = 0; i < howRich; i++) {
			Vector3 spawnPos = new Vector3 (Random.Range (0, roomBuild.roomWidth), 0.3f, Random.Range (0, roomBuild.roomDepth));
			if (!PickupPositions.Contains (spawnPos)) {
				GameObject spawnedPickup = Instantiate (pickups [Random.Range (0, pickups.Length)], spawnPos, Quaternion.identity, gameObject.transform);
				PickupPositions.Add (spawnPos);
			} else {
				//print ("Pickup tried to spawn in same place. Trying again");
				i--;
			}
		}
	}
}
