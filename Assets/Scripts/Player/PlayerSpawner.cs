using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {


	public GameObject[] playerObjects;

	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame	
	void Update () {
		
	}

	public void SpawnPlayer(int playerNumber) {
		Debug.Log("SpawnPlayer");
		GameObject spawnedPlayer = (GameObject)Instantiate(playerObjects[playerNumber - 1], transform.position, transform.rotation);
	}
}
