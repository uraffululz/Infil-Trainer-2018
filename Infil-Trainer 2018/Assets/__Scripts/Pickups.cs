using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour {

	ScoreTracker scoreScript;

	Vector3 startPos;
	float moveSpeed;


	void Awake () {
		scoreScript = GameObject.Find ("LevelManager").GetComponent<ScoreTracker> ();
		startPos = transform.position;
	}


	void Start () {
		
	}
	

	void Update () {
		Movement ();
	}


	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			Destroy (gameObject);
			scoreScript.score += 100;
		}
	}


	void Movement () {
		moveSpeed += 20 * Time.deltaTime;
		transform.rotation = Quaternion.Euler (0.0f, moveSpeed, 0.0f);

		//Vector3 topPos = new Vector3 (transform.position.x, 0.3f, transform.position.z);
		Vector3 newPos = startPos + Vector3.up * 0.1f * Mathf.Sin (Time.timeSinceLevelLoad);
		transform.position = newPos;
	}
}
