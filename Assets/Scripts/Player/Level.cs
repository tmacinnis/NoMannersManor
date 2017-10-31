using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Level : MonoBehaviour {

	public static int numplayers;
	// Use this for initialization
	void Start () {

        Cursor.visible = false;
		Physics2D.IgnoreLayerCollision(13, 13);
		int players = numplayers;
		GameObject[] spawners = GameObject.FindGameObjectsWithTag("PlayerSpawner");

		List<int> spawnersUsed = new List<int>();
		int i;
		while (players > 0) {
			i = Random.Range(0, spawners.Length);

			if (!spawnersUsed.Contains(i)) {
				spawnersUsed.Add(i);
				spawners[i].GetComponent<PlayerSpawner>().SpawnPlayer(players);
				players--;
			}
			else {
				continue;
			}
		}

		if (numplayers == 3) {
			UIManager.instance.DisablePlayerUI (4);
		} else if (numplayers == 2) {
			UIManager.instance.DisablePlayerUI (3);
			UIManager.instance.DisablePlayerUI (4);
		} else if (numplayers == 1) {
			UIManager.instance.DisablePlayerUI (2);
			UIManager.instance.DisablePlayerUI (3);
			UIManager.instance.DisablePlayerUI (4);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
