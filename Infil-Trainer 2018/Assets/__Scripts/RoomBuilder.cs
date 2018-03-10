using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour {
	[SerializeField] GameObject player;
	[SerializeField] GameObject floorPlane;
	[SerializeField] GameObject wallPanel;
	[SerializeField] GameObject ceilingTile;

	public int roomWidth;
	public int roomDepth;

	public GameObject[] floors;
	public GameObject[] walls;
	public GameObject[] ceilings;


	void Awake () {
		roomWidth = Random.Range (5, 10);
		roomDepth = Random.Range (5, 10);

		LayFloor ();
		PutUpWalls ();
		HangCeiling ();

		floors = GameObject.FindGameObjectsWithTag ("Floor");
		walls = GameObject.FindGameObjectsWithTag ("Wall");
		ceilings = GameObject.FindGameObjectsWithTag ("Ceiling");


		CreatePickupParent ();

		CreateLaserParent ();
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
				floorPlane.name = "Floor";
			}
		}
	}


	void PutUpWalls () {
		GameObject wallParent = new GameObject ();
		wallParent.name = "WallParent";
		wallParent.transform.parent = gameObject.transform;
		for (int rD = 0; rD <= roomDepth+1; rD++) {
			for (int rW = 0; rW < roomWidth; rW++) {
				if (rD == 0) {
					Vector3 wallPlace = new Vector3 (rW, 0.0f, rD);
					Vector3 wallOffset = new Vector3 (0.0f, 0.0f, -0.5f);
					wallPanel = Instantiate (wallPanel, wallPlace + wallOffset, Quaternion.identity, wallParent.transform);
				} else if (rD > 0 && rD < roomDepth+1) {
					if (rW == 0) {
						Vector3 wallPlace = new Vector3 (rW, 0.0f, rD);
						Vector3 wallOffset = new Vector3 (-0.5f, 0.0f, -1.0f);
						wallPanel = Instantiate (wallPanel, wallPlace + wallOffset, Quaternion.Euler (0.0f, 90.0f, 0.0f), wallParent.transform);
					} else if (rW == roomWidth-1) {
						Vector3 wallPlace = new Vector3 (rW, 0.0f, rD);
						Vector3 wallOffset = new Vector3 (0.5f, 0.0f, -1.0f);
						wallPanel = Instantiate (wallPanel, wallPlace + wallOffset, Quaternion.Euler (0.0f, -90.0f, 0.0f), wallParent.transform);
					}
				}
				if (rD == roomDepth) {
					Vector3 wallPlace = new Vector3 (rW, 0.0f, rD);
					Vector3 wallOffset = new Vector3 (0.0f, 0.0f, -0.5f);
					wallPanel = Instantiate (wallPanel, wallPlace + wallOffset, Quaternion.Euler (0.0f, 180.0f, 0.0f), wallParent.transform);
				}
				wallPanel.name = "Wall";
			}
		}
	}


	void HangCeiling () {
		GameObject ceilingParent = new GameObject ();
		ceilingParent.name = "CeilingParent";
		ceilingParent.transform.parent = gameObject.transform;

		for (int rD = 0; rD < roomDepth; rD++) {
			for (int rW = 0; rW < roomWidth; rW++) {
				Vector3 ceilingPos = new Vector3 (rW, 3.0f, rD);

				ceilingTile = Instantiate (ceilingTile, ceilingPos, Quaternion.identity, ceilingParent.transform);
				ceilingTile.name = "Ceiling";
			}
		}
	}


	void CreatePickupParent () {
		GameObject PickupParent = new GameObject ();
		PickupParent.name = "PickupParent";
		PickupParent.transform.parent = gameObject.transform;
		PickupParent.AddComponent<PickupParent> ();
	}


	void CreateLaserParent () {
		GameObject laserParent = new GameObject ();
		laserParent.name = "LaserParent";
		laserParent.transform.parent = gameObject.transform;
		laserParent.AddComponent<LaserParent> ();
	}


	void SpawnPlayer () {
		Vector3 playerPos = new Vector3 (roomWidth / 2, 0.5f, 1.0f);
		player = Instantiate (player, playerPos, Quaternion.identity);
		player.name = "Player";

		//Setup Main Camera
		GameObject camEmpty = new GameObject ();
		camEmpty.name = "CamEmpty";
		camEmpty.transform.position = player.transform.position;
		camEmpty.transform.parent = player.transform;
		camEmpty.AddComponent<Camera> ();
	}
}
