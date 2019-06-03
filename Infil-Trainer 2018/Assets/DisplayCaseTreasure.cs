using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCaseTreasure : MonoBehaviour {

	[SerializeField] GameObject[] possibleTreasures; //Populate and use this later to randomly determine which treasure is spawned within each case
	public GameObject selectedTreasure;


	void Awake () {
		selectedTreasure = GameObject.CreatePrimitive(PrimitiveType.Cube);
		selectedTreasure.transform.localScale = Vector3.one * .1f;
		selectedTreasure.transform.parent = transform.GetChild(0);
		selectedTreasure.name = "TreasureCube";
		selectedTreasure.transform.position = transform.GetChild(0).transform.position;
	}


	void Update () {
		
	}
}
