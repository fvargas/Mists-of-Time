using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
		{
			this.GetComponent<Camera>().orthographicSize = Mathf.Max(this.GetComponent<Camera>().orthographicSize-0.1f, 1f);
			this.GetComponent<Camera>().fieldOfView = Mathf.Max(this.GetComponent<Camera>().fieldOfView-0.8f, 20f);
			
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
		{
			this.GetComponent<Camera>().orthographicSize = Mathf.Min(this.GetComponent<Camera>().orthographicSize+0.1f, 25f);
			this.GetComponent<Camera>().fieldOfView = Mathf.Min(this.GetComponent<Camera>().fieldOfView+0.8f, 60f);
		}

	}
}
