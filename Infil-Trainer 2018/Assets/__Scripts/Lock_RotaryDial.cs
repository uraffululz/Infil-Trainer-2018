using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock_RotaryDial : MonoBehaviour {

	LockManager lockMan;
	Camera mainCam;

	[SerializeField] GameObject plate;
	[SerializeField] GameObject dial;
	enum whichDir {right, left};
	whichDir thisDir = whichDir.right;

	GameObject rotCam;
	Camera camCom;


	void Awake () {
		lockMan = gameObject.GetComponentInParent<LockManager> ();
		mainCam = GameObject.FindWithTag ("MainCamera").GetComponent<Camera> ();

		RotarySetup ();
		NumberSetup ();
		RotaryCamera ();
	}


	void OnEnable () {
		mainCam.enabled = false;
		camCom.enabled = true;
	}


	void Start () {
		
	}
	

	void Update () {
		RotateLock ();
	}


	void RotarySetup () {
		Vector3 lockOffset = new Vector3 (0.0f, -200.0f, 0.0f);
		if (GameObject.Find ("BackPlate") != null) {
			lockOffset.x += 5.0f;
		}

		plate = Instantiate (plate, lockOffset, Quaternion.identity, transform);
		plate.name = "BackPlate";

		dial = Instantiate (dial, lockOffset + Vector3.right * 0.2f, Quaternion.identity, plate.transform);
		dial.name = "RotaryDial";
	}


	void NumberSetup () {
		int dialNum1 = Random.Range (0, 360);
		int dialNum2 = Random.Range (0, 360);
		int dialNum3 = Random.Range (0, 360);

/*TODO Add the 3 selected numbers to an array/list
 * 
*/
	}


	Camera RotaryCamera () {
		rotCam = new GameObject ();
		rotCam.name = "RotaryCam";
		camCom = rotCam.AddComponent<Camera>();
		camCom.CopyFrom (mainCam);
		camCom.transform.rotation = Quaternion.FromToRotation (mainCam.transform.rotation.eulerAngles, Vector3.zero);

		rotCam.transform.position = plate.transform.position + Vector3.back * 1.0f;
		rotCam.transform.parent = plate.transform;

		//mainCam.enabled = false;
		camCom.enabled = false;

		//camCom = glassCam.GetComponent<Camera> ();
		return camCom;
	}


	void RotateLock () {
		if (Input.GetKey(KeyCode.RightArrow)) {
			if (thisDir == whichDir.right) {
				
			} else if (thisDir == whichDir.left) {
				//Wrong direction
			}
			
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			if (thisDir == whichDir.right) {
				//Wrong direction

			} else if (thisDir == whichDir.left) {

			}
		}
/*TODO Use left-right keyboard inputs to rotate lock (Start puzzle by rotating to the right)
 * If the correct number is landed on, indicate to the player that the lock should be rotated in the opposite direction
 * If the correct number is passed, or the player rotates the lock in the wrong direction, the puzzle attempt is failed
*/
	}
}
