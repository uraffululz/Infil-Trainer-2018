using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour {

	//Scene GameObject References
	GameObject player;

	//My Room References
	[SerializeField] GameObject myRoom;
	MyRoomData roomData;

	//Prefab References For Spawning
	[SerializeField] GameObject node;
	[SerializeField] GameObject receiver;
	[SerializeField] GameObject beam;

	//Prefab Parent References (to be spawned later \/)
	GameObject nodeParent;
	GameObject receiverParent;
	GameObject beamParent;

	//Spawn Count References
	int spawnCount = 10;
	int spawnRetryCount = 0;

	//Spawn Room Surface Location References
	[SerializeField] List<int> nodeSetWhichSurfaceParent = new List<int>();
	[SerializeField] List<int> receiverSetWhichSurfaceParent = new List<int>();

	//Lists of Spawned Prefabs
	[SerializeField] List<GameObject> nodesSpawned = new List<GameObject>();
	[SerializeField] List<GameObject> receiversSpawned = new List<GameObject>();
	[SerializeField] List<GameObject> beamsSpawned = new List<GameObject>();
	


	void Awake() {
		//Initialize references
		myRoom = transform.parent.gameObject;
		roomData = myRoom.GetComponent<MyRoomData>();

		//Set the number of lasers to be spawned in my room
		//Needs to be updated to the new refactoring standards
		//spawnCount = roomData.myWidth * roomData.myDepth / 2;

		//Spawn parents to contain the various prefabs to be spawned, to keep the hierarchy clean
		SpawnComponentParents();
	}


	void Start () {
		//Initialize references
		player = GameObject.FindWithTag("Player");
	}


	void Update () {
		//As part of the room-building process, start spawning laser prefabs
		if (roomData.myBuildState == MyRoomData.myRoomBuildState.building) {
				SpawnStartsHere(spawnCount);
		}
		//else if (roomData.myBuildState == MyRoomData.myRoomBuildState.finished) {
			
		//}
	}


	void SpawnComponentParents() {
		nodeParent = new GameObject("NodeParent");
		nodeParent.transform.parent = gameObject.transform;

		receiverParent = new GameObject("ReceiverParent");
		receiverParent.transform.parent = gameObject.transform;

		beamParent = new GameObject("BeamParent");
		beamParent.transform.parent = gameObject.transform;
	}


	void SpawnStartsHere(int howManyToSpawn) {
		spawnRetryCount = 0;

		SetSpawnParents(howManyToSpawn);
	}


	void SetSpawnParents(int newSpawnCount) {
		for (int i = 0; i < newSpawnCount; i++) {
			//Choose the surfaces for, and populate, the lists of surfaces to spawn the room's nodes and receivers
			nodeSetWhichSurfaceParent.Add(Random.Range(0, roomData.myLaserSpawnSurfaces.Length));
			receiverSetWhichSurfaceParent.Add(Random.Range(0, roomData.myLaserSpawnSurfaces.Length));
		}

		if (nodeSetWhichSurfaceParent.Count == spawnCount) {
			SpawnNodesAndReceivers(roomData.laserNode, newSpawnCount);
		}
		else {
			print("I didn't spawn enough SpawnParents");
		}
	}


	void SpawnNodesAndReceivers(GameObject component, int currentSpawnCount) {
		for (int l = spawnCount - currentSpawnCount; l < spawnCount; l++) {
			GameObject compParent = null;

			switch (component.tag) {
				case "LaserNode":
					compParent = roomData.myLevel1Children[nodeSetWhichSurfaceParent[l]];
					break;
				case "LaserReceiver":
					compParent = roomData.myLevel1Children[receiverSetWhichSurfaceParent[l]];
					break;
				default:
					Debug.Log("I don't have the right tag");
					compParent = new GameObject();
					break;
			}

			GameObject selectedTile = compParent.transform.GetChild(Random.Range(0, compParent.transform.childCount)).gameObject;
//TODO This is NOT random. Keep at it
//Really? Sure fucking SEEMS like it is. Is that an old TODO?
			Vector3 randomPointOnSelectedTile = new Vector3(
				Random.Range(-selectedTile.GetComponent<MeshCollider>().bounds.extents.x, selectedTile.GetComponent<MeshCollider>().bounds.extents.x),
				Random.Range(0 /*because the wall objects' origins are at their base, not their center, and everything else is flat on the x-axis anyway*/,
								selectedTile.GetComponent<MeshCollider>().bounds.extents.y * 2),
				Random.Range(-selectedTile.GetComponent<MeshCollider>().bounds.extents.z, selectedTile.GetComponent<MeshCollider>().bounds.extents.z));

			GameObject newComponent = Instantiate(component, selectedTile.transform.position + randomPointOnSelectedTile, Quaternion.identity, gameObject.transform);

			switch (component.tag) {
				case "LaserNode":
					nodesSpawned.Add(newComponent);
					newComponent.transform.parent = nodeParent.transform;

					//When the correct number of nodes has been spawned, begin spawning the accompanying receivers
					if (nodesSpawned.Count == spawnCount) {
						SpawnNodesAndReceivers(roomData.laserReceiver, currentSpawnCount); //ditto
					}
					break;
				case "LaserReceiver":
					receiversSpawned.Add(newComponent);
					newComponent.transform.parent = receiverParent.transform;

					//When the correct number of nodes and receivers havd been spawned, begin spawning their shared laserbeams
					if (receiversSpawned.Count == spawnCount) {
						SpawnBeams(currentSpawnCount);
					}
					break;
				default:
					Debug.Log("I don't have the right tag");
					break;
			}
		}
	}


	void SpawnBeams (int myNewSpawnCount) {
		for (int bts = spawnCount - myNewSpawnCount /*Pretty sure this is the missing number I was looking for. Don't fuck with it unless you know it's wrong*/; bts < spawnCount; bts++) {
				float beamScaleTotal = (receiversSpawned[bts].transform.position - nodesSpawned[bts].transform.position).magnitude;
				Vector3 beamPos = (nodesSpawned[bts].transform.position + receiversSpawned[bts].transform.position) / 2;

				GameObject newBeam = Instantiate(roomData.laserBeam, beamPos, Quaternion.identity, beamParent.transform);
				beamsSpawned.Add(newBeam);
				newBeam.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, beamScaleTotal);
				newBeam.gameObject.transform.rotation = Quaternion.LookRotation(newBeam.transform.position - receiversSpawned[bts].transform.position);
		}
		
		for (int bsc = spawnCount - 1; bsc >= 0; bsc--) {
			bool beamFuckedUp = false;

			if (nodeSetWhichSurfaceParent[bsc] == receiverSetWhichSurfaceParent[bsc]
/*TODO I don't know if I still need this "OR" statement \/, since the player is spawned standing within the first doorway's BoxCollider*/
			|| beamsSpawned[bsc].GetComponent<BoxCollider>().bounds.Intersects(player.GetComponent<CapsuleCollider>().bounds)) {
				beamFuckedUp = true;
			}

			if (roomData.beamBlockers.Count > 0) {
				foreach (GameObject blocker in roomData.beamBlockers) {
					if (beamsSpawned[bsc].GetComponent<BoxCollider>().bounds.Intersects(blocker.GetComponent<BoxCollider>().bounds)) {
						beamFuckedUp = true;
					}
				}
			}

			if (beamFuckedUp == true) {
				SetBeamToRetry(bsc);
			}
		}

		if (spawnRetryCount > 0) {
			SpawnStartsHere(spawnRetryCount);
		}
		else {
			roomData.myBuildState = MyRoomData.myRoomBuildState.finished;
		}
	}


	void SetBeamToRetry(int beamIndex) {
		//This beam spawned incorrectly, so destroy its parents and erase any mention of it from the history books, and try again
		Destroy(nodesSpawned[beamIndex]);
		Destroy(receiversSpawned[beamIndex]);
		Destroy(beamsSpawned[beamIndex]);

		nodesSpawned.RemoveAt(beamIndex);
		receiversSpawned.RemoveAt(beamIndex);
		beamsSpawned.RemoveAt(beamIndex);

		nodeSetWhichSurfaceParent.RemoveAt(beamIndex);
		receiverSetWhichSurfaceParent.RemoveAt(beamIndex);

		spawnRetryCount++;
	}
}
