using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinParent : MonoBehaviour {

	RoomBuilder roomBuild;

	[SerializeField] GameObject coin;
	[SerializeField] GameObject gem;


	void Awake () {
		roomBuild = GameObject.Find ("LevelManager").GetComponent<RoomBuilder> ();
	}


	void Start () {
		SpawnCoins ();
	}
	

	void Update () {
		
	}


	void SpawnCoins() {
//TODO Make sure they don't spawn within each others' space
		int howRich = (int)(roomBuild.roomDepth * roomBuild.roomWidth) / 10;
		GameObject[] pickups = new GameObject[] { coin, gem };

		for (int i = 0; i < howRich; i++) {
			Vector3 spawnPos = new Vector3 (Random.Range (1, roomBuild.roomWidth - 1), 0.3f, Random.Range (1, roomBuild.roomDepth - 1));

			GameObject spawnedPickup = Instantiate (pickups [Random.Range (0, pickups.Length)], spawnPos, Quaternion.identity, gameObject.transform);

		}
	}
}
