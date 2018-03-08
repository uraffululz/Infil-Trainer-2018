using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour {
	[SerializeField] GameObject player;
	[SerializeField] GameObject floorPlane;

	[SerializeField] int roomWidth;
	[SerializeField] int roomDepth;


	void Awake () {
		roomWidth = Random.Range (4, 10);
		roomDepth = Random.Range (4, 10);

		LayFloor ();
	}


	void Start () {
		SpawnPlayer ();
	}
	

	void Update () {
		
	}


	void LayFloor () {
		GameObject floorParent = new GameObject ();
		floorParent.name = "FloorParent";
		floorParent.transform.parent = gameObject.transform;

		for (int rD = 0; rD < roomDepth; rD++) {
			for (int rW = 0; rW < roomWidth; rW++) {
				Vector3 floorPos = new Vector3 (rW, 0.0f, rD);

				floorPlane = Instantiate (floorPlane, floorPos, Quaternion.identity, floorParent.transform);
			}
		}
	}


	void SpawnPlayer () {
		Vector3 playerPos = new Vector3 (roomWidth / 2, 1.0f, 1.0f);
		GameObject Player = Instantiate (player, playerPos, Quaternion.identity);

		//Setup Main Camera
		GameObject camEmpty = new GameObject ();
		camEmpty.name = "CamEmpty";
		camEmpty.transform.position = Player.transform.position;
		camEmpty.transform.parent = Player.transform;
		camEmpty.AddComponent<Camera> ();
	}
}
