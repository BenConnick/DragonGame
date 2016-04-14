using UnityEngine;
using System.Collections;

public class Dragon : VehicleBehavior {

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

	// state machine 2D int array
	protected int[,] stateMachine;
    
    // stack of orders (command history)
    protected int[] orders; // commands are in enum/int form
    protected int lastOrderIdx=0; // this should be -1 but for debugging we set it to a default value
    protected int currentOrder=-1;

    // stack of actions (action history)
    protected int[] actions; // actions are in enum/int form
    protected int lastActionIdx = 0; // this should be -1 but for debugging we set it to a default value

    protected int state = 0;

    // the manifestation of the top of the action stack
    public int State
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

		target = player.transform; // set target player

        // instantiate history
        orders = new int[100]; // list of orders given
		actions = new int[100]; // list of all actions taken (state history)

		SetStateTransitions(); // create state machine

        AddAction(1); // start wandering

		print ("manager's public value numOfEnemies: " + manager.numOfEnemies);
	}

    void Update()
    {
		// find new state using statemachine rules
		int newState = stateMachine [Mathf.Max(0,currentOrder), state]; // changes state based on command arg

		// only change state if there is a difference
		if (newState != state) {
			// debug
			print("" + state + "->" + newState);

			// update state
			state = newState;

			// update record / history
			AddAction (state);
		}
		// order followed, wait for new order
		currentOrder = -1;

		// move around
        base.Update();
        // parent (base) calls CalcSteeringForce
    }

	// set the dragon's target
	public void SetTarget(Transform t) {
		target = t;
	}

    // increases the alloted data for the array if they get too big
    protected void BufferArrayLength<T>(ref T[] arr, int lastEntry)
    {
        if (lastEntry > arr.Length / 2)
        {
            T[] temp = new T[arr.Length*2];
            arr.CopyTo(temp,0);
            arr = temp;
        }
    }

    // give the dragon a command from CommandControls
    public void Command(int command)
    {
        AddOrder(command);
    }

    // add a new order to the stack
    protected void AddOrder(int order)
    {
        lastOrderIdx++;
        orders[lastOrderIdx] = order;
        currentOrder = order;
        BufferArrayLength<int>(ref orders, lastOrderIdx);
    }

    // add a new action to the stack
    protected void AddAction(int action)
	{
		lastActionIdx++;
		actions [lastActionIdx] = action;
		BufferArrayLength<int> (ref actions, lastActionIdx);
	}
		

	// move the Dragon
    protected override void CalcSteeringForce()
    {
		// reset force vector
		Vector3 force = Vector3.zero;

		// check what state the dragon is in
        switch (state)
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
                // NOTE TO SELF: set target
                force += seekWt * Arrival(target.position);
                break;
            // try to intercept
            case 3:
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
	protected void SetStateTransitions() {
		stateMachine = new int[,] {
			//							state 0:	state 1:	state 2:	state 3:	state 4:	state 5:	state 6:	state 7:
			//		  					idling		wandering 	chasing		patrolling	following	slashing	burning		other		
			/* 0: no cmd		*/	 { 0,			1,			2,			3,			4,			5,			6,			7 }, 	
			/* cmd 1: wander	*/	 { 1,			1,			1,			1,			1,			1,			1,			1 }, 	
			/* cmd 2: go to		*/	 { 2,			2,			2,			2,			2,			2,			2,			2 }, 	
			/* cmd 3: patrol	*/	 { 3,			3,			3,			3,			3,			3,			3,			3 }, 	
			/* cmd 4: folloW	*/	 { 4,			4,			4,			4,			4,			4,			4,			4 }, 	
			/* cmd 5: slash		*/	 { 5,			5,			5,			5,			5,			5,			5,			5 }, 	
			/* cmd 6: burn		*/	 { 6,			6,			6,			6,			6,			6,			6,			6 },	
			/* cmd 7: stay		*/	 { 7,			7,			7,			7,			7,			7,			7,			7 },	
		};
	}

}
