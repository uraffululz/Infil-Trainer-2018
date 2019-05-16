using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	//Game Scene Variables
	[SerializeField] GameObject canvasManager;
	[SerializeField] GameObject timerBar;
	[SerializeField] GameObject timeUpText;
	[SerializeField] Light sceneLight;

	//Laser Countdown Timer Variables
	public enum TimerOn { timerDeactivated, timerActivated };
	public static TimerOn timerState;
	float timerMaxxTime = 30.0f; //Increase later to 30 seconds? 1 minute? Some other amount of time, as calculated by the size of the level? Other?
	[SerializeField] float timerCurrentTime;
	[SerializeField] bool laserTimeoutGameOver = false;
	[SerializeField] bool colorLerpToRed = false;



	void Awake() {
		timerCurrentTime = timerMaxxTime;
	}


	void Start () {
		
	}


	void Update () {
		if (timerState == TimerOn.timerActivated) {
			TimerActive();
		}
	}


	void TimerActive() {
		//print("Laser Countdown timer Activated");

		timerBar.SetActive(true);


		if (timerCurrentTime <= 0) {
			//timerCurrentTime = 0;

			DeactivateCurrentlyActiveTimer();
			laserTimeoutGameOver = true;
			timeUpText.SetActive(true);

			print("Timer ran out. Game over.");

//TODO Reinstate this when necessary. Disabling now for testing/development purposes
			//player.GetComponent<PlayerMove>().allowMove = false;
		}
		else {
			timerCurrentTime -= Time.deltaTime;

			if (sceneLight.GetComponent<Light>().color.r > 0.9f) {
				colorLerpToRed = false;
			}
			else if (sceneLight.GetComponent<Light>().color.r < 0.1f) {
				colorLerpToRed = true;
			}

			if (colorLerpToRed) {
				sceneLight.GetComponent<Light>().color = Color.Lerp(sceneLight.GetComponent<Light>().color, Color.red, 1.0f * Time.deltaTime);
			}
			else {
				sceneLight.GetComponent<Light>().color = Color.Lerp(sceneLight.GetComponent<Light>().color, Color.black, 1.0f * Time.deltaTime);
			}
		}

		timerBar.GetComponent<Image>().fillAmount = timerCurrentTime / timerMaxxTime;

	}


	public void DeactivateCurrentlyActiveTimer() {
		timerState = TimerOn.timerDeactivated;
		
		sceneLight.GetComponent<Light>().color = Color.white;

		timerCurrentTime = timerMaxxTime;
		timerBar.GetComponent<Image>().fillAmount = 1f;
		timerBar.SetActive(false);
		}
}
