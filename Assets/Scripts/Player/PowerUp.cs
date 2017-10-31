using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	bool pickUp = false;
	PowerUpSpawner parentSpawner;
	public Player.PowerUp powerUpType;
	public AudioClip pickUpClip;
	public GameObject powerUpPickupParticle;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if(pickUp) {
			
			parentSpawner.SendMessage("OnPickedUpPowerUp");
			Destroy(this.gameObject);
		}

	}

	void OnTriggerEnter2D(Collider2D other) {
		
		if(other.tag == "PowerUpSpawner") {
			parentSpawner = other.GetComponentInParent<PowerUpSpawner>();
		}
		Player player = other.GetComponentInParent<Player>();
		if (other.tag == "Player") {
			if (powerUpType == Player.PowerUp.Pistol) {
				player.currentPowerUp = Player.PowerUp.Pistol;
				player.powerUpUsesLeft = 1;
				UIManager.instance.SetAmmo(player.playerNumber, 1);
				UIManager.instance.SetPowerup(player.playerNumber, Player.PowerUp.Pistol);
			}
			else if (powerUpType == Player.PowerUp.Crossbow) {
				player.currentPowerUp = Player.PowerUp.Crossbow;
				player.powerUpUsesLeft = 3;
				UIManager.instance.SetAmmo(player.playerNumber, 3);
				UIManager.instance.SetPowerup(player.playerNumber, Player.PowerUp.Crossbow);
			}
			else if(powerUpType == Player.PowerUp.Shield) {
				player.currentPowerUp = Player.PowerUp.Shield;
				player.powerUpUsesLeft = 2;
				UIManager.instance.SetAmmo(player.playerNumber, 2);
				UIManager.instance.SetPowerup(player.playerNumber, Player.PowerUp.Shield);
			}
			else if(powerUpType == Player.PowerUp.Blink) {
				player.currentPowerUp = Player.PowerUp.Blink;
				player.powerUpUsesLeft = 3;
				UIManager.instance.SetAmmo(player.playerNumber, 3);
				UIManager.instance.SetPowerup(player.playerNumber, Player.PowerUp.Blink);
			}

			pickUp = true;
			SoundManager.instance.PlaySingle(pickUpClip);
			ParticleManager.instance.createParticleEffect(powerUpPickupParticle, transform);
		}



	}

}
