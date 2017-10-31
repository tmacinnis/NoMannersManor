using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController2D))]
public class Player : MonoBehaviour
{
	public int playerNumber;
	public float jumpHeight = 4f;
	public float timeToJumpApex = 0.4f;

	public enum PowerUp {
		Pistol, Crossbow, Shield, Blink, None
	}

	public PowerUp currentPowerUp{get; set;}
	public int powerUpUsesLeft{get; set;}

	public int maxJumps = 2;
	int jumpsLeft;
	//how much gravity is applied to the character
	float gravity;

	//movement Speed of the Character
	public float moveSpeed = 7f;
	float jumpVelocity;
	//velocity of the character;
	Vector3 velocity;

	float velocityXSmoothing;

	float accelerationTimeAirborn = 0;
	float accelerationTimeGrounded = 0;

	float fireSpeed = 2;
	float fireRate;

	public GameObject bulletProjectile;
	public GameObject arrowProjectile;
	private GameObject firedProjectile;

	public int health = 3;

	public int meleeTimer;

	bool isFacingRight = true;
	bool oldDirection;
	bool isJumping = false;
	bool isDashing = false;
	bool isRespawning = true;


	Collider2D meleeHitbox;
	public CircleCollider2D shieldHitBox;

	public bool shieldOn {get; set;}

	private Animator upperAnimator;
	private Animator lowerAnimator;
	//private Animator particleAnimator;

	public float blinkDistance = 800f;
	public Vector2 knockbackBuffer;

	public AudioClip blinkClip;
	public AudioClip gunClip1;
	public AudioClip gunClip2;
	public AudioClip hurtClip1;
	public AudioClip hurtClip2;
	public AudioClip hurtClip3;
	public AudioClip hurtClip4;
	public AudioClip jumpClip;
	public AudioClip crossbowClip;
	public AudioClip meleeClip1;
	public AudioClip meleeClip2;

	public SpriteRenderer upperSprite;
	public SpriteRenderer lowerSprite;

	public SpriteRenderer shieldSprite;


	public GameObject deathParticle;
	public GameObject blinkParticle;
	public GameObject dashParticle;

	float meleeCooldown = 0;
	float crossbowCooldown = 0;
	float blinkCooldown = 0;
	public float invulnTime = 0;
	float dashCooldown = 0;
	float dashing = 1f;
	public int score;

    PlayerController2D controller;
	// Use this for initialization
	void Start ()
    {
		//meleeHitbox = gameObject.GetComponentsInChildren<BoxCollider2D>()[1];
		upperAnimator = gameObject.GetComponentsInChildren<Animator>()[0];
		lowerAnimator = gameObject.GetComponentsInChildren<Animator>()[1];
		//particleAnimator = gameObject.GetComponentsInChildren<Animator> () [2];

		upperSprite = gameObject.GetComponentsInChildren<SpriteRenderer>()[0];
		lowerSprite = gameObject.GetComponentsInChildren<SpriteRenderer>()[1];
		shieldSprite = gameObject.GetComponentsInChildren<SpriteRenderer>()[2];


//		meleeHitbox.enabled = false;
		jumpsLeft = maxJumps;
        controller = GetComponent<PlayerController2D>();

		gravity = -(2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2));
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		powerUpUsesLeft = 0;
		currentPowerUp = PowerUp.None;
		knockbackBuffer = new Vector2(0,0);

		shieldHitBox = GetComponent<CircleCollider2D>();

		invulnTime = 0.5f;
		isRespawning = true;

		UIManager.instance.SetHealth(playerNumber, health);
		UIManager.instance.SetAmmo(playerNumber, powerUpUsesLeft);
		UIManager.instance.SetPowerup(playerNumber, currentPowerUp);
	}
	// Update is called once per frame
	void Update ()
	{
		meleeCooldown -= Time.deltaTime;
		crossbowCooldown -= Time.deltaTime;
		blinkCooldown -= Time.deltaTime;
		invulnTime -= Time.deltaTime;
		dashCooldown -= Time.deltaTime;

		if (invulnTime > 0) {
			Physics2D.IgnoreLayerCollision (this.gameObject.layer, 13);
			Physics2D.IgnoreLayerCollision (this.gameObject.layer, 18);
		}
		else {
			GetComponent<BoxCollider2D> ().enabled = true; 
			Physics2D.IgnoreLayerCollision (this.gameObject.layer, 13,false);
			Physics2D.IgnoreLayerCollision (this.gameObject.layer, 18,false);
			isRespawning = false;
		}

		if (Input.GetButtonDown ("Respawn" + playerNumber)) {
			GameObject[] spawners = GameObject.FindGameObjectsWithTag ("PlayerSpawner");
			int i = Random.Range (0, spawners.Length);
			spawners [i].GetComponent<PlayerSpawner> ().SpawnPlayer (playerNumber);
			Destroy (gameObject);
		}

		if (Input.GetButtonDown ("Melee" + playerNumber) && meleeCooldown <= 0 && dashing == 1.0f) {
			upperAnimator.SetTrigger ("MeleeAttack");
			meleeCooldown = 0.7f;
			SoundManager.instance.RandomizeSfx (meleeClip1, meleeClip2);
//			meleeHitbox.enabled = true;
		}
			
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		if (controller.collisions.below) {
			lowerAnimator.SetBool ("isJumping", false);
			isJumping = false;
		}
		//left and right movement input
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal" + playerNumber), Input.GetAxisRaw ("Vertical" + playerNumber));
		//jump input
		if (Input.GetButtonDown ("Jump" + playerNumber) && (jumpsLeft != 0)) {
			velocity.y = jumpVelocity;
			jumpsLeft--;
			lowerAnimator.SetBool ("isJumping", true);
			isJumping = true;
			SoundManager.instance.RandomizeSfx (jumpClip);
		}


		//Handle dash
		if (Input.GetButtonDown ("Dash" + playerNumber) && isDashing == false /* && controller.collisions.below == true*/ && invulnTime <= 0 && meleeCooldown <=0.4f){
			dashing = 2.0f;
			dashCooldown = 1.0f;
			invulnTime = 0.3f;
			isDashing = true;
			ParticleManager.instance.createParticleEffect(dashParticle, transform);
		}
		if (dashCooldown < 0.7) {
			//resetColor ();
			dashing = 1.0f;	 
		}
		if (dashCooldown <= 0)
			isDashing = false;


		if (invulnTime > 0f) {
			darken ();
		} else {
			resetColor();
		}

		// apply gravity & movement
		//Debug.Log(input.x);
		float targetVelocityX = (input.x * moveSpeed)*dashing;
		velocity.x = targetVelocityX;

		Debug.Log("Velocity.x: " + velocity.x);
		//velocity.x = Mathf.SmoothDamp( velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborn);
		velocity.y += gravity * Time.deltaTime;



		if (Input.GetButtonDown("Fire" + playerNumber)) {
			switch (currentPowerUp) {
			case PowerUp.Pistol : FirePistol();
							  	  break;
			case PowerUp.Crossbow : FireArrow();
									break;
			case PowerUp.Blink: Blink();
								
								break;
			
			default: break;
			}

		}
			
		if(shieldOn == true && currentPowerUp == PowerUp.Shield && powerUpUsesLeft == 0) {
			ToggleShield();
		}

		if(shieldOn == true && currentPowerUp != PowerUp.Shield) {
			ToggleShield();
		}

		if(currentPowerUp == PowerUp.Shield && shieldOn == false && powerUpUsesLeft == 2) {
			ToggleShield();
		}

		if (velocity.x != 0) {
			if (velocity.x > 0) {
				if(isFacingRight != true) {
					oldDirection = isFacingRight;
					isFacingRight = true;
					flip();
				}


			}
			else if(velocity.x < 0) {
				if(isFacingRight != false) {
					oldDirection = isFacingRight;
					isFacingRight = false;
					flip();
				}


			}
				
		}

		if ((knockbackBuffer.x > 1 || knockbackBuffer.x < -1) && knockbackBuffer.y > 1 ) {
			velocity.x += (knockbackBuffer.x / 10)*Time.deltaTime*200;
			velocity.y += (knockbackBuffer.y / 10)*Time.deltaTime*200;

			knockbackBuffer.x -= knockbackBuffer.x / 10;
			knockbackBuffer.y -= knockbackBuffer.y / 10;
		}

		if(transform.localScale.x > 0) {
			 if (controller.collisions.right == true) {
					velocity.x = 0;
			}
		} else if (transform.localScale.x < 0) {
			if (controller.collisions.left == false) {
				velocity.x *= -1;
			} else {
				velocity.x = 0;
			}
		}
		/*if (transform.localScale.x > 0) {
			if (velocity.x > 0 && controller.collisions.right)
				velocity.x = 0;
			if (velocity.x < 0 && controller.collisions.left)
				velocity.x = 0;
		} else {
			if (velocity.x > 0 && controller.collisions.left)
				velocity.x = 0;
			if (velocity.x < 0 && controller.collisions.right)
				velocity.x = 0;
		}*/

		controller.Move(velocity * Time.deltaTime, transform.localScale.x);
	
		if(velocity.x != 0 && isJumping == false) {
			lowerAnimator.SetBool("isWalking", true);
			//Debug.Log("yo");
		}
		else {
			lowerAnimator.SetBool("isWalking", false);
			//Debug.Log("yo2");
		}
		//apply velocity to the character
		//controller.Move(velocity * Time.deltaTime);




		if(controller.collisions.below) {
			jumpsLeft = maxJumps;
		}






	}


	void OnHit (GameObject source) {
		if (invulnTime <= 0) {
			health--;
			UIManager.instance.SetHealth (playerNumber, health);
			SoundManager.instance.RandomizeSfx (hurtClip1, hurtClip2, hurtClip3, hurtClip4);
			if (health == 0 || source.tag == "Bullet") {
				int lives = UIManager.instance.GetLives (playerNumber);
				UIManager.instance.SetLives (playerNumber, --lives);
				ParticleManager.instance.createParticleEffect (deathParticle, transform);
				if (lives > 0) {
					GameObject[] spawners = GameObject.FindGameObjectsWithTag ("PlayerSpawner");
					int i = Random.Range (0, spawners.Length);
					spawners [i].GetComponent<PlayerSpawner> ().SpawnPlayer (playerNumber);


				}

				Destroy (gameObject);
			

			} else {
				invulnTime = 0.5f; 
				//GetComponent<BoxCollider2D>().enabled = false;
				if (source.gameObject.tag == "Player") {
					
					if (source.GetComponentInParent<Player> ().isFacingRight) {
						Debug.Log ("Still doing knockback stuff");
						knockbackBuffer.x = 400;
					} else {
						Debug.Log ("Still doing knockback stuff2");
						knockbackBuffer.x = -400;
					}
					knockbackBuffer.y = 15;
				} else { 
					if (source.gameObject.name == "EnemyFish(clone)") {
						if ((int)source.GetComponentInParent<EnemyFish> ().superficialDirection == 1) {
							knockbackBuffer.x = 400;
						} else {
							knockbackBuffer.x = -400;
						}
						knockbackBuffer.y = 15;
					} else if (source.gameObject.name == "EnemyOcto(clone)") {
						if ((int)source.GetComponentInParent<EnemyOcto> ().superficialDirection == 1) {
							knockbackBuffer.x = 400;
						} else {
							knockbackBuffer.x = -400;
						}
						knockbackBuffer.y = 15;
					}else {
						if (isFacingRight) {
							knockbackBuffer.x = -300;
						} else {
							knockbackBuffer.x = 300;
						}
					knockbackBuffer.y = 15;
					}
				}


			}
		}

	}

	void OnBecameInvisible() {
		//GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");

		//int i = Random.Range (0, spawners.Length);

		//spawners[i].GetComponent<PlayerSpawner>().SpawnPlayer(playerNumber);

		//Destroy(gameObject);																														
	} 

	void FirePistol() {
		
		if (powerUpUsesLeft > 0) {
			firedProjectile = Instantiate(bulletProjectile, transform.position, transform.rotation) as GameObject;
			firedProjectile.GetComponentInParent<Bullet> ().playerNumber = playerNumber;
			firedProjectile.GetComponent<Rigidbody2D>().AddForce(new Vector2((isFacingRight?1:-1) * 2000, 0));
			if(!isFacingRight) {
				Vector3 scale = firedProjectile.transform.localScale;
				scale.x *= -1;
				firedProjectile.transform.localScale = scale;
			}
			Physics2D.IgnoreCollision(firedProjectile.GetComponent<Collider2D>(), GetComponent<BoxCollider2D>());	
			powerUpUsesLeft -= 1;
			if(powerUpUsesLeft == 0) {
				currentPowerUp = PowerUp.None;
				UIManager.instance.SetPowerup(playerNumber, currentPowerUp);
			}
			UIManager.instance.SetAmmo(playerNumber, powerUpUsesLeft);
			SoundManager.instance.RandomizeSfx(gunClip1, gunClip2);
			upperAnimator.SetTrigger("ShootPistol");
		}

	}

	void FireArrow() {
		if (powerUpUsesLeft > 0 && crossbowCooldown <= 0) {
			firedProjectile = Instantiate(arrowProjectile, transform.position, transform.rotation) as GameObject;
			firedProjectile.GetComponentInParent<Bullet> ().playerNumber = playerNumber;
			firedProjectile.GetComponent<Rigidbody2D>().AddForce(new Vector2((isFacingRight?1:-1) * 1000, 250));
			if(!isFacingRight) {
				Vector3 scale = firedProjectile.transform.localScale;
				scale.x *= -1;
				firedProjectile.transform.localScale = scale;
			}
			Physics2D.IgnoreCollision(firedProjectile.GetComponent<Collider2D>(), GetComponent<BoxCollider2D>());	
			powerUpUsesLeft -= 1;
			if(powerUpUsesLeft == 0) {
				currentPowerUp = PowerUp.None;
				UIManager.instance.SetPowerup(playerNumber, currentPowerUp);
			}
			UIManager.instance.SetAmmo(playerNumber, powerUpUsesLeft);
			SoundManager.instance.RandomizeSfx(crossbowClip);
			crossbowCooldown = 0.7f;
			upperAnimator.SetTrigger("ShootCrossbow");
		}
	}
	void Blink() {
		if(powerUpUsesLeft > 0 && blinkCooldown <= 0) {
			if (isFacingRight) {
				velocity.x += (blinkDistance * 1)*Time.deltaTime*175;
				//jumpsLeft += 1;
			}
			else {
				velocity.x += (blinkDistance * -1)*Time.deltaTime*175;
				//jumpsLeft += 1;
			}
			powerUpUsesLeft -= 1;
			if(powerUpUsesLeft == 0){
				currentPowerUp = PowerUp.None;
				UIManager.instance.SetPowerup(playerNumber, currentPowerUp);

			}
			UIManager.instance.SetAmmo(playerNumber, powerUpUsesLeft);
			SoundManager.instance.PlaySingle(blinkClip);
			blinkCooldown = 0.5f;
			ParticleManager.instance.createParticleEffect(blinkParticle, transform);
		}
	}

	void resetColor(){
		Color tmp = upperSprite.color;
		tmp = Color.white;
		upperSprite.color = tmp;
		lowerSprite.color = tmp;
	}

	void darken(){
		Color tmp = upperSprite.color;
		if (isDashing) {
			tmp.a = 0.5f;
			tmp.b = 0.8f;
			tmp.r = 0.8f;
			tmp.g = 0.8f;
		} else if (isRespawning) {
			tmp.a = 0.5f;
			tmp.b = 0.8f;
			tmp.r = 0.5f;
			tmp.g = 0.5f;
		}else{
			tmp.b = 0.5f;
			tmp.r = 0.9f;
			tmp.g = 0.5f;
		}
		upperSprite.color = tmp;
		lowerSprite.color = tmp;
	}

	void ToggleShield() {
		shieldHitBox.enabled = !shieldHitBox.enabled;
		shieldSprite.enabled = !shieldSprite.enabled;
		shieldOn = !shieldOn;
	}

	void flip() {

		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

}
