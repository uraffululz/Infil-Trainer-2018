using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour {

	GameObject player;

	[SerializeField] GameObject myRoom;
	MyRoomData roomData;

	[SerializeField] GameObject node;
	[SerializeField] GameObject receiver;
	[SerializeField] GameObject beam;

	GameObject nodeParent;
	GameObject receiverParent;
	GameObject beamParent;

	int spawnCount = 10;
	int spawnRetryCount = 0;
	int totalSpawnCount;
	int totalRespawnCount = 0;

	[SerializeField] List<int> nodeSetWhichSurfaceParent = new List<int>();
	[SerializeField] List<int> receiverSetWhichSurfaceParent = new List<int>();

	[SerializeField] List<GameObject> nodesSpawned;
	[SerializeField] List<GameObject> receiversSpawned;
	[SerializeField] List<GameObject> beamsSpawned;
	



	void Awake() {
		myRoom = transform.parent.gameObject;
		roomData = myRoom.GetComponent<MyRoomData>();

		//spawnCount = roomData.myWidth * roomData.myDepth / 2;
		totalSpawnCount = spawnCount;

		nodesSpawned = new List<GameObject>();
		receiversSpawned = new List<GameObject>();
		beamsSpawned = new List<GameObject>();

		SpawnComponentParents();

	}


	void Start () {
		//TOMAYBEDO Put this\/ in Awake()?
		//if (roomData.hasLasers) {
		//	SpawnStartsHere(spawnCount);

		//}

		player = GameObject.FindWithTag("Player");
	}


	void Update () {
		if (roomData.myBuildState == MyRoomData.myRoomBuildState.building) {
			SpawnStartsHere(spawnCount);
		}
		else if (roomData.myBuildState == MyRoomData.myRoomBuildState.finished) {
			
		}
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

		//print("Spawning this many: " + howManyToSpawn);
		SetSpawnParents(howManyToSpawn);
	}


	void SetSpawnParents(int newSpawnCount) {
		for (int i = 0; i < newSpawnCount; i++) {
			nodeSetWhichSurfaceParent.Add(Random.Range(0, roomData.myLaserSpawnSurfaces.Length));
			receiverSetWhichSurfaceParent.Add(Random.Range(0, roomData.myLaserSpawnSurfaces.Length));

			if (nodeSetWhichSurfaceParent.Count == spawnCount) {
				SpawnNodesAndReceivers(node, newSpawnCount);
			}
		}

		if(nodeSetWhichSurfaceParent.Count != spawnCount) {
			print("I didn't spawn enough SpawnParents");
		}

		/*for (int i = nodeSetWhichSurfaceParent.Count-1; i >= 0; i--) {
			//if (nodeSetWhichSurfaceParent[i] != null) {
				if (nodeSetWhichSurfaceParent[i] == receiverSetWhichSurfaceParent[i]) {
					nodeSetWhichSurfaceParent.RemoveAt(i);
					receiverSetWhichSurfaceParent.RemoveAt(i);
					spawnRetryCount++;
				}
			//}
			
		}

		//print("Total Respawns: " + totalRespawnCount + "  Total Spawns: " + totalSpawnCount);

		if (spawnRetryCount > 0) {
			//print(spawnRetryCount);
			totalSpawnCount += spawnRetryCount;
			totalRespawnCount += spawnRetryCount;
			//spawnCount = spawnRetryCount;
			SpawnStartsHere(spawnRetryCount);
		}
		else {
			SpawnNodesAndReceivers(node, spawnCount); //or maybe nodeSetWhichSpawnParent.Count
			SpawnNodesAndReceivers(receiver, spawnCount); //ditto
			SpawnBeams(newSpawnCount);
		}
		*/

		//SpawnNodesAndReceivers(node, newSpawnCount); //or maybe nodeSetWhichSpawnParent.Count
		//SpawnNodesAndReceivers(receiver, newSpawnCount); //ditto
		//SpawnBeams(newSpawnCount);
	}


	void SpawnNodesAndReceivers(GameObject component, int currentSpawnCount) {
		for (int l = spawnCount - currentSpawnCount; l < spawnCount; l++) {
			//print(l);
			GameObject compParent = null;
			if (component.tag == "LaserNode") {
				compParent = roomData.myLevel1Children[nodeSetWhichSurfaceParent[l]];
			}
			else if (component.tag == "LaserReceiver") {
				compParent = roomData.myLevel1Children[receiverSetWhichSurfaceParent[l]];
			}
			else {
				Debug.Log("I don't have the right tag");
				compParent = new GameObject();
			}

			GameObject selectedTile = compParent.transform.GetChild(Random.Range(0, compParent.transform.childCount)).gameObject;
			//TODO This is NOT random. Keep at it
			Vector3 randomPointOnSelectedTile = new Vector3(
				Random.Range(-selectedTile.GetComponent<MeshCollider>().bounds.extents.x,
								selectedTile.GetComponent<MeshCollider>().bounds.extents.x),
				Random.Range(0 /*because the wall objects' origins are at their base, not their center, and everything else is flat on the x-axis anyway*/,
								selectedTile.GetComponent<MeshCollider>().bounds.extents.y * 2),
				Random.Range(-selectedTile.GetComponent<MeshCollider>().bounds.extents.z,
								selectedTile.GetComponent<MeshCollider>().bounds.extents.z));


			GameObject newComponent = Instantiate(component, selectedTile.transform.position + randomPointOnSelectedTile, Quaternion.identity, gameObject.transform);

			if (component.tag == "LaserNode") {
				nodesSpawned.Add(newComponent);
				newComponent.transform.parent = nodeParent.transform;

				if (nodesSpawned.Count == spawnCount) {
					SpawnNodesAndReceivers(receiver, currentSpawnCount); //ditto

				}
			}
			else if (component.tag == "LaserReceiver") {
				receiversSpawned.Add(newComponent);
				newComponent.transform.parent = receiverParent.transform;

				if (receiversSpawned.Count == spawnCount) {
					SpawnBeams(currentSpawnCount);
				}
			}
			else {
				Debug.Log("I don't have the right tag");
			}
		}
	}


	void SpawnBeams (int myNewSpawnCount) {
		//print("Total Respawns: " + totalRespawnCount + "  Total Spawns: " + totalSpawnCount);

		for (int bts = spawnCount - myNewSpawnCount /*Pretty sure this is the missing number I was looking for. Don't fuck with it unless you know it's wrong*/; 
																																	bts < spawnCount; bts++) {
			//print("bts: " + bts);
				float beamScaleTotal = (receiversSpawned[bts].transform.position - nodesSpawned[bts].transform.position).magnitude;
				Vector3 beamPos = (nodesSpawned[bts].transform.position + receiversSpawned[bts].transform.position) / 2;

				GameObject newBeam = Instantiate(beam, beamPos, Quaternion.identity, beamParent.transform);
				beamsSpawned.Add(newBeam);
				newBeam.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, beamScaleTotal);
				newBeam.gameObject.transform.rotation = Quaternion.LookRotation(newBeam.transform.position - receiversSpawned[bts].transform.position);
		}
		
		
		for (int bsc = spawnCount - 1; bsc >= 0; bsc--) {
			//print("bsc: " + bsc);

			bool beamFuckedUp = false;

			if (nodeSetWhichSurfaceParent[bsc] == receiverSetWhichSurfaceParent[bsc]
							|| beamsSpawned[bsc].GetComponent<BoxCollider>().bounds.Intersects(player.GetComponent<CapsuleCollider>().bounds)) {
				//print("This beam spawned inside the player! Witness me!");

				beamFuckedUp = true;
			}
			else {
				if (roomData.beamBlockers.Count > 0) {
					foreach (GameObject blocker in roomData.beamBlockers) {
						if (beamsSpawned[bsc].GetComponent<BoxCollider>().bounds.Intersects(blocker.GetComponent<BoxCollider>().bounds)) {
							//print("This beam spawned inside a beamBlocker! Witness me!");

							beamFuckedUp = true;
						}
					}
				}
				

				/*foreach (GameObject doorway in roomData.myDoorways) {
					if (beamsSpawned[bsc].GetComponent<BoxCollider>().bounds.Intersects(doorway.GetComponent<BoxCollider>().bounds)) {
						print("This beam spawned inside a doorway! Witness me!");

						SetBeamToRetry(bsc);
					}
				}
				*/
			}

			if (beamFuckedUp == true) {
				SetBeamToRetry(bsc);

			}
		}


		if (spawnRetryCount > 0) {
			//print(spawnRetryCount);
			totalSpawnCount += spawnRetryCount;
			totalRespawnCount += spawnRetryCount;

			//spawnCount = spawnRetryCount;
			SpawnStartsHere(spawnRetryCount);
		}
		else {
			//Debug.Log("Room" + transform.parent.name + " successfully spawned");
			roomData.myBuildState = MyRoomData.myRoomBuildState.finished;
		}
	}


	void SetBeamToRetry(int beamIndex) {
		Destroy(nodesSpawned[beamIndex]);
		Destroy(receiversSpawned[beamIndex]);
		Destroy(beamsSpawned[beamIndex]);

		nodesSpawned.RemoveAt(beamIndex);
		receiversSpawned.RemoveAt(beamIndex);
		beamsSpawned.RemoveAt(beamIndex);

		nodeSetWhichSurfaceParent.RemoveAt(beamIndex);
		receiverSetWhichSurfaceParent.RemoveAt(beamIndex);

		spawnRetryCount++;

		//print("Spawns to Retry: " + spawnRetryCount);
	}


	
}
