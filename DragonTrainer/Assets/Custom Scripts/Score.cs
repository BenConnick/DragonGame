using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	public int score = 0;
	protected Text scoreText; 

	// Use this for initialization
	void Start () {
		scoreText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DisplayScore() {
		scoreText.text = "Score: " + score;
	}
}
