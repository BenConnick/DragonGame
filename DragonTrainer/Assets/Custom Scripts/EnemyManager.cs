using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	GameObject[] waypoints;
	GameObject tower;

	// Use this for initialization
	void Start () {
		waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		tower = GameObject.Find("Tower");
		// assign waypoints indicies in order of closest to farthest from the tower
		IndexWaypoints();
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
