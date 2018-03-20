using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour {
	RoomFiller roomFill;

	GameObject floorParent;
	GameObject wallParent;
	GameObject ceilingParent;
	GameObject pickupParent;
	GameObject laserParent;

	[SerializeField] GameObject floorPlane;
	[SerializeField] GameObject wallPanel;
	[SerializeField] GameObject ceilingTile;
	[SerializeField] GameObject doorWay;
	[SerializeField] GameObject door;

	public int roomWidth;
	public int roomDepth;
	public float roomHeight = 2.0f;
	int doorNum = 0;

	public GameObject[] floors;
	public GameObject[] walls;
	public GameObject[] ceilings;

	public enum BuildingStates {building, done};
	public BuildingStates buildProgress;


	void Awake () {
		roomFill = gameObject.GetComponent<RoomFiller> ();

		buildProgress = BuildingStates.building;
//Room too big?
		roomWidth = Random.Range (5, 8);
		roomDepth = Random.Range (5, 8);

		CreateParents ();
	}


	void Start () {
		LayFloor ();
		PutUpWalls ();
		HangCeiling ();

		floors = GameObject.FindGameObjectsWithTag ("Floor");
		walls = GameObject.FindGameObjectsWithTag ("Wall");
		ceilings = GameObject.FindGameObjectsWithTag ("Ceiling");
	}
	

	void Update () {
		
	}


	void LayFloor () {
		for (int rD = 0; rD < roomDepth; rD++) {
			for (int rW = 0; rW < roomWidth; rW++) {
				Vector3 floorPos = new Vector3 (rW, 0.0f, rD);

				floorPlane = Instantiate (floorPlane, floorPos, Quaternion.identity, floorParent.transform);
				floorPlane.name = "Floor";
			}
		}
	}


	void PutUpWalls () {
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

					if (rW > 1 && rW <= roomWidth - 1 && doorNum < 1) {
						doorWay = Instantiate (doorWay, wallPlace + wallOffset, Quaternion.identity, wallParent.transform);
						doorWay.name = "Doorway";
						roomFill.beamBlockers.Add (doorWay.GetComponent<BoxCollider>());

						Vector3 doorOffset = new Vector3 (0.3215f, 0.0f, 0.0f);
						door = Instantiate (door, doorWay.transform.position + doorOffset, Quaternion.identity, doorWay.transform);
						door.name = "Door";

						doorNum++;
					} else {
						wallPanel = Instantiate (wallPanel, wallPlace + wallOffset, Quaternion.Euler (0.0f, 180.0f, 0.0f), wallParent.transform);
					}
				}
				wallPanel.name = "Wall";
			}
		}
//TEMPORARY link to Game Over / Win Screen
		GameObject levelEnd = GameObject.CreatePrimitive(PrimitiveType.Quad);
		levelEnd.name = "LevelEnd";
		levelEnd.transform.position = GameObject.Find ("Door").transform.position + Vector3.forward * 0.2f;
	}


	void HangCeiling () {
		for (int rD = 0; rD < roomDepth; rD++) {
			for (int rW = 0; rW < roomWidth; rW++) {
				Vector3 ceilingPos = new Vector3 (rW, roomHeight, rD);

				ceilingTile = Instantiate (ceilingTile, ceilingPos, Quaternion.identity, ceilingParent.transform);
				ceilingTile.name = "Ceiling";
			}
		}
	}


	void CreateParents () {
		floorParent = new GameObject ();
		floorParent.name = "FloorParent";
		floorParent.transform.parent = gameObject.transform;

		wallParent = new GameObject ();
		wallParent.name = "WallParent";
		wallParent.transform.parent = gameObject.transform;

		ceilingParent = new GameObject ();
		ceilingParent.name = "CeilingParent";
		ceilingParent.transform.parent = gameObject.transform;

		pickupParent = new GameObject ();
		pickupParent.name = "PickupParent";
		pickupParent.transform.parent = gameObject.transform;

		laserParent = new GameObject ();
		laserParent.name = "LaserParent";
		laserParent.transform.parent = gameObject.transform;
		laserParent.AddComponent<LaserParent> ();
	}


	void CreateLaserParent () {
		GameObject laserParent = new GameObject ();
		laserParent.name = "LaserParent";
		laserParent.transform.parent = gameObject.transform;
		laserParent.AddComponent<LaserParent> ();
	}
}
