using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class BowShoot : MonoBehaviour {

	public GameObject AmmoGO;
	public GameObject xRotGO;
	public GameObject yRotGO;
	public Camera bowCamera;
	public bool shootOnClick = false;
	public float reloadTime = 0.01f;
	public float maxPullTime = 3.0f;
	public float maxProjectileForce = 10.0f;
	protected float reloadTimer = 0.5f;
	protected bool pulling = false;
	protected float pForce = 0.0f;
	protected float pullTimer = 0.0f;
	protected float minFOV = 30;
	protected float maxFOV = 60;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (reloadTimer > 0) {
			reloadTimer -= Time.deltaTime;
		}
		if (shootOnClick && Input.GetMouseButtonDown(0) && reloadTimer < 0) {
			pulling = true;
		}
		if (pulling) {
			pullTimer += Time.deltaTime;
			bowCamera.GetComponent<Camera>().fieldOfView = minFOV + Mathf.Min (1,Mathf.Lerp(0,maxPullTime,pullTimer)) * (maxFOV - minFOV);
		}
		if (shootOnClick && Input.GetMouseButtonUp(0) && reloadTimer < 0 && pullTimer > 0) {
			pForce = maxProjectileForce * Mathf.Min(1,pullTimer/maxPullTime);
			Shoot();
			reloadTimer = reloadTime;
			pullTimer = 0.0f;
			pulling = false;
		}
		bowCamera.fieldOfView = maxFOV - Mathf.Min (1,Mathf.Lerp(0,maxPullTime,pullTimer)) * (maxFOV - minFOV);
	}



	public void Shoot() {
		Quaternion lookRot = xRotGO.transform.rotation;// * yRotGO.transform.rotation; 
		GameObject projectile = (GameObject)GameObject.Instantiate(AmmoGO,transform.position + transform.right*0.3f + transform.up*0.3f + transform.rotation*new Vector3(0,0,2), lookRot);
		projectile.GetComponent<Rigidbody>().AddForce(lookRot*new Vector3(0,0,pForce));
	}
}
