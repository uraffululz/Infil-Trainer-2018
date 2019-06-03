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

	//End-Of-Level Achievement Variables
	Text moneyEarnedText;
	Text achievementsEarnedText;
	Text achievementMultiplierText;
	int AchievementMultiplier = 1;
	public static bool allLasersDisabled = false;
	bool allTreasuresCollected = false;
	public static bool noAlarmsActivated = true;

	public float levelParTime;
	public static bool levelTimeBelowPar = true;
	


	void Awake() {
		player = GameObject.FindWithTag("Player");

		//Initialize UI Elements
		moneyEarnedText = UIWin.transform.Find("MoneyEarnedText").GetComponent<Text>();
		achievementsEarnedText = UIWin.transform.Find("AchievementsEarnedText").GetComponent<Text>();
		achievementMultiplierText = UIWin.transform.Find("AchievementMultiplierText").GetComponent<Text>();

		timerCurrentTime = timerMaxxTime;
		levelParTime = LevelBuilder.maxRoomNum;
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
		CompileEndOfLevelAchievements();

		//Update UI
		CanvasManager.isTimerActive = false;
		UIPlayer.SetActive(false);
		UIWin.SetActive(true);

		moneyEarnedText.text = ("$" + canvasManager.GetComponent<CanvasManager>().score * AchievementMultiplier);

		//Disable player character movement
		player.GetComponent<PlayerMove>().allowMove = false;
	}


	void CompileEndOfLevelAchievements() {
		//WinBox winBoxScript = GameObject.Find("WinBox").GetComponent<WinBox>();

		//Determine if all of the level's lasers have been disabled
		if (allLasersDisabled) {
			//Activate "All Lasers Disabled" achievement
			AchievementMultiplier++;
			achievementsEarnedText.text += ("All Lasers Disabled" + System.Environment.NewLine);
		}

		//Determine if any alarms were triggered by the player
		if (noAlarmsActivated) {
			//Activate "No Alarms Activated" achievement
			AchievementMultiplier++;
			achievementsEarnedText.text += ("No Alarms Activated" + System.Environment.NewLine);
		}

		//Determine if the player completed the level below "par" time
		if (levelTimeBelowPar) {
			//Activate "Level Time Below Par" achievement
			AchievementMultiplier++;
			achievementsEarnedText.text += ("Level Time Below Par" + System.Environment.NewLine);
		}

		//Determine if the player picked up all treasures/pickups in the level
		if (gameObject.GetComponent<LevelBuilder>().levelTreasures.Count <= 0) {
			//Activate "All Treasures Collected" achievement
			allTreasuresCollected = true;
			AchievementMultiplier++;
			achievementsEarnedText.text += ("All Treasures Collected" + System.Environment.NewLine);
		}

//TODO Add "No Retries Used On Puzzles Or Locks" Achievement

		//Determine if the player ACHIEVED ALL ACHIEVEMENTS
		if (allLasersDisabled && noAlarmsActivated && levelTimeBelowPar && allTreasuresCollected) {
			AchievementMultiplier = 10;
			achievementsEarnedText.text += ("ALL ACHIEVEMENTS ACHIEVED");
		}

		achievementMultiplierText.text = ("Achievement Multiplier: " + AchievementMultiplier);
	}


	public void ReturnToIntermission_Lose() {
		SceneManager.LoadScene("InterMission");
	}


	public void ReturnToInterMission_Win() {
		SceneManager.LoadScene("InterMission");
	}
}
