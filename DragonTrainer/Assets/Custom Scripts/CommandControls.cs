using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CommandControls : MonoBehaviour {

    public Dragon dragon; // set in inspector
    public Canvas canvas; //set in the inspector
    public GameObject[] command = new GameObject[3]; //the HUD elements that we will update
    public Texture[] comText = new Texture[6]; // the textures we will use on the HUD

    KeyCode prevInput; // make sure we don't spamm the program with repeat info
	// Use this for initialization
	void Start () {
        //setting the textures to a default value
        command[0].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[0];
        command[1].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[3];
        command[2].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[4];
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Alpha1)) {
            dragon.Command(1); //tell the dragon our command
            UpdateHUD(1); //update the hud
            prevInput = KeyCode.Alpha1; //record our keypress
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && prevInput != KeyCode.Alpha2)
        {
            UpdateHUD(2); //update the hud
            prevInput = KeyCode.Alpha2; //record our keypress
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateHUD(3); //update the hud
            prevInput = KeyCode.Alpha3; //record our keypress
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UpdateHUD(4); //update the hud
            prevInput = KeyCode.Alpha4; //record our keypress
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UpdateHUD(5); //update the hud
            prevInput = KeyCode.Alpha5; //record our keypress
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UpdateHUD(6); //update the hud
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
}
