using UnityEngine;
using System.Collections;

public class HorizontalFollow : MonoBehaviour {
	public float xoffset;
	public float zoffset;
	private float yoffset;
	public GameObject target;
	// Use this for initialization
	void Start () {
		yoffset = transform.position.y - target.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (target.transform.position.x+xoffset,transform.position.y,target.transform.position.z+zoffset);
	}
}
