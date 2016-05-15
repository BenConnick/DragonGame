using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	Image bar;
	float maxHealth = 100f;

	// Use this for initialization
	void Start () {
		bar = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DisplayHealth(float val) {
		bar.rectTransform.sizeDelta = new Vector2 (200f * (val/maxHealth), 30);
	}
}
