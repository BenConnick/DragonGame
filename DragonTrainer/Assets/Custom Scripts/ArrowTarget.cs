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
		print(gameObject.name);
		if(col.gameObject.name.Contains("Arrow"))
		{
			if (particleEffect) {
				GameObject.Instantiate(particleEffect,transform.position,transform.rotation);
			}
			print (gameObject.name + " HIT");
			if (destroyOnHit) {
				Destroy(gameObject);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
