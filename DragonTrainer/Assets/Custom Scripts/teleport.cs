using UnityEngine;
using System.Collections;

public class teleport : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// teleport on enter
	void OnTriggerEnter(Collider other) {
		print ("teleport");
		other.transform.position = new Vector3 (70, 40, 10);
	}
}
