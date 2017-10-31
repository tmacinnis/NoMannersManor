using UnityEngine;
using System.Collections;
using System;

public class EnemySpawning : MonoBehaviour {

	public int difficulty = 15;
	private int startDifficulty;
	public GameObject[] enemyObjects;
	GameObject[] spawners;
	GameObject spawnedEnemy;
	float startTime;
	float currentTime;
	int currentSecond;
	bool spawnInSecond;

	// Use this for initialization
	void Start () {
		startDifficulty = difficulty;
		spawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
		currentTime = 0;
		currentSecond = 0;
		spawnInSecond = false;
	}

	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
		//Debug.Log (currentSecond);
		//Debug.Log (spawnInSecond);

		if (Input.GetButtonDown ("IncreaseDifficulty")) {
			if (difficulty >= 2) {
				difficulty--;
			}
		}
		if (Input.GetButtonDown ("DecreaseDifficulty")) {
			difficulty++;
		}
			

		if (Math.Floor ((double)currentTime) != currentSecond) {
			currentSecond = (int)Math.Floor ((double)currentTime);
			spawnInSecond = false;
			//Debug.Log (spawnInSecond);
		}
		//Debug.Log (currentSecond % difficulty);
		if (currentSecond == 1 && !spawnInSecond) {
			int i = UnityEngine.Random.Range (0, spawners.Length);
			spawnedEnemy = (GameObject)Instantiate (enemyObjects[0], transform.position, transform.rotation);
			if (difficulty>2 && (currentSecond % startDifficulty == 0))
				difficulty--;
			spawnInSecond = true;
		}

		if (currentSecond % difficulty == 1  && !(currentSecond % (difficulty * 4) == 1) && !spawnInSecond) {
			int i = UnityEngine.Random.Range (0, spawners.Length);
			spawnedEnemy = (GameObject)Instantiate (enemyObjects[0], transform.position, transform.rotation);
			if (difficulty>2 && (currentSecond % startDifficulty == 0))
				difficulty--;
			spawnInSecond = true;
		}
		if (currentSecond % (difficulty * 4) == 1 && enemyObjects.Length>1 && !spawnInSecond && currentSecond > 2) {
			int i = UnityEngine.Random.Range (0, spawners.Length);
			spawnedEnemy = (GameObject)Instantiate (enemyObjects[1], transform.position, transform.rotation);	
			if (difficulty>2 && (currentSecond % startDifficulty == 0))
				difficulty--;
			spawnInSecond = true;
		}
	}

	public void spawnEnemy(int enemyType){
		if (enemyType <= enemyObjects.Length-1 && enemyType >= 0)
			spawnedEnemy = (GameObject)Instantiate (enemyObjects[enemyType], transform.position, transform.rotation);
	}
}
