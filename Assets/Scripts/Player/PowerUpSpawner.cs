using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour {

	public GameObject[] powerups;

	float timer = 0;
	public int waitTime = 5;
	bool powerUpPresent = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		timer += Time.deltaTime;

		if ((timer > waitTime) && powerUpPresent == false ) {
			SpawnPowerUp();

			timer = 0; 
		}
	}

	void SpawnPowerUp() {
		int i = Random.Range(0, powerups.Length);

		GameObject spawnedPowerUp = Instantiate(powerups[i], transform.position, transform.rotation) as GameObject;

		powerUpPresent = true;
	}

	void OnPickedUpPowerUp() {
			Debug.Log("Picked Up a Power Up");
			powerUpPresent = false;
			timer = 0;
	}
}
