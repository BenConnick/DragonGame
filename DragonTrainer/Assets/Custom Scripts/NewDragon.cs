using UnityEngine;
using System.Collections;

public class NewDragon : VehicleBehavior {

	// state Enum
	public enum States { IDLING, WANDERING, CHASING, PATROLLING, FOLLOWING_PLAYER, ATTACKING }

	//Attributes 
	public float wanderWt = 10.0f;
	public float seekWt = 10.0f;
	public float alignWt = 100.0f;
	public float avoidWt = 100.0f;
	public float distFromEnemies;
	public float distFromObstacles;
	public float maxDistFromPlayer;
	public GameObject player;
	protected Transform target;
	protected NavMeshAgent nAgent;
	protected int mode;
	EnemyManager manager;

	//keep track of surroundings
	protected GameObject[] enemies;
	protected GameObject[] obstacles;

	protected int currentOrder=-1;

	protected States state = States.CHASING;

	// the thing that the dragon is currently doing
	public States State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	// Use this for initialization
	void Start () {

		base.Start (); //call the vehicle start method

		manager = FindObjectOfType<EnemyManager>(); // get manager reference

		//populate the arrays
		enemies = GameObject.FindGameObjectsWithTag("enemy");
		obstacles = GameObject.FindGameObjectsWithTag("obstacle");

		//target = player.transform; // set target player

		TakeAction (States.CHASING);

	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.name.Contains ("Enemy")) {
			col.gameObject.GetComponent<EnemyBehavior> ().setHealth (0);
		}
	}

	void Update()
	{
		
		// order followed, wait for new order
		currentOrder = -1;

		// acquire target
		if (target == null) {
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

			target = closest.transform;
		}

		// move around
		base.Update();
		// parent (base) calls CalcSteeringForce
	}

	// set the dragon's target
	public void SetTarget(Transform t) {
		target = t;
	}

	// give the dragon a command from CommandControls
	public void Command(int command)
	{
		AddOrder(command);
	}

	// add a new order to the stack
	protected void AddOrder(int order)
	{
		TakeAction (order);
	}

	protected void TakeAction(int action) {
		state = (States)action;
	}

	protected void TakeAction(States action) {
		state = action;
	}

	// move the Dragon
	protected override void CalcSteeringForce()
	{
		// reset force vector
		Vector3 force = Vector3.zero;

		// check what state the dragon is in
		switch ((int)state)
		{
		// halt
		case 0:
			// arrive at own position
			//force += seekWt * Arrival(transform.position);
			break;
			// go to target
		case 1:
			force += wanderWt * Wander();
			break;
			// arrive / follow
		case 2:
			force += seekWt * Pursue(target.gameObject,1f);
			break;
			// if something goes wrong, wander
		default:
			force += wanderWt * Wander();
			break;
		}


		//avoid obstacles
		for (int i = 0; i < obstacles.Length; i++) {
			if (obstacles [i].transform.position != transform.position) {
				force += avoidWt * Avoid (obstacles [i], distFromObstacles);
			}
		}

		// limit force and apply
		force = Vector3.ClampMagnitude (force, maxForce);
		ApplyForce (force);
	}

	// creates the array that makes decisions for the state machine


}
