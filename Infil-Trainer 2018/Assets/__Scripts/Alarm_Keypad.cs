using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Alarm_Keypad : MonoBehaviour {

	[SerializeField] Camera mainCam;
	AlarmManager alarmMan;
	[SerializeField] GameObject canvas;
	[SerializeField] GameObject buttonPrefab;

	[SerializeField] GameObject keypad;
	GameObject padCamEmpty;
	Camera padCam;

	enum padStatus {choosing, solved, failed, unsolved};
	padStatus padStat;

	[SerializeField] int[] digits = new int[4];
	[SerializeField] List<string> guesses = new List<string>();
	[SerializeField] string correctGuess;

	GameObject buttonParent;
	public List<GameObject> Buttons;
	GameObject clickedButton;
	Text clickedButtonText;

	int attemptsLeft = 4;

//TOMAYBEDO Keep the level's alarm timer counting down, to maintain tension and create a more balanced risk-reward dynamic?


	void Awake () {
		mainCam = Camera.main;
		alarmMan = gameObject.GetComponent<AlarmManager> ();
		canvas = GameObject.Find ("CanvasManager").GetComponent<CanvasManager> ().canvas;

		buttonParent = new GameObject("ButtonParent");
		buttonParent.transform.parent = canvas.transform;
		buttonParent.transform.position = canvas.transform.position;

		KeypadSetup ();
		SetNumbers ();
		SetupButtons();
	}


	void OnEnable () {
		padStat = padStatus.choosing;

		padCam.enabled = true;
		mainCam.enabled = false;

		buttonParent.SetActive(true);

		SetNumbers();
		AssignGuessesToButtons(false);

		attemptsLeft = 4;
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
		padCam.farClipPlane = 1f;
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

		//Determine correct keypad code by randomly choosing a sequence from guesses[]
		correctGuess = guesses[Random.Range(0, guesses.Count)];

	}


	void SetupButtons () {
		Vector3 buttPos = new Vector3 (/*about -384f at 1080p resolution*/-canvas.GetComponent<RectTransform>().rect.width * .2f, 200f, 0f);
		int buttonsInLine = 0;

		foreach (var guess in guesses) {
			GameObject newButton = Instantiate (buttonPrefab, buttonParent.transform);
			Button buttonComp = newButton.GetComponent<Button> ();
			RectTransform buttRect = newButton.GetComponent<RectTransform> ();

			Buttons.Add (newButton);

			buttRect.position = buttonParent.transform.position + buttPos;
			buttonsInLine++;

			if (buttonsInLine >= 6) {
				buttPos.x = -canvas.GetComponent<RectTransform>().rect.width * .2f;
				buttPos.y -= 100f;
				buttonsInLine = 0;
			} else {
				buttPos.x += 150f;
			}
		}

		AssignGuessesToButtons(true);

		buttonParent.SetActive(false);
	}


	void AssignGuessesToButtons (bool isThisTheInitialSetup) {
		foreach (var thisButton in Buttons) {
			if (isThisTheInitialSetup) {
				thisButton.GetComponent<Button>().onClick.AddListener(ClickButton);
			}
			int assignedGuess = (Random.Range(0, guesses.Count));
			Text buttonText = thisButton.GetComponentInChildren<Text>();
			buttonText.text = guesses[assignedGuess];
			guesses.RemoveAt(assignedGuess);
		}
	}


	public void ClickButton () {
		clickedButton = EventSystem.current.currentSelectedGameObject;
		if (clickedButton != null) {
			clickedButtonText = clickedButton.GetComponent<Button> ().GetComponentInChildren<Text> ();
			if (clickedButtonText.text == correctGuess) {
				clickedButtonText.text = "<color=green>" + clickedButtonText.text + "</color>";
				padStat = padStatus.solved;
			} else {
				Debug.Log ("You guessed wrong");
				attemptsLeft--;
				if (attemptsLeft <= 0) {
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
		} else {
			guessDigit1 = "<color=red>" + guessDigit1 + "</color>";
		}

		if (guessDigit2 == correctGuess.Substring(1, 1)) {
			correctlyPlacedDigits++;
			guessDigit2 = "<color=green>"+guessDigit2+"</color>";
		}
		else {
			guessDigit2 = "<color=red>" + guessDigit2 + "</color>";
		}
		if (guessDigit3 == correctGuess.Substring (2, 1)) {
			correctlyPlacedDigits++;
			guessDigit3 = "<color=green>"+guessDigit3+"</color>";
		}
		else {
			guessDigit3 = "<color=red>" + guessDigit3 + "</color>";
		}
		if (guessDigit4 == correctGuess.Substring(3, 1)) {
			correctlyPlacedDigits++;
			guessDigit4 = "<color=green>"+guessDigit4+"</color>";
		}
		else {
			guessDigit4 = "<color=red>" + guessDigit4 + "</color>";
		}
		clickedButtonText.text = guessDigit1 + guessDigit2 + guessDigit3 + guessDigit4;
	}


	void Solved () {
		buttonParent.SetActive(false);
		padCam.enabled = false;
		mainCam.enabled = true;
		alarmMan.bStat = AlarmManager.boxStatus.solved;
		Debug.Log("You guessed right");

		this.enabled = false;
	}


	void Failed () {
		Destroy(buttonParent);
		Destroy (keypad);
		padCam.enabled = false;
		mainCam.enabled = true;
		alarmMan.bStat = AlarmManager.boxStatus.failed;
		Debug.Log("You failed to guess the right keypad combo. Get out NOW!");

		Destroy(this);
	}


	void Unsolved () {
		buttonParent.SetActive(false);
		padCam.enabled = false;
		mainCam.enabled = true;
		alarmMan.bStat = AlarmManager.boxStatus.unsolved;
		this.enabled = false;
	}
}
