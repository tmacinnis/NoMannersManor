using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public PlayerUIScript[] playerUI;
	int[] playerScore = new int[4];

	public static UIManager instance = null;
	public Button quitButton;
	public bool buttonShown;
	public Button pauseButton;
	public bool pauseButtonShown;

	void Awake() {
		for (int i = 0; i < 4; i++) {
			playerScore [i] = 0;
		}
		if(instance == null) {
			instance = this;
		}
		else if(instance != this) {
			Destroy(gameObject);
		}

		for(int i = 0; i < playerUI.Length; i++) {
			SetLives(i + 1, 3);
		}
		pauseButtonShown = false;
		buttonShown = false;
		//quitButton.gameObject.SetActive(true);

	}

	public void incrementScore(int playerNumber){
		playerScore [playerNumber - 1]++;
	}

	public void SetAmmo(int playerNumber, int ammoNumber) {
		playerUI[playerNumber - 1].SetAmmo(ammoNumber);
	}

	public void SetHealth(int playerNumber, int healthNumber) {
		playerUI[playerNumber - 1].SetHealth(healthNumber);
	}
	public void SetLives(int playerNumber, int livesNumber) {
		playerUI[playerNumber - 1].SetLives(livesNumber);
	}
	public int GetLives(int PlayerNumber) {
		return playerUI[PlayerNumber - 1].GetLives();
	}
	public void SetPowerup(int PlayerNumber, Player.PowerUp p) {
		playerUI[PlayerNumber - 1].SetPowerUp(p);
	}
	public void DisablePlayerUI(int playerNumber) {
		if (playerUI.Length != 1) {
			playerUI [playerNumber - 1].SetLives (0);
			foreach (Image i in playerUI[playerNumber - 1].GetComponentsInChildren<Image>()) {
				if (i != this.gameObject) {
					i.enabled = false;
				}

			}
		}
		//playerUI[playerNumber].enabled = false;
	}

	void Update() {
		int winningPlayer =  0;
		switch (playerUI.Length) {
		case 1:
			if (playerUI [0].GetLives () == 0) {
				winningPlayer = 1;
			}
			break;
		case 2:
			if (playerUI [0].GetLives () > 0 && playerUI [1].GetLives () == 0) {
				winningPlayer = 1;
				Debug.Log ("player 1 wins");
			} else if (playerUI [1].GetLives () > 0 && playerUI [0].GetLives () == 0) {
				winningPlayer = 2;
			}
			break;
		case 3:
			if (playerUI [0].GetLives () > 0 && playerUI [1].GetLives () == 0 && playerUI [2].GetLives () == 0) {
				winningPlayer = 1;
				Debug.Log ("player 1 wins");
			} else if (playerUI [1].GetLives () > 0 && playerUI [0].GetLives () == 0 && playerUI [2].GetLives () == 0) {
				winningPlayer = 2;
			} else if (playerUI [2].GetLives () > 0 && playerUI [0].GetLives () == 0 && playerUI [1].GetLives () == 0) {
				winningPlayer = 3;
			}
			break;
		case 4:		
			if (playerUI [0].GetLives () > 0 && playerUI [1].GetLives () == 0 && playerUI [2].GetLives () == 0 && playerUI [3].GetLives () == 0) {
				winningPlayer = 1;
				Debug.Log ("player 1 wins");
			} else if (playerUI [1].GetLives () > 0 && playerUI [0].GetLives () == 0 && playerUI [2].GetLives () == 0 && playerUI [3].GetLives () == 0) {
				winningPlayer = 2;
			} else if (playerUI [2].GetLives () > 0 && playerUI [0].GetLives () == 0 && playerUI [1].GetLives () == 0 && playerUI [3].GetLives () == 0) {
				winningPlayer = 3;
			} else if (playerUI [3].GetLives () > 0 && playerUI [0].GetLives () == 0 && playerUI [1].GetLives () == 0 && playerUI [2].GetLives () == 0) {
				winningPlayer = 4;
			}
			break;
		}
		if(winningPlayer != 0 && buttonShown == false) {
			Debug.Log("showing ending dialog");
			Time.timeScale = 0;
			Cursor.visible = true;
			buttonShown = true;
			quitButton.gameObject.SetActive(true);
			quitButton.enabled = false;	
			quitButton.enabled = true;
			quitButton.interactable = true;
			if(playerUI.Length == 1)
				quitButton.GetComponentInChildren<Text>().text = "Player " + winningPlayer +" wins!\nScore: " + playerScore[0] ;
			if(playerUI.Length == 1)
				
			winningPlayer = -1;
		}

		if(Input.GetButtonDown("Pause") && buttonShown == false) {

			if (pauseButtonShown == false) {
				Debug.Log("Pause");
				Time.timeScale = 0;
				Cursor.visible = true;
				pauseButtonShown = true;
				pauseButton.gameObject.SetActive(true);	
				pauseButton.enabled = true;
				pauseButton.interactable = true;
			}
			else {
				Time.timeScale = 1;
				Cursor.visible = false;
				pauseButtonShown = false;
				pauseButton.gameObject.SetActive(false);
				pauseButton.enabled = false;	
				pauseButton.interactable = false;
			}
		}

	}
}
