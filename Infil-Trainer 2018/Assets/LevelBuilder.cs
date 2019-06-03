using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour {

	//Room Building Variables
	public static int maxRoomNum;
	GameObject currentRoomObject;
	[SerializeField] GameObject[] roomArray;
	Vector3 currentRoomSpawnPoint;
	Quaternion currentRoomSpawnRot;

	[SerializeField] GameObject doorPrefab;

	//Player Variables
	[Header("Player Variables")]
	[SerializeField] GameObject playerPrefab;
	GameObject player;

	public List<GameObject> levelTreasures;
	


	void Awake() {
		maxRoomNum = 2 /*Random.Range(3, 5)*/;
		SpawnRooms();
	}


	void SpawnRooms () {
//TOPOSSIBLYDO Pull the spawned rooms from 3 (or more) different lists, depending on their placement,
//i.e. StartRooms, MidRooms, EndRooms
//That way, I can set each up with the correct entry/exit doors, make sure the player doesn't start in a hallway/stairway(maybe), etc.
		for (int rS = 0 /* or currentRoomNum*/; rS < maxRoomNum; rS++) {
			//When placing the first room
			if (rS == 0) {
				currentRoomSpawnPoint = Vector3.zero;
				currentRoomSpawnRot = Quaternion.identity;
			}
			//When placing the final room
			else if (rS == maxRoomNum - 1) {
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

			//Spawn the new room with the designated position/rotation
			GameObject newRoom = Instantiate(roomArray[Random.Range(0, roomArray.Length)], currentRoomSpawnPoint, currentRoomSpawnRot, this.transform);
			
			//Use the newly-spawned room as a reference to place the next
			currentRoomObject = newRoom;
			SpawnMyDoor(newRoom);

			//When placing the final room
			if (rS == maxRoomNum - 1) {
				CreateWinBox();
			}
		}
	}


	void SpawnMyDoor(GameObject thisRoom) {
		GameObject thisDoor = Instantiate(doorPrefab, thisRoom.transform.position, thisRoom.transform.localRotation, thisRoom.transform);
	}


	void CreateWinBox() {
		//Place the level exit (win-box) beyond the final room's exit
		Transform prevRoom = currentRoomObject.transform.Find("ExitWallParent");

		GameObject winBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
		winBox.transform.position = prevRoom.position + (prevRoom.forward * 0.5f);
		winBox.name = "WinBox";

		winBox.GetComponent<BoxCollider>().isTrigger = true;
		winBox.AddComponent<WinBox>();
	}
}
