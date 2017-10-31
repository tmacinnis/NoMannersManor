using UnityEngine;
using System.Collections;

public class enemyTeleporter : MonoBehaviour {

	public SpriteRenderer teleporterSprite;
	private Animator teleporterAnimator;

	public GameObject teleportParticle;

	// Use this for initialization
	void Start () {
		teleporterAnimator = gameObject.GetComponentsInChildren<Animator>()[0];
		teleporterSprite = gameObject.GetComponentsInChildren<SpriteRenderer>()[0];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.tag == "Enemy") {
			GameObject[] spawners = GameObject.FindGameObjectsWithTag ("EnemySpawner");
			int i = Random.Range (0, spawners.Length);
			if (other.gameObject.name == "EnemyFish(Clone)") {
				ParticleManager.instance.createParticleEffect(teleportParticle, transform);
				other.gameObject.GetComponentInParent<EnemyFish>().kill = true;
				spawners [i].GetComponent<EnemySpawning> ().spawnEnemy (0);
			}
			if (other.gameObject.name == "EnemyOcto(Clone)"){
				ParticleManager.instance.createParticleEffect(teleportParticle, transform);
				other.gameObject.GetComponentInParent<EnemyOcto>().kill = true;
				spawners [i].GetComponent<EnemySpawning> ().spawnEnemy (1);
			}
		}
	}

	void OnCollisionStay2D(Collision2D other){
		if (other.gameObject.tag == "Enemy") {
			GameObject[] spawners = GameObject.FindGameObjectsWithTag ("EnemySpawner");
			int i = Random.Range (0, spawners.Length);
			if (other.gameObject.name == "EnemyFish(Clone)") {
				ParticleManager.instance.createParticleEffect(teleportParticle, transform);
				other.gameObject.GetComponentInParent<EnemyFish>().kill = true;
				spawners [i].GetComponent<EnemySpawning> ().spawnEnemy (0);
			}
			if (other.gameObject.name == "EnemyOcto(Clone)"){
				ParticleManager.instance.createParticleEffect(teleportParticle, transform);
				other.gameObject.GetComponentInParent<EnemyFish>().kill = true;
				spawners [i].GetComponent<EnemySpawning> ().spawnEnemy (1);
			}
		}
	}
}
