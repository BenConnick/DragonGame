using UnityEngine;
using System.Collections;
using BayesDemo;

public class NewDragon : VehicleBehavior {

	// state Enum
	public enum States { IDLING, WANDERING, CHASING, PATROLLING, FOLLOWING_PLAYER, ATTACKING }

	public enum Decisions { CLOSEST, TOWER, STAND }
	protected float decisionInterval = 2.0f;
	protected float timeSinceDecision = 0.0f;
	public Decisions decision = Decisions.TOWER;

	public int getDecision()
	{
		return (int)decision;
	}

	//Attributes 
	public float wanderWt = 10.0f;
	public float seekWt = 10.0f;
	public float alignWt = 100.0f;
	public float avoidWt = 100.0f;
	public float distFromEnemies;
	public float distFromObstacles;
	public float maxDistFromPlayer;
	public Transform tower;
	public GameObject player;
	protected Transform target;
	protected NavMeshAgent nAgent;
	protected int mode;
	EnemyManager manager;

	protected Bayes brain;

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

		//populate the obstacles array
		obstacles = GameObject.FindGameObjectsWithTag("obstacle");

		TakeAction (States.CHASING);

		brain = new Bayes ();
		brain.ReadObsTab ("dragonObservations.txt");
		brain.BuildStats();

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

		//update our decision timer
		timeSinceDecision += Time.deltaTime;
		//make a decision if it is time
		if (timeSinceDecision >= decisionInterval){
			//decide method
			makeDecision();
			//reset the timer
			timeSinceDecision = 0.0f;
		}

		// acquire target
		if (target == null) {
			//decide method
			makeDecision();
			//reset the timer
			timeSinceDecision = 0.0f;
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
			break;
			// go to target
		case 1:
			force += wanderWt * Wander();
			break;
			// arrive / follow
		case 2:
			if (target != null) force += seekWt * Pursue(target.gameObject,1f);
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

	//Dummy method for making a bayesian decision
	protected void makeDecision()
	{
		// find the closest enemy to dragon
		Vector3 minDistVec = new Vector3(100000000000,0,0);
		// store the closest
		EnemyBehavior closestToDrag = null;
		
		foreach (EnemyBehavior e in manager.enemies) {
			if ((transform.position - e.transform.position).sqrMagnitude < (transform.position - minDistVec).sqrMagnitude) {
				closestToDrag = e;
				minDistVec = e.transform.position;
			}
		}
		
		//if (closestToDrag != null) target = closestToDrag.transform;

		float closestEnemyDistanceToDragon = (transform.position - closestToDrag.transform.position).magnitude;



		EnemyBehavior closest = (EnemyBehavior)manager.enemies[0];
		minDistVec = new Vector3(1.0f, 0.0f, 0.0f) * 40.0f;
		// find clsoest to tower
		foreach (EnemyBehavior e in manager.enemies) {
			if ((tower.position - e.transform.position).sqrMagnitude < (tower.position - closest.transform.position).sqrMagnitude) {
				closest = e;
				minDistVec = e.transform.position;
			}
		}

		float closestEnemyDistanceToTower = (tower.position - minDistVec).magnitude;

		// get info about world
		// find how many dudes are "near" the closest dude
		int enemiesNearDragonEnemy = 0;
		foreach (EnemyBehavior e in manager.enemies) {
			if ((closestToDrag.transform.position - e.transform.position).sqrMagnitude < 300) {
				enemiesNearDragonEnemy++;
			}
		}
		
		int enemiesNearTowerEnemy = 0;
		foreach (EnemyBehavior e in manager.enemies) {
			if ((closest.transform.position - e.transform.position).sqrMagnitude < 300) {
				enemiesNearTowerEnemy++;
			}
		}


		//let's do something bayesian in here.
		//brain.Decide(/*closest to dragon, numbydragon, closest to tower, num by tower*/);
		bool goHome = brain.Decide(
			closestEnemyDistanceToDragon, 
			enemiesNearDragonEnemy, 
			closestEnemyDistanceToTower, 
			enemiesNearTowerEnemy);
		print (goHome);

		//now dragon decides which enemy to pursue -- if true, enemy closest to tower
		//-- if false, enemy closest to dragon
		if (goHome) { target = closest.transform;}
		else { target = closestToDrag.transform;}

		//update the HUD to reflect dragon's decision
		player.GetComponent<CommandControls>().UpdateHUD((int)decision);
		//tell me you've made a decision
		UnityEngine.Debug.Log("Made a new decision after " + timeSinceDecision);


	}

}
