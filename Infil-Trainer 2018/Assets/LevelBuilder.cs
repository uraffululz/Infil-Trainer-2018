﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {

	//Room Building Variables
	int maxRoomNum;
	public int currentRoomNum = 0;
	GameObject currentRoomObject;
	[SerializeField] GameObject[] roomArray;
	Vector3 currentRoomSpawnPoint;
	Quaternion currentRoomSpawnRot;

	//Player Variables
	GameObject player;
	[SerializeField] GameObject playerPrefab;


	void Awake() {
		maxRoomNum = Random.Range(3, 5);
		//print(maxRoomNum);
		SpawnRooms();
		SpawnPlayer();
		
	}

	void Start () {
		
	}


	void Update () {
		
	}


	void SpawnRooms () {
		for (int rS = 0 /* or currentRoomNum*/; rS <= maxRoomNum; rS++) {
			//When placing the first room
			if (rS == 0) {
				currentRoomSpawnPoint = Vector3.zero;
				currentRoomSpawnRot = Quaternion.identity;
			}
			//When placing the final room
			else if (rS == maxRoomNum) {
				//Find the position and rotation of the most recently-spawned room's exit wall/door
				Transform prevRoom = currentRoomObject.transform.Find("ExitWallParent");
				currentRoomSpawnPoint = prevRoom.transform.position + prevRoom.transform.forward * 0.06f;
				currentRoomSpawnRot = prevRoom.transform.rotation;

				//Choose a "newRoom" from a separate roomArray than the others (one containing rooms without an exit door)
				//Alternatively, place differently-colored "exit signs" over the final door

			}
			//When placing the other/in-between rooms
			else {
				//Find the position and rotation of the most recently-spawned room's exit wall/door
				Transform prevRoom = currentRoomObject.transform.Find("ExitWallParent");
				currentRoomSpawnPoint = prevRoom.transform.position + prevRoom.transform.forward * 0.06f;
				currentRoomSpawnRot = prevRoom.transform.rotation;

			}

			GameObject newRoom = Instantiate(roomArray[Random.Range(0, roomArray.Length)], currentRoomSpawnPoint, currentRoomSpawnRot, this.transform);
			SpawnLaserParent(newRoom);


			currentRoomObject = newRoom;
			//currentRoomNum++;
		}
	}


	void SpawnLaserParent(GameObject myRoom) {
		GameObject laserParent = new GameObject();
		laserParent.name = "LaserParent";
		laserParent.transform.parent = myRoom.transform;
		laserParent.AddComponent<LaserManager>();
	}


	void SpawnPlayer() {
		//		Vector3 playerPos = new Vector3 (roomBuild.roomWidth / 2, 0.0f, 0.0f);
		Vector3 playerPos = new Vector3(0, 0.5f, 0.5f);

		player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
		player.name = "Player";
		//fillerPositions.Add(playerPos);
		//beamBlockers.Add(player.GetComponent<CapsuleCollider>());

		player.GetComponent<MeshRenderer>().material.color = Color.blue;

		//Setup Main Camera
//TODO Make sure the camera's view adjusts to any changes in aspect ratio or scaling
		GameObject camEmpty = new GameObject();
		camEmpty.name = "CamEmpty";
		Vector3 camOffset = new Vector3(0.0f, 0.3f, 0.0f);
		camEmpty.transform.position = player.transform.position + camOffset;
		camEmpty.transform.parent = player.transform;
		Camera playCam = camEmpty.AddComponent<Camera>();
		playCam.tag = "MainCamera";
		playCam.clearFlags = CameraClearFlags.SolidColor;
		playCam.backgroundColor = Color.black;
		playCam.nearClipPlane = 0.001f;

	}
}
