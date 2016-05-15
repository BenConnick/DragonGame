using UnityEngine;
using System.Collections;

public class ArrowTarget : MonoBehaviour {

	public GameObject particleEffect;
	public bool destroyOnHit = false;

	protected Score score;

	// Use this for initialization
	void Start () {
		score = GameObject.FindObjectOfType<Score> ();
		print (score);
	}

	void OnCollisionEnter (Collision col)
	{
		//print(col.gameObject.name);
		if(col.gameObject.name.Contains("Arrow") || col.gameObject.name.Contains("Dragon"))
		{
			if (particleEffect) {
				ShowBlood ();
			}
			//print (gameObject.name + " HIT");
			if (destroyOnHit) {
				EnemyBehavior e = gameObject.GetComponent<EnemyBehavior>();
				e.setHealth (0);
				score.score += 15;
				score.DisplayScore ();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void ShowBlood() {
		GameObject.Instantiate(particleEffect,transform.position,transform.rotation);
	}
}
