using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CommandControls : MonoBehaviour {

    public Dragon dragon; // set in inspector
    public Canvas canvas; //set in the inspector
    public GameObject[] command = new GameObject[3];
    public Texture[] comText = new Texture[6];
	// Use this for initialization
	void Start () {
        command[0].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[0];
        command[1].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[1];
        command[2].transform.GetComponent<UnityEngine.UI.RawImage>().texture = comText[5];
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
