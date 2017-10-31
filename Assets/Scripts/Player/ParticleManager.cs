using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {

	public static ParticleManager instance = null;
	// Use this for initialization
	void Awake () {
		if(instance == null) {
			instance = this;
		}
		else if(instance != this) {
			Destroy(gameObject);
		}
	}
	
	public void createParticleEffect(GameObject obj, Transform t) {
		GameObject instantiatedObject = Instantiate(obj, t.position, t.rotation) as GameObject;
	}
}
