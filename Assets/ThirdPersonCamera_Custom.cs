using UnityEngine;
using System.Collections;

public class ThirdPersonCamera_Custom : MonoBehaviour {

	public GameObject target;
	public float damping = 1;
	public float distance = 40f;
	Vector3 offset;
	public float height;
	private Vector3 velocity = Vector3.zero;
	public bool look_back = false;
	void Start() {
		height = transform.position.y;
	}
	
	void LateUpdate() {
		//float currentAngle = transform.eulerAngles.y;
		//float desiredAngle = target.transform.eulerAngles.y;
		//float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);
		Vector3 new_position;
		if (look_back) {
			new_position = target.transform.position - distance * target.transform.forward;
		} else {
			new_position = target.transform.position + distance * target.transform.forward;

		}
		transform.position = Vector3.SmoothDamp (transform.position, new Vector3 (new_position.x, height, new_position.z), ref velocity, damping);
		transform.LookAt(target.transform);
	}
}
