using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Eric Stivala: my process for commands
//step 1: Pick a command with buttons 1-6
//step 2: Right click to issue said command
//step 3: repeat

public class CommandControls : MonoBehaviour {

    public Dragon dragon; // set in inspector
    public Canvas canvas; //set in the inspector
    public GameObject[] command = new GameObject[3]; //the HUD elements that we will update
    public Texture[] comText = new Texture[6]; // the textures we will use on the HUD

    int commandNum = 0; //stores what number command is in progress

    //variables for raycasting commands
    GameObject camera;
    Vector3 origin;
    Vector3 direction;
    Ray ray; //stores origin and distance
    RaycastHit hit; //used to get info back from the raycast
    public float maxRayDistance; //the maximum distance we can command

    //waypoint Marker -- helps the player see where they are aiming
    GameObject marker;

    //target for waypoint marker to highlight
    Transform targetEnemy;
    bool activeTarget = false;

    KeyCode prevInput; // keeps our previous input -- may not be necessary
	// Use this for initialization
	void Start () {
        //setting the textures to a default value
        command[0].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[5];
        command[1].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[5];
        command[2].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[5];
        command[3].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[5];

        //set the origin from where our raycast begins
        //camera = gameObject.GetComponent<Camera>();
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        origin = camera.transform.position;
        direction = camera.transform.forward;
        ray = new Ray(origin, direction);

        marker = GameObject.FindGameObjectWithTag("marker");
        marker.gameObject.GetComponent<Renderer>().material.color = new Color(0.5f, 0.2f, 0.6f); // idle tracking color -- purple for high contrast
	}
	
	// Update is called once per frame
	void Update () {
        if (commandNum != 0)
        {
            //reset the target variable
            if(activeTarget) {activeTarget = false;}

            castCommand();
        }

        if(activeTarget)
        {
            marker.transform.position = targetEnemy.transform.position;
        }

        //move to a point
	    if (Input.GetKeyDown(KeyCode.Alpha1)) {
            //dragon.Command(1); //tell the dragon our command
            //commandNum = 1; //update command in progress
            commandNum = 2; //update command in progress -- 2 for demo purposes
            marker.transform.localScale = new Vector3(1.0f, 6.0f, 1.0f);//scale to indicate point
            marker.gameObject.GetComponent<Renderer>().material.color = new Color(0.5f, 0.2f, 0.6f); // idle tracking color
            prevInput = KeyCode.Alpha1; //record our keypress
        }
        //patrol an area
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            commandNum = 2; //update command in progress
            marker.transform.localScale = new Vector3(30.0f, 6.0f, 30.0f);//scale to indicate area
            marker.gameObject.GetComponent<Renderer>().material.color = new Color(0.5f, 0.2f, 0.6f); // idle tracking color
            prevInput = KeyCode.Alpha2; //record our keypress
        }
        //target an enemy
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            commandNum = 3; //update command in progress
            marker.transform.localScale = new Vector3(2.0f, 6.0f, 2.0f);//scale to indicate enemy
            marker.gameObject.GetComponent<Renderer>().material.color = new Color(0.5f, 0.2f, 0.6f); // idle tracking color
            prevInput = KeyCode.Alpha3; //record our keypress
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            commandNum = 4; //update command in progress
            marker.transform.localScale = new Vector3(1.0f, 6.0f, 1.0f);//scale to indicate point
            marker.gameObject.GetComponent<Renderer>().material.color = new Color(0.5f, 0.2f, 0.6f); // idle tracking color
            prevInput = KeyCode.Alpha4; //record our keypress
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            commandNum = 5; //update command in progress
            marker.transform.localScale = new Vector3(1.0f, 6.0f, 1.0f);//scale to indicate point
            marker.gameObject.GetComponent<Renderer>().material.color = new Color(0.5f, 0.2f, 0.6f); // idle tracking color
            prevInput = KeyCode.Alpha5; //record our keypress
        }
        //wander command for demo
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            //commandNum = 6; //update command in progress
            commandNum = 1; //update command in progress -- wander for demo
            marker.transform.localScale = new Vector3(1.0f, 6.0f, 1.0f);//scale to indicate point
            marker.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f); // for wander
            prevInput = KeyCode.Alpha6; //record our keypress
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("Approval Key Pressed");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            print("Disapproval Key Pressed");
        }

        //put command through with a right click
        if (Input.GetMouseButtonDown(1) && commandNum != 0)
        {
            dragon.Command(commandNum); //tell the dragon our command
            UpdateHUD(commandNum); // command went through, update HUD

			// set target to location
			dragon.SetTarget(marker.transform);

            //set target if applicable
            if (commandNum == 3 && hit.transform != null)
            {
                targetEnemy = hit.transform;
				dragon.SetTarget (targetEnemy); // set target in dragon
                activeTarget = true;
            }

            commandNum = 0; //reset command in progress

            //change color of marker to indicate active command
            marker.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.5f, 0.1f); //active command color
        }
	}

    //for visual feedback on the HUD
    void UpdateHUD(int input){
        Texture next; //stores what the next image should be
        Texture current = comText[input - 1]; //command 1 is at index 0
        for(int i = 0; i < command.Length; i++)
        {
            next = command[i].transform.GetComponent<UnityEngine.UI.RawImage>().texture;
            command[i].transform.GetComponent<UnityEngine.UI.RawImage>().texture = current;
            current = next;
        }
    }

    //raycasting method for commands -- may need arg for command number
    void castCommand(){
        //update ray castin variables
        origin = camera.transform.position;
        direction = camera.transform.forward;
        ray = new Ray(origin, direction);

        //cast the ray
        Debug.DrawRay(origin, direction, Color.yellow);
        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            //move waypoint to point on the ground
            marker.transform.position = hit.point;
        }
    }
}
