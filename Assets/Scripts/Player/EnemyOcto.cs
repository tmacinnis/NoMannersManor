using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (PlayerController2D))]
public class EnemyOcto : MonoBehaviour {

	public float jumpHeight = 4f;
	public float timeToJumpApex = 0.4f;
	public float speed = 7f;
	float gravity;

	Vector3 velocity;

	BoxCollider2D octoHitbox;
	Rigidbody2D octobody;

	public SpriteRenderer octoSprite;
	private Animator octoAnimator;

	private GameObject firedProjectile;
	public GameObject deathParticle;
	public GameObject laserProjectile;

	bool isFacingRight = true;
	bool oldDirection;

	public AudioClip hurtClip1;
	public AudioClip hurtClip2;
	public AudioClip laserClip1;
	public AudioClip laserClip2;


	public float direction;
	public float superficialDirection;
	float notFalling;
	float random;

	float currentTime;
	int currentSecond;
	public bool kill;

	bool fireLaserInSecond;
	bool flipInSecond;
	float move;
	float playerInvulnTime;

	BoxCollider2D collidedPlayer;

	PlayerController2D controller;

	// Use this for initialization
	void Start () {
		kill = false;
		octoAnimator = gameObject.GetComponentsInChildren<Animator>()[0];
		octoSprite = gameObject.GetComponentsInChildren<SpriteRenderer>()[0];
		//random = Random.Range (0f, 3f);

		playerInvulnTime = 0;
		move = 1;
		currentTime = 0;
		currentSecond = 0;
		flipInSecond = false;
		fireLaserInSecond = false;

		gravity = -(2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2));

		controller = GetComponent<PlayerController2D>();

		velocity.x = speed;

	}

	// Update is called once per frame
	void Update () {
		if (kill)
			Destroy (gameObject);

		currentTime += Time.deltaTime;
		playerInvulnTime -= Time.deltaTime;

		random = UnityEngine.Random.Range (0f, 2f);
		move = 1;

		if (playerInvulnTime <= 0)
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), collidedPlayer, false);


		if (controller.collisions.below)
			velocity.y = 0;

		if (Math.Floor ((double)currentTime) != currentSecond) {
			currentSecond = (int)Math.Floor ((double)currentTime);
			flipInSecond = false;
			fireLaserInSecond = false;
		}



		if (controller.collisions.left && !flipInSecond) {
			flipInSecond = true;
			direction = -1f;
			flip ();
		} else if (controller.collisions.right && !flipInSecond) {
			flipInSecond = true;
			direction = 1f;
			flip ();
		} else if (controller.collisions.left || controller.collisions.right) {
			move = 0;
		} else if ((float)currentSecond % 4 - random <= 1){
			if ((float)currentSecond % 4 - random > 0){
				Debug.Log (random);
				if (!flipInSecond) {
					if (direction == 1f)
						direction = -1f;
					if (direction == -1f)
						direction = 1f;
					Debug.Log ("flip: " + direction);
					flipInSecond = true;
					flip ();
				}
			}
		}

		random = UnityEngine.Random.Range (0f, 2f);
		if ((float)currentSecond % 4 - random <= .5 && (float)currentSecond % 4 - random > 0 && !fireLaserInSecond) {
			Debug.Log ("Fire ze lasers!");
			fireLaserInSecond = true;
			FireLaser ();

		}


		if (controller.collisions.below && direction == 0)
			direction = 1;

		velocity.x = speed*direction*move;
		velocity.y += gravity * Time.deltaTime;

		if ((int)transform.localScale.x > 0) {
			superficialDirection = 1;
			isFacingRight = false;
		} else {
			superficialDirection = -1;
			isFacingRight = true;
		}
		controller.Move(velocity * Time.deltaTime, transform.localScale.x);
		//Debug.Log (transform.localScale.x);
		//Debug.Log(superficialDirection);
	}

	void OnHit (GameObject source){
		Debug.Log ("Hit Octo");
		if(source.tag == "Player")
			UIManager.instance.incrementScore (source.gameObject.GetComponentInParent<Player> ().playerNumber);
		if(source.gameObject.tag == "Bullet")
			UIManager.instance.incrementScore (source.gameObject.GetComponentInParent<Bullet> ().playerNumber);
		ParticleManager.instance.createParticleEffect(deathParticle, transform);
		DestroyObject (gameObject);
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.gameObject.tag == "Player") {
			if (other.gameObject.GetComponentInParent<Player> ().invulnTime <= 0) {
				if (!other.gameObject.GetComponentInParent<Player> ().shieldOn) {
					Debug.Log ("Octo Hit Player");
					other.gameObject.SendMessage ("OnHit", this.gameObject);
				} else {
					other.gameObject.GetComponentInParent<Player> ().powerUpUsesLeft--;
					UIManager.instance.SetAmmo (other.gameObject.GetComponentInParent<Player> ().playerNumber, other.gameObject.GetComponentInParent<Player> ().powerUpUsesLeft);
					other.gameObject.GetComponentInParent<Player>().invulnTime = 0.5f;
					/*if (superficialDirection == 1) {
						other.gameObject.GetComponentInParent<Player> ().knockbackBuffer.x = 400;
					} else {
						other.gameObject.GetComponentInParent<Player> ().knockbackBuffer.x = -400;
					}
					other.gameObject.GetComponentInParent<Player>().knockbackBuffer.y = 15;*/
				}
			} else {
				collidedPlayer = other.gameObject.GetComponent<BoxCollider2D>();
				playerInvulnTime = other.gameObject.GetComponent<Player> ().invulnTime;
				Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), other.gameObject.GetComponent<BoxCollider2D> ());

			}
		}
		if(other.gameObject.tag == "Laser")
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), other.gameObject.GetComponent<BoxCollider2D> ());
		if (other.gameObject.tag == "Bullet") {
			UIManager.instance.incrementScore (other.gameObject.GetComponentInParent<Bullet> ().playerNumber);
			Destroy (gameObject);
		}
	}
	void OnCollisionStay2D(Collision2D other) {

		if (other.gameObject.tag == "Player") {
			Debug.Log ("Octo collision Stay");
			if (other.gameObject.GetComponentInParent<Player>().invulnTime <= 0) {
				if (!other.gameObject.GetComponentInParent<Player>().shieldOn){
					Debug.Log ("Octo Hit Player");
					other.gameObject.SendMessage ("OnHit", this.gameObject);
				}else{
					other.gameObject.GetComponentInParent<Player>().powerUpUsesLeft--;
					UIManager.instance.SetAmmo(other.gameObject.GetComponentInParent<Player>().playerNumber, other.gameObject.GetComponentInParent<Player>().powerUpUsesLeft);
					other.gameObject.GetComponentInParent<Player>().invulnTime = 0.5f;
					//if (superficialDirection == 1) {
					//	other.gameObject.GetComponentInParent<Player>().knockbackBuffer.x = 400;
					//}
					//else {
					//	other.gameObject.GetComponentInParent<Player>().knockbackBuffer.x = -400;
					//}
					//other.gameObject.GetComponentInParent<Player>().knockbackBuffer.y = 15;
				}
			} else {
				collidedPlayer = other.gameObject.GetComponent<BoxCollider2D>();
				playerInvulnTime = other.gameObject.GetComponent<Player> ().invulnTime;
				Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), other.gameObject.GetComponent<BoxCollider2D> ());
			}
		}
		if(other.gameObject.tag == "Laser")
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), other.gameObject.GetComponent<BoxCollider2D> ());
		if (other.gameObject.tag == "Bullet") {
			UIManager.instance.incrementScore (other.gameObject.GetComponentInParent<Bullet> ().playerNumber);
			Destroy (gameObject);
		}
	}

	void FireLaser() {
		firedProjectile = Instantiate(laserProjectile, transform.position, transform.rotation) as GameObject;
		firedProjectile.GetComponent<Rigidbody2D>().AddForce(new Vector2((isFacingRight?1:-1) * 1000, 0));
		if(!isFacingRight) {
			Vector3 scale = firedProjectile.transform.localScale;
			scale.x *= -1;
			firedProjectile.transform.localScale = scale;
		}
		Physics2D.IgnoreCollision(firedProjectile.GetComponent<Collider2D>(), GetComponent<BoxCollider2D>());	
		SoundManager.instance.RandomizeSfx(laserClip1, laserClip2);
	}


	void flip() {
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	void DestroyMe(){
		Destroy (gameObject);
	}

}
