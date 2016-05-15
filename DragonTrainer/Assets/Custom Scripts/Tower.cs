using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

	HealthBar healthBar;
	float health = 100f;

	public float Health {
		get {
			return health;
		}
		set {
			health = value;
		}
	}
	// Use this for initialization
	void Start () {
		healthBar = GameObject.FindObjectOfType<HealthBar> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	public void Damage(float amount) {
		print (amount);
		health -= amount;
		healthBar.DisplayHealth (health);
	}
}
