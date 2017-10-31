using UnityEngine;
using System.Collections;

public class HitBoxManager : MonoBehaviour {

	public PolygonCollider2D frame2;
	public PolygonCollider2D frame3;
	public PolygonCollider2D frame4;

	private PolygonCollider2D[] colliders;
	private PolygonCollider2D localcollider;

	public enum hitBoxes {
		frame2Box,
		frame3Box,
		frame4Box,
		clear
	}

	// Use this for initialization
	void Start () {
		colliders = new PolygonCollider2D[]{frame2, frame3, frame4};

		localcollider = gameObject.AddComponent<PolygonCollider2D>();
		localcollider.isTrigger = true;
		localcollider.pathCount = 0;
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void setHitBox(hitBoxes val) {
		if (val != hitBoxes.clear) {
			localcollider.SetPath(0, colliders[(int)val].GetPath(0));
			return;
		}
		localcollider.pathCount = 0;
	}

	public void OnTriggerEnter2D(Collider2D other){
		if(other.isTrigger != true) {
			if(other.gameObject.layer == 8 || other.gameObject.tag == "Enemy") {
				Debug.Log("Hit");
				other.gameObject.SendMessage("OnHit", this.gameObject);
			}
		}
	}
}
