using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	//private Vector2 intialForce;
	public int playerNumber;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log ("Bullet Collision");
		if(collision.gameObject.tag.Equals("Level"))
			Destroy(gameObject);

		if (collision.gameObject.tag == "Player"){
			if (collision.gameObject.GetComponentInParent<Player> ().invulnTime > 0) {
				Physics2D.IgnoreCollision (collision.gameObject.GetComponent<BoxCollider2D> (), GetComponent<BoxCollider2D>());
			} else {
				Debug.Log ("Hit_Bullet");
				collision.gameObject.SendMessage ("OnHit", this.gameObject);
				Destroy (gameObject);
			}
		
		}
		if ( collision.gameObject.tag == "Enemy") {
			//if (!(this.gameObject.tag == "Laser")){
				Debug.Log ("Hit_Bullet");
				collision.gameObject.SendMessage ("OnHit", this.gameObject);
				Destroy (gameObject);
			//}
		}
	}
	void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "Player") {
			if((other.GetComponent<Player>().shieldOn == true)) {
				other.GetComponent<Player>().powerUpUsesLeft -= 1;
				UIManager.instance.SetAmmo(other.GetComponent<Player>().playerNumber, other.GetComponent<Player>().powerUpUsesLeft);
				Destroy(gameObject);
			}
		}

			
	}

	void OnBecameInvisible() {
		Destroy(gameObject);
	}
}
