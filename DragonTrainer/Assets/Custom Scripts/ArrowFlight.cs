using UnityEngine;
using System.Collections;

public class ArrowFlight : MonoBehaviour {

	protected bool flying = true;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
        flying = true;
	}

	void OnCollisionEnter(Collision evt) {
        if (evt.collider.name != "FPSController")
        {
            flying = false;
			transform.position = transform.position + transform.forward;
			GetComponent<Rigidbody> ().isKinematic = false;
			GetComponent<Rigidbody> ().useGravity = false;
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			GetComponent<Rigidbody> ().freezeRotation = true;
			GetComponent<Collider> ().enabled = false;
			transform.SetParent (evt.collider.transform);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (flying) transform.LookAt(transform.position+rb.velocity*Time.deltaTime);
	}
}
