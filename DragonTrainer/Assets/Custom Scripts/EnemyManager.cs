using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//ben version
public class EnemyManager : MonoBehaviour {

	GameObject[] waypoints;
	GameObject tower;

    //can be set in the editor
    public int numOfEnemies = 10;
    public int numOfObstacles = 100;

    //set manually in editor so the program understands
    public Object enemyPrefab;
    private GameObject[] tempEnemies;
    public Object treePrefab;

    public ArrayList enemies = new ArrayList();

    // Use this for initialization
    void Start () {
		waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		tower = GameObject.Find("Tower");
		// assign waypoints indicies in order of closest to farthest from the tower
		IndexWaypoints();

        //create obstacles
        for (int i = 0; i < numOfObstacles; i++)
        {
            //create them all over the terrain
            Vector3 pos = new Vector3(Random.Range(-130, 330), 0.0f, Random.Range(-100, 330));
            //put the obstacles on top of terrain
            pos.y = Terrain.activeTerrain.SampleHeight(pos);
            //create now
            GameObject.Instantiate(treePrefab, pos, Quaternion.identity);
        }

        //create the specified number of enemies
        for (int i = 0; i < numOfEnemies; i++)
        {
            //create them at the far end of the path
            Vector3 pos = new Vector3(Random.Range(-40, 100), 0.0f, Random.Range(300, 360));
            //put the enemy on top of terrain
            pos.y = Terrain.activeTerrain.SampleHeight(pos) + 1f;
            //create now
            GameObject.Instantiate(enemyPrefab, pos, Quaternion.identity);
        }

        //find all enemies and put them into a public arraylist
        tempEnemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject go in tempEnemies)
        {
			enemies.Add(go.GetComponent<EnemyBehavior>());
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject[] Waypoints {
		get {
			return waypoints;
		}
	}

	/// <summary>
	/// Returns the distance squared to the tower
	/// </summary>
	/// <param name="go">Game Object</param>
	protected float DSq2tower(GameObject go) {
		return (go.transform.position - tower.transform.position).sqrMagnitude;
	}


	// produce a distance array for use with quickSort
	protected float[] DistArrFromGOArr(GameObject[] gos) {
		//print ("distance");
		float[] distances = new float[gos.Length];
		for (var i=0; i<gos.Length; i++) {
			distances[i] = DSq2tower(gos[i]);
		}
		return distances;
	}


	// auto assign waypoints their index by proximity to tower
	protected void QuickSortWaypoints(float[] arr, int left, int right) {

		int i = left, j = right;
		float tmp;
		GameObject tmpGO;
		float pivot = arr[(left + right) / 2];
		
		/* partition */
		while (i <= j) {	
			while (arr[i] < pivot)		
				i++;
			while (arr[j] > pivot)	
				j--;
			if (i <= j) {	
				// swap in dist array
				tmp = arr[i];	
				arr[i] = arr[j];	
				arr[j] = tmp;
				// swap in GO array
				tmpGO = waypoints[i];
				waypoints[i] = waypoints[j];
				waypoints[j] = tmpGO;
				// increment
				i++;	
				j--;
			}
		};
		
		
		/* recursion */
		if (left < j)
			QuickSortWaypoints(arr, left, j);
		if (i < right)
			QuickSortWaypoints(arr, i, right);
	}

	protected void AssignSortedWaypointIndicies(GameObject[] WPs) {
		//print("assign");
		for (int i=0; i<WPs.Length; i++) {
			WPs[i].GetComponent<Waypoint>().index = i;
		}
	}

	protected void IndexWaypoints() {
		//print ("index");
		float[] dists = DistArrFromGOArr(waypoints);
		// sort in-place (after this method, waypoints will be sorted)
		QuickSortWaypoints(dists,0,dists.Length-1);
		AssignSortedWaypointIndicies(waypoints);
	}
}
