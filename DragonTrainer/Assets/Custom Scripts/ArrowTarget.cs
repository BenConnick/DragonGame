using UnityEngine;
using System.Collections;

public class ArrowTarget : MonoBehaviour {

	public GameObject particleEffect;

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
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
