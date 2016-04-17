using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//ben version
public class EnemyBehavior : VehicleBehavior {

    //Attributes 
    public float wanderWt = 10.0f;
	public float seekWt = 10.0f;
    public float alignWt = 100.0f;
    public float avoidWt = 100.0f;
    public float distFromEnemies = 1.0f;
    public float distFromObstacles = 2.0f;
	protected int finalWaypointIndex = 9;
	protected int nextWaypointIndex = 8;
	protected Transform target;
	protected NavMeshAgent nAgent;
	EnemyManager manager;

    //keep track of surroundings
    private GameObject[] enemies;
    private GameObject[] obstacles;
    private ArrayList allEnemies = new ArrayList();

	// for use with navmesh
	bool nav = false;

	// Use this for initialization
	void Start () {
        //call the vehicle start method
        base.Start ();
		manager = FindObjectOfType<EnemyManager>();

        //populate the arrays
        obstacles = GameObject.FindGameObjectsWithTag("obstacle");
		nAgent = GetComponent<NavMeshAgent> ();
		if (nAgent != null) nav = true;

		target = GameObject.FindGameObjectWithTag ("tower").transform;

	}
    protected override void CalcSteeringForce()
    {
		if (nav) {
			//if (nAgent.destination == null)
				nAgent.SetDestination (target.position);
		}
		else {
			Vector3 force = Vector3.zero;
			force += wanderWt * Wander ();
			force += wanderWt * Wander ();
			force += seekWt * Arrival (target.position);

            //avoid each other
            foreach (EnemyBehavior e in manager.enemies)
            {
                if(e.transform.position != transform.position)
                {
                    force += avoidWt * Avoid(e.gameObject, distFromEnemies);
                }
            }
				

			//avoid obstacles
			for (int i = 0; i < obstacles.Length; i++) {
				if (obstacles [i].transform.position != transform.position) {
					force += avoidWt * Avoid (obstacles [i], distFromObstacles);
				}
			}

			//FindWaypoints();

			// create path by drawing line from prev to next
			//int prevWaypointIndex = (nextWaypointIndex+1 > finalWaypointIndex) ? 0 : nextWaypointIndex + 1;

			// calculate the path follwing force
			//force += seekWt * FollowPath(gameObject,manager.Waypoints[prevWaypointIndex].transform.position,manager.Waypoints[nextWaypointIndex].transform.position,5.0f);
			//force += alignWt * Align(manager.Waypoints[nextWaypointIndex].transform.position - manager.Waypoints[prevWaypointIndex].transform.position);
	        
			force = Vector3.ClampMagnitude (force, maxForce);
			ApplyForce (force);
		}
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
