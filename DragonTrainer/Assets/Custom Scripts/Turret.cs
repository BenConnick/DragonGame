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
			Vector3 minDist = new Vector3(100000000000,0,0);
			/*for (EnemyBehavior enemy in manager.enemies) {
				if (
			}*/
		}
	}
}
