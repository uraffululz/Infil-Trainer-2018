using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRoomData : MonoBehaviour {

	public RoomStats myStats;

	public int myWidth;
	public int myDepth;
	public int myHeight;

	public bool hasFloorTreasure;
	public bool hasWallTreasure;
	public bool hasPickups;

	public bool hasLasers;

	public List<GameObject> myLevel1Children;
	public GameObject[] myLaserSpawnSurfaces;
	public List<GameObject> myFloorTiles;
	public List<GameObject> myWallTiles;
	[SerializeField] List<GameObject> myCeilingTiles;
	public List<GameObject> myDoorways;
	[SerializeField] List<GameObject> myDoors;

	public List<GameObject> beamBlockers = new List<GameObject>();

	public enum myRoomBuildState {building, finished};
	public myRoomBuildState myBuildState;

	void Awake() {
		myBuildState = myRoomBuildState.building;

		myWidth = myStats.roomWidth;
		myDepth = myStats.roomDepth;
		myHeight = myStats.roomHeight;

		hasFloorTreasure = myStats.allowFloorTreasure;
		hasWallTreasure = myStats.allowWallTreasure;
		hasPickups = myStats.allowPickups;

		hasLasers = myStats.allowLasers;

		int numberOfChildren = gameObject.transform.childCount;

		for (int c = 0; c < numberOfChildren; c++) {
			myLevel1Children.Add(transform.GetChild(c).gameObject);
		}

		foreach (GameObject groupParent in myLevel1Children) {
			for (int i = 0; i < groupParent.transform.childCount; i++) {
				if (groupParent.transform.GetChild(i).CompareTag("Floor")) {
					myFloorTiles.Add(groupParent.transform.GetChild(i).gameObject);
				}
				else if (groupParent.transform.GetChild(i).CompareTag("Wall")) {
					myWallTiles.Add(groupParent.transform.GetChild(i).gameObject);
				}
				else if (groupParent.transform.GetChild(i).CompareTag("Ceiling")) {
					myCeilingTiles.Add(groupParent.transform.GetChild(i).gameObject);
				}
				else if (groupParent.transform.GetChild(i).CompareTag("Doorway")) {
					myDoorways.Add(groupParent.transform.GetChild(i).gameObject);
				}
//ACTUALLY, this will need to check the next level down, if the door is a child of the doorway
				else if (groupParent.transform.GetChild(i).CompareTag("Door")) {
					myDoors.Add(groupParent.transform.GetChild(i).gameObject);
				}
			}

			if (groupParent.layer == LayerMask.NameToLayer("BeamBlockers")) {
				beamBlockers.Add(groupParent);
			}
		}

		foreach (GameObject doorway in myDoorways) {
			beamBlockers.Add(doorway);
		}

	}


	void Start() {
		//beamBlockers.Add(GameObject.FindGameObjectWithTag("Player"));

	}
}
