using UnityEngine;
using System.Collections;

public class IsoCamera : MonoBehaviour {
	public GameObject target;
	public float xoffset = -475f;//546f;
	public float zoffset = -475f;//546f;
	public float height = 560f;//460f;
	public float size = 60f;

	private Vector3 reverseProjectionPosition(Vector3 projection_position){
		Vector3 result = new Vector3();
		result.y = projection_position.y + height;
		result.x = projection_position.x + xoffset;
		result.z = projection_position.z + zoffset;
		return result;
	}

	// Use this for initialization
	void Start () {
		//Set up rotation
		transform.eulerAngles = new Vector3 (40,45,0);
	}
	
	// Update is called once per frame
	void Update () {
		if (target == null) {
			return;
		}
		transform.position = reverseProjectionPosition (target.transform.position);
	}
}
