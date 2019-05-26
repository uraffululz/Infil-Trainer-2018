using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	//Game Scene Object Variables
	[Header("Object References")]
	[SerializeField] Light sceneLight;
	GameObject player;

	//Scene UI Variables
	[Header("UI Variables")]
	[SerializeField] GameObject canvasManager;
	[SerializeField] GameObject UIPlayer;
	[SerializeField] Image timerBar;
	[SerializeField] GameObject UILose;
	[SerializeField] GameObject UIWin;

	//Laser Countdown Timer Variables
	[Header("Timer Variables")]
	[SerializeField] float timerCurrentTime;
	float timerMaxxTime = 10.0f; //Increase later to 30 seconds? 1 minute? Some other amount of time, as calculated by the size of the level? Other?
	[SerializeField] bool colorLerpToRed = false;
	[SerializeField] bool laserTimeoutGameOver = false;
	public enum TimerOn {timerDeactivated, timerActivated};
	public static TimerOn timerState;
	


	void Awake() {
		player = GameObject.FindWithTag("Player");

		timerCurrentTime = timerMaxxTime;
	}


	void Update () {
		if (timerState == TimerOn.timerActivated) {
			TimerActive();
		}
	}


	void TimerActive() {
		//While the level's "alarm timer" is currently active:
		//Enable/show the Timer Bar on the player's UI
		timerBar.gameObject.SetActive(true);

		if (timerCurrentTime <= 0) {
			//The level's alarm timer has run out. You lose.
			GameOver();
		}
		else {
			timerCurrentTime -= Time.deltaTime;

			//Detect whether the scene lighting should be lerping to/from red or black
			if (sceneLight.color.r > 0.95f) {
				colorLerpToRed = false;
			}
			else if (sceneLight.color.r < 0.05f) {
				colorLerpToRed = true;
			}

			//Lerp the color of the scene lighting to the appropriate color
			if (colorLerpToRed) {
				sceneLight.color = Color.Lerp(sceneLight.color, Color.red, 1.0f * Time.deltaTime);
			}
			else {
				sceneLight.color = Color.Lerp(sceneLight.color, Color.black, 1.0f * Time.deltaTime);
			}
		}

		//Determine how "Full" the level's Timer bar should be, as the timer is counting down
		timerBar.fillAmount = timerCurrentTime / timerMaxxTime;

	}


	public void DeactivateCurrentlyActiveTimer() {
		//Disable the level's active alarm, resetting the relevant timer and lighting color values and disabling/hiding the Timer Bar
		timerState = TimerOn.timerDeactivated;

		timerCurrentTime = timerMaxxTime;

		sceneLight.color = Color.white;

		timerBar.fillAmount = timerCurrentTime / timerMaxxTime;
		timerBar.gameObject.SetActive(false);
		}


	void GameOver() {
//TODO Enable and Animate (from off-screen to screen-center) GAME OVER UI Menu
		DeactivateCurrentlyActiveTimer();
		laserTimeoutGameOver = true;
		UIPlayer.SetActive(false);
		UILose.SetActive(true);

//TODO Reinstate this when necessary. Disabling now for testing/development purposes
		//Disable player character movement
		player.GetComponent<PlayerMove>().allowMove = false;
	}


	public void Win() {
		//Update UI
		canvasManager.GetComponent<CanvasManager>().isTimerActive = false;
		UIPlayer.SetActive(false);
		UIWin.SetActive(true);

		//Disable player character movement
		player.GetComponent<PlayerMove>().allowMove = false;
	}


	public void ReturnToIntermission_Lose() {
		SceneManager.LoadScene("InterMission");
	}


	public void ReturnToInterMission_Win() {
		SceneManager.LoadScene("InterMission");
	}
}
