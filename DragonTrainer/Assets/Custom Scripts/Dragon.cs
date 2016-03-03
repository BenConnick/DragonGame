using UnityEngine;
using System.Collections;
using Assets.Custom_Scripts;

public class Dragon : VehicleBehavior {

	//Attributes 
    public float wanderWt = 10.0f;
	public float seekWt = 10.0f;
    public float alignWt = 100.0f;
    public float avoidWt = 100.0f;
    public float distFromEnemies;
    public float distFromObstacles;
    public float maxDistFromPlayer;
	protected Transform target;
	protected NavMeshAgent nAgent;
    protected int mode;
	EnemyManager manager;

    //keep track of surroundings
    protected GameObject[] enemies;
    protected GameObject[] obstacles;
    
    // stack of orders (command history)
    protected int[] orders; // commands are in enum/int form
    protected int lastOrder=-1;
    protected int currentOrder;

    // stack of actions (action history)
    protected int[] actions; // actions are in enum/int form
    protected int lastAction=-1;

    protected int state;

    protected Decision decisionTreeRoot;

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
        //call the vehicle start method
        base.Start ();

        // get manager reference
		manager = FindObjectOfType<EnemyManager>();

        //populate the arrays
        enemies = GameObject.FindGameObjectsWithTag("enemy");
        obstacles = GameObject.FindGameObjectsWithTag("obstacle");

		target = GameObject.FindGameObjectWithTag ("tower").transform;

        // instantiate history
        orders = new int[100];
        actions = new int[100];

        // create decision tree
        decisionTreeRoot = BuildTreeFromCSVString("?command,?wander,wander,?goto,goto,?attack,?fire,fire,slash,wander,?wandering,?toofar,goto,wander,?attacking,wander,?slashing,slash,fire");
	}

    void Update()
    {
        base.Update();

    }

    // increases the alloted data for the array if they get too big
    protected void BufferArrayLength<T>(T[] arr, int lastEntry)
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
        switch (command)
        {
            case 0:

            default:
                break;
        }
        AddOrder(command);
    }

    // add a new order to the stack
    protected void AddOrder(int order)
    {
        lastOrder++;
        orders[lastOrder] = order;
        BufferArrayLength<int>(orders, lastOrder);
    }

    // add a new action to the stack
    protected void AddAction(int action)
    {
        lastAction++;
        actions[lastAction] = action;
        BufferArrayLength<int>(actions, lastAction);
    }

    // uses a binary decision tree to decide on a course of action
    protected void MakeDecision()
    {

    }

    protected override void CalcSteeringForce()
    {
		Vector3 force = Vector3.zero;

        switch (state)
        {
            // wander
            case 0:
                force += wanderWt * Wander();
                break;
            // go to target
            case 1:
                // NOTE TO SELF: set target
                force += seekWt * Arrival(target.position);
                break;
            // try to intercept
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

    protected Decision BuildTreeFromCSVString(string treeStr)
    {
        // do a reverse pre-order traversal of comma separated string
        string[] nodeStrings = treeStr.Split(new char[] {','});
        print(nodeStrings.Length);
        Decision root = BuildTreeRecursiveHelper(nodeStrings, 0);
        return root;
    }

    protected Decision BuildTreeRecursiveHelper(string[] nodeStrings, int idx)
    {
        // return null if invalid
        if (idx >= nodeStrings.Length) return null;

        // debug
        print("BTRH " + nodeStrings[idx]);

        // create the top node at this step in the recursion
        Decision root = new Decision(nodeStrings[idx]);

        // is this a branch or a leaf? Branches are denoted by a '?' as their first char
        if (nodeStrings[idx][0] != '?')
        {
            // if this is not a branch, we are done
            return root;
        }

        // index out of bounds checking
        if (idx + 1 < nodeStrings.Length)
        {
            // add yes node
            root.AddYesNode(BuildTreeRecursiveHelper(nodeStrings, idx + 1));
        }
        else
        {
            return root; // exit early if you run out of strings
        }
        // index out of bounds checking
        if (idx + GetNumNodeChildren(root.Yes) + 1 < nodeStrings.Length)
        {
            // add no node
            root.AddNoNode(BuildTreeRecursiveHelper(nodeStrings, idx + GetNumNodeChildren(root.Yes) + 1));
        }
        return root;
    }

    // checks how many nodes are children of this node (depth first recursive)
    protected int GetNumNodeChildren(Decision n) {
        if (n == null) return 0;
        int yesChildren = GetNumNodeChildren(n.Yes);
        int noChildren = GetNumNodeChildren(n.No);
        return 1 + yesChildren + noChildren;
    }

    public string CreateTreeString(Decision rootNode)
    {
        if (rootNode == null) return "";
        string returnStr = rootNode.Content;
        if (rootNode.Yes != null) returnStr += "," + CreateTreeString(rootNode.Yes);
        if (rootNode.No != null) returnStr += "," + CreateTreeString(rootNode.No);
        return returnStr;
    }
}
