using UnityEngine;
using System.Collections;

public class MeleeHitbox : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("hello world");
		if (other.gameObject.tag == "Player") {
			other.gameObject.SendMessage("OnHit");
		}
	}
	void OnTriggerStay2D(Collider2D other) {
		Debug.Log("hello world");
		if (other.gameObject.tag == "Player") {
			other.gameObject.SendMessage("OnHit");
		}
	}
}
			