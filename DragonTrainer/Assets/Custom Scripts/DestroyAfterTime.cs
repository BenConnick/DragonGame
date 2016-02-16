using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {

	public float lifetime = 5.0f; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (lifetime < 0) {
			Destroy(gameObject);
		} else {
			lifetime-=Time.deltaTime;
		}
	}
}
