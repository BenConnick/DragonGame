using UnityEngine;
using System.Collections;

public class ArrowTarget : MonoBehaviour {

	public GameObject particleEffect;
	public bool destroyOnHit = false;

	// Use this for initialization
	void Start () {

	}

	void OnCollisionEnter (Collision col)
	{
		//print(col.gameObject.name);
		if(col.gameObject.name.Contains("Arrow") || col.gameObject.name.Contains("Dragon"))
		{
			if (particleEffect) {
				ShowBlood ();
			}
			//print (gameObject.name + " HIT");
			if (destroyOnHit) {
				EnemyBehavior e = gameObject.GetComponent<EnemyBehavior>();
				e.setHealth (0);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void ShowBlood() {
		GameObject.Instantiate(particleEffect,transform.position,transform.rotation);
	}
}
