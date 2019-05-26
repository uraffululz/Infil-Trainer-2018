using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room Object", menuName = "New Room Type", order = 51)]
public class RoomStats : ScriptableObject {

	public int roomWidth;
	public int roomDepth;
	public int roomHeight;

	public bool allowFloorTreasure;
	public bool allowWallTreasure;
	public bool allowPickups;

	public bool allowLasers;
}
