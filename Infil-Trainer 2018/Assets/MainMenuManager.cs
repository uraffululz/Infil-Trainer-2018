using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

	[SerializeField] Canvas mainCanvas;
	[SerializeField] GameObject mainParent;
	[SerializeField] GameObject optionParent;


	void Start () {
		
	}


	void Update () {
		
	}

	public void LoadTestScene() {
		SceneManager.LoadScene("TestScene");
	}


	public void OptionMenuOpen() {
		mainParent.SetActive(false);
		optionParent.SetActive(true);
	}


	public void OptionMenuClose() {
		mainParent.SetActive(true);
		optionParent.SetActive(false);
	}
}
