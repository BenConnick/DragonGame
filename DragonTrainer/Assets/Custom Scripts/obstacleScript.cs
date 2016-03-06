using UnityEngine;
using System.Collections;

public class obstacleScript : MonoBehaviour {

    GameObject[] waypoints;
    GameObject tower;

    public float distFromTower;
    public float distFromPath;

	// Use this for initialization
	void Start () {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        tower = GameObject.Find("Tower");

        //keep trees away from tower
        if(Mathf.Abs((tower.transform.position - transform.position).magnitude) < distFromTower)
        {
            //move the tree away from tower
            transform.position = new Vector3(Random.Range(-40, 100), 0.0f, Random.Range(300, 360));
        }

        //keep trees away from path
        for(int i = 0; i < waypoints.Length; i++)
        {
            if(Mathf.Abs((waypoints[i].transform.position - transform.position).magnitude) < distFromPath)
            {
                //move the tree away from path -- temporary value for this milestone
                transform.position = new Vector3(Random.Range(-40, 100), 0.0f, Random.Range(300, 360));
            }
        }

        //give variety to the tree's appearance
        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Random.Range(0, 180));
        transform.localScale += new Vector3(0, Random.Range(-2, 6)/10.0f, 0);
        gameObject.GetComponent<Renderer>().materials[1].color = new Color(Random.Range(0, 5) / 10.0f, 0.55f, 0.08f);
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
}
