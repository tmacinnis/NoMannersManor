using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class PlayerUIScript : MonoBehaviour {

	public Image[] Lives;
	public Image[] Health;
	public Image[] Ammo;
	public Image PistolImage;
	public Image CrossbowImage;
	public Image BlinkImage;
	public Image ShieldImage;

	private int lives;

	public void SetHealth(int num) {

		foreach (Image i in Health) {
			i.enabled = true;
		}
		for(int i = 0; i < num; i++) {
			Health[i].enabled = false;		
		}

	}

	public void SetLives(int num) {

		foreach (Image i in Lives) {
			i.enabled = true;
		}
		for(int i = 0; i < num; i++) {
			Lives[i].enabled = false;		
		}
		lives = num;
	}

	public void SetAmmo(int num) {
		foreach (Image i in Ammo) {
			i.enabled = false;
		}
		for(int i = 0; i < num; i++) {
			Ammo[i].enabled = true;		
		}
	}

	public int GetLives() {
		return lives;
	}

	public void SetPowerUp(Player.PowerUp p) {

		CrossbowImage.enabled = false;
		PistolImage.enabled = false;
		BlinkImage.enabled = false;
		ShieldImage.enabled = false;

		if(p == Player.PowerUp.Pistol) {
			PistolImage.enabled = true;
		}
		else if(p == Player.PowerUp.Crossbow) {
			CrossbowImage.enabled = true;
		}
		else if (p == Player.PowerUp.Blink) {
			BlinkImage.enabled = true;
		}
		else if (p == Player.PowerUp.Shield) {
			ShieldImage.enabled = true;
		}

	}
}
