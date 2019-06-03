using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour {

	GameObject levMan;
	CanvasManager cMan;

	Vector3 startPos;
	float moveSpeed;

	int myWorth = 100;


	void Awake () {
		levMan = GameObject.Find("LevelManager");
		cMan = GameObject.Find ("CanvasManager").GetComponent<CanvasManager> ();
		startPos = transform.position + Vector3.up * 0.3f;
	}


	void Start () {
		
	}
	

	void Update () {
		Movement ();
	}


	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			levMan.GetComponent<LevelBuilder>().levelTreasures.Remove(gameObject);
			cMan.AddToScore(myWorth);
			Destroy(gameObject);
		}
	}


	void Movement () {
		moveSpeed += 20 * Time.deltaTime;
		transform.rotation = Quaternion.Euler (0.0f, moveSpeed, 0.0f);

		Vector3 newPos = startPos + Vector3.up * 0.1f * Mathf.Sin (Time.timeSinceLevelLoad);
		transform.position = newPos;
	}
}
