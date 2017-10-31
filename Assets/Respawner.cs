using UnityEngine;
using System.Collections;

public class Respawner : MonoBehaviour {

	void Death() {
		int playerNumber = GetComponentInParent<Player>().playerNumber;
		int lives = UIManager.instance.GetLives(playerNumber);
		UIManager.instance.SetLives(playerNumber, --lives);
		if(lives > 0) {
			GameObject[] spawners = GameObject.FindGameObjectsWithTag("PlayerSpawner");

			int i = Random.Range (0, spawners.Length);

			spawners[i].GetComponent<PlayerSpawner>().SpawnPlayer(playerNumber);

		}

		Destroy(gameObject);
	}
}
