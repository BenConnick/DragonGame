using UnityEngine;
using System.Collections;

public class ArrowFlight : MonoBehaviour {

	protected bool flying = true;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}

	void OnCollisionEnter() {
		flying = false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(transform.position+rb.velocity*Time.deltaTime);
	}
}
