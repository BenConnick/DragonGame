using UnityEngine;
using System.Collections;

public class CommandControls : MonoBehaviour {

    public Dragon dragon; // set in inspector

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Alpha1)) {
            dragon.Command(1);
            print("one pressed");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            print("two pressed");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print("three pressed");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            print("four pressed");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            print("five pressed");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            print("six pressed");
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
}
