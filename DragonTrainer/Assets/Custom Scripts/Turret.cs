using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

	EnemyManager manager;
	float killTimer;
	public float KillInterval; // set in inspector

	// Use this for initialization
	void Start () {
		manager = FindObjectOfType<EnemyManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		killTimer += Time.deltaTime;
		if (killTimer > KillInterval) {
			killTimer = killTimer - KillInterval; // reset kill timer

			// find the closest enemy
			Vector3 minDistVec = new Vector3(100000000000,0,0);
			// store the closest
			EnemyBehavior closest = null;

			foreach (EnemyBehavior e in manager.enemies) {
				if ((transform.position - e.transform.position).sqrMagnitude < (transform.position - minDistVec).sqrMagnitude) {
					closest = e;
					minDistVec = e.transform.position;
				}
			}

			//print("closest monster " + closest.id);

			RaycastHit hit;

			//Debug.DrawLine (transform.position, closest.transform.position,Color.red,0.5f);

			Ray myRay = new Ray (transform.position, new Vector3(0,1,0) + closest.transform.position - transform.position);

			// draw from pos to pos + dir*dist
			Debug.DrawLine (transform.position, transform.position + myRay.direction*(closest.transform.position - transform.position).magnitude,Color.blue,0.5f);

			Physics.Raycast (myRay, out hit, (closest.transform.position - transform.position).magnitude + 10);

			//print (hit.transform + " hit (A)");

			if (hit.transform && hit.transform.gameObject.GetComponent<EnemyBehavior>() != null) {
				// kill
				hit.transform.gameObject.GetComponent<EnemyBehavior>().setHealth(-1000000);
				print (hit.transform + " hit");
			} else {
				//print (hit.transform + " hit (B)");
			}
		}
	}
}
