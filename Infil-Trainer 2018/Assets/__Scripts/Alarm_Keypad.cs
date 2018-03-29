using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Alarm_Keypad : MonoBehaviour {

	Camera mainCam;
	AlarmManager alarmMan;
	[SerializeField] GameObject canvas;
	[SerializeField] GameObject button;

	[SerializeField] GameObject keypad;
	GameObject padCamEmpty;
	Camera padCam;

	enum padStatus {choosing, solved, failed, unsolved};
	padStatus padStat;

	[SerializeField] int[] digits = new int[4];
	[SerializeField] List<string> guesses = new List<string>();
	[SerializeField] string correctGuess;

	public List<GameObject> Buttons;
	GameObject clickedButton;
	Text clickedButtonText;

	int attemptsLeft = 3;


	void Awake () {
		mainCam = Camera.main;
		alarmMan = gameObject.GetComponent<AlarmManager> ();
		canvas = GameObject.Find ("LevelManager").GetComponent<CanvasManager> ().canvas;

		KeypadSetup ();
		SetNumbers ();
	}


	void OnEnable () {
		padStat = padStatus.choosing;

		padCam.enabled = true;
		mainCam.enabled = false;

		foreach (var button in Buttons) {
			button.SetActive (true);
		}
	}


	void Start () {
		SetupButtons ();
	}
	

	void Update () {
		if (Input.GetKeyDown(KeyCode.Q)) {
//This leaves the puzzle "unsolved" (and no longer "inProgress" for now). Later on, there will be alternate states for: 
			padStat = padStatus.unsolved;
		} else if (Input.GetKeyDown(KeyCode.R)) {
//"solved", meaning the player succeeded in solving the puzzle within the allotted number of attempts 
			padStat = padStatus.solved;
		} else if (Input.GetKeyDown(KeyCode.F)) {
//"failed", meaning the player failed to solve the puzzle within the allotted number of attempts 
			padStat = padStatus.failed;
		}


		if (padStat == padStatus.choosing) {
			//ClickButton ();
		} else if (padStat == padStatus.solved) {
			Solved ();
		} else if (padStat == padStatus.failed) {
			Failed ();
		} else if (padStat == padStatus.unsolved) {
			Unsolved ();
		}
	}


	void KeypadSetup () {
		Vector3 padPos = new Vector3 (0.0f, 100.0f, 0.0f);
		keypad = Instantiate (keypad, padPos, Quaternion.identity, gameObject.transform);

		padCamEmpty = new GameObject ();
		padCamEmpty.name = "KeypadCam";

		padCam = padCamEmpty.AddComponent<Camera> ();
		padCam.CopyFrom (mainCam);
		padCam.enabled = false;

		padCamEmpty.transform.position = padPos + Vector3.back;
		padCamEmpty.transform.parent = keypad.transform;
	}


	void SetNumbers () {
		digits[0] = Random.Range(0, 10);
		digits[1] = Random.Range(0, 10);
		while (digits[1] == digits[0]) {
			digits [1] = Random.Range (0, 10);
		}
		digits[2] = Random.Range(0, 10);
		while (digits[2] == digits[0] || digits[2] == digits[1]) {
			digits [2] = Random.Range (0, 10);
		}
		digits[3] = (Random.Range(0, 10));
		while (digits[3] == digits[0] || digits[3] == digits[1] || digits[3] == digits[2]) {
			digits[3] = Random.Range (0, 10);
		}
		//print ("Keypad digits determined. Compiling digit sequences");

		//Compile sequences using digits
		guesses.Add(string.Concat(digits[0], digits[1], digits[2], digits[3]));
		guesses.Add(string.Concat(digits[0], digits[1], digits[3], digits[2]));
		guesses.Add(string.Concat(digits[0], digits[2], digits[1], digits[3]));
		guesses.Add(string.Concat(digits[0], digits[2], digits[3], digits[1]));
		guesses.Add(string.Concat(digits[0], digits[3], digits[1], digits[2]));
		guesses.Add(string.Concat(digits[0], digits[3], digits[2], digits[1]));

		guesses.Add(string.Concat(digits[1], digits[0], digits[2], digits[3]));
		guesses.Add(string.Concat(digits[1], digits[0], digits[3], digits[2]));
		guesses.Add(string.Concat(digits[1], digits[2], digits[0], digits[3]));
		guesses.Add(string.Concat(digits[1], digits[2], digits[3], digits[0]));
		guesses.Add(string.Concat(digits[1], digits[3], digits[0], digits[2]));
		guesses.Add(string.Concat(digits[1], digits[3], digits[2], digits[0]));

		guesses.Add(string.Concat(digits[2], digits[0], digits[1], digits[3]));
		guesses.Add(string.Concat(digits[2], digits[0], digits[3], digits[1]));
		guesses.Add(string.Concat(digits[2], digits[1], digits[0], digits[3]));
		guesses.Add(string.Concat(digits[2], digits[1], digits[3], digits[0]));
		guesses.Add(string.Concat(digits[2], digits[3], digits[0], digits[1]));
		guesses.Add(string.Concat(digits[2], digits[3], digits[1], digits[0]));

		guesses.Add(string.Concat(digits[3], digits[0], digits[1], digits[2]));
		guesses.Add(string.Concat(digits[3], digits[0], digits[2], digits[1]));
		guesses.Add(string.Concat(digits[3], digits[1], digits[0], digits[2]));
		guesses.Add(string.Concat(digits[3], digits[1], digits[2], digits[0]));
		guesses.Add(string.Concat(digits[3], digits[2], digits[0], digits[1]));
		guesses.Add(string.Concat(digits[3], digits[2], digits[1], digits[0]));

		//print ("Keypad sequences determined. Choosing correct code");

		//Determine correct keypad code by randomly choosing a sequence from guesses[]
		correctGuess = guesses[Random.Range(0, guesses.Count)];

		//print ("Keycode chosen");
	}


	void SetupButtons () {
		//print ("Populating Buttons array");
		Vector3 buttPos = new Vector3 (150f, 450f, 0f);
		int buttonsInLine = 0;

		foreach (var guess in guesses) {
			button = Instantiate (button, canvas.transform);
			Button buttonComp = button.GetComponent<Button> ();
			RectTransform buttRect = button.GetComponent<RectTransform> ();

			Buttons.Add (button);

			buttRect.position = buttPos;
			buttonsInLine++;

			if (buttonsInLine >= 6) {
				buttPos.x = 150;
				buttPos.y -= 70f;
				buttonsInLine = 0;
			} else {
				buttPos.x += 100f;
			}
			//print ("Button positions set");
		}

		foreach (var button in Buttons) {
			button.GetComponent<Button> ().onClick.AddListener (ClickButton);
			int assignedGuess = (Random.Range (0, guesses.Count));
			Text buttonText = button.GetComponentInChildren<Text> ();
			buttonText.text = guesses [assignedGuess];
			guesses.RemoveAt (assignedGuess);

			//print ("Guess applied to button");
		}
		//print ("Button setup complete");
	}


	public void ClickButton () {
		clickedButton = EventSystem.current.currentSelectedGameObject;
		if (clickedButton != null) {
			clickedButtonText = clickedButton.GetComponent<Button> ().GetComponentInChildren<Text> ();
			if (clickedButtonText.text == correctGuess) {
				clickedButtonText.text = "<color=green>" + clickedButtonText.text + "</color>";
				Debug.Log ("You guessed right. YOU WIN");
				padStat = padStatus.solved;
			} else {
				Debug.Log ("You guessed wrong");
				attemptsLeft--;
				if (attemptsLeft <= 0) {
					Debug.Log ("You lose");
					padStat = padStatus.failed;
				} else {
					CompareDigits ();
				}
			}
		}
	}


	void CompareDigits () {
		int correctlyPlacedDigits = 0;

		string guessDigit1 = clickedButtonText.text.Substring (0, 1);
		string guessDigit2 = clickedButtonText.text.Substring (1, 1);
		string guessDigit3 = clickedButtonText.text.Substring (2, 1);
		string guessDigit4 = clickedButtonText.text.Substring (3, 1);


		if (guessDigit1 == correctGuess.Substring(0, 1)) {
			correctlyPlacedDigits++;
			guessDigit1 = "<color=green>"+guessDigit1+"</color>";
		}
		if (guessDigit2 == correctGuess.Substring(1, 1)) {
			correctlyPlacedDigits++;
			guessDigit2 = "<color=green>"+guessDigit2+"</color>";
		}
		if (guessDigit3 == correctGuess.Substring (2, 1)) {
			correctlyPlacedDigits++;
			guessDigit3 = "<color=green>"+guessDigit3+"</color>";
		}
		if (guessDigit4 == correctGuess.Substring(3, 1)) {
			correctlyPlacedDigits++;
			guessDigit4 = "<color=green>"+guessDigit4+"</color>";
		}
		clickedButtonText.text = guessDigit1 + guessDigit2 + guessDigit3 + guessDigit4;


	}


	void Solved () {
		foreach (var button in Buttons) {
			Destroy (button);
		}
		Destroy (keypad);
		padCam.enabled = false;
		mainCam.enabled = true;
		alarmMan.bStat = AlarmManager.boxStatus.solved;
		Destroy (this);
	}


	void Failed () {
		foreach (var button in Buttons) {
			Destroy (button);
		}
		Destroy (keypad);
		padCam.enabled = false;
		mainCam.enabled = true;
		alarmMan.bStat = AlarmManager.boxStatus.failed;
		Destroy (this);
	}


	void Unsolved () {
		foreach (var button in Buttons) {
			button.SetActive (false);
		}
		padCam.enabled = false;
		mainCam.enabled = true;
		alarmMan.bStat = AlarmManager.boxStatus.unsolved;
		this.enabled = false;
	}
}
