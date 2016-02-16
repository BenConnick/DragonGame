using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehavior : VehicleBehavior {

    //Attributes 
    public float wanderWt = 10.0f;
	protected float seekWt = 10.0f;
	protected float alignWt = 100.0f;
	protected int finalWaypointIndex = 9;
	protected int nextWaypointIndex = 8;
	EnemyManager manager;
	// Use this for initialization
	void Start () {
        //call the vehicle start method
        base.Start ();
		manager = FindObjectOfType<EnemyManager>();
	}
    protected override void CalcSteeringForce()
    {
        Vector3 force = Vector3.zero;
        //force += wanderWt * Wander();

		FindWaypoints();

		// create path by drawing line from prev to next
		int prevWaypointIndex = (nextWaypointIndex+1 > finalWaypointIndex) ? 0 : nextWaypointIndex + 1;

		// calculate the path follwing force
		force += seekWt * FollowPath(gameObject,manager.Waypoints[prevWaypointIndex].transform.position,manager.Waypoints[nextWaypointIndex].transform.position,5.0f);
		force += alignWt * Align(manager.Waypoints[nextWaypointIndex].transform.position - manager.Waypoints[prevWaypointIndex].transform.position);

        force = Vector3.ClampMagnitude(force, maxForce);
        ApplyForce(force);
    }

	protected void FindWaypoints() {
		// if you have hit your waypoint, go the the next
		if (transform.position.z < manager.Waypoints[nextWaypointIndex].transform.position.z) {
			nextWaypointIndex--;
			if (nextWaypointIndex < 0) {
				nextWaypointIndex = finalWaypointIndex;
			}
		}
	}
}
