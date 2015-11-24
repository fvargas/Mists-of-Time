using UnityEngine;
using System.Collections;

public class TileControl : MonoBehaviour {
	public static int STATE_IDLE = 0;
	public static int STATE_DESTROY = 1;
	public static int STATE_BORN = 2;
	public int state = STATE_IDLE;
	public float speed = 0f;
	public float acc;
	public float delay = 0f;
	public float current_delay = 0f;
	public float target_y;
	private Map m;
	private bool control_alpha;

	// Use this for initialization
	void Start () {
	}

	public void destroy(float delay, float tar_y,bool alpha){
		this.delay = delay;
		this.state = STATE_DESTROY;
		this.target_y = tar_y;
		this.control_alpha = alpha;
		if (!alpha)
			return;
		var temp = this.GetComponent<Renderer>().material.color;
		temp.a = 1f;
		this.GetComponent<Renderer>().material.color=temp;
	}

	public void born(float delay, float tar_y, Map m,bool alpha){

		this.delay = delay;
		this.state = STATE_BORN;
		this.target_y = tar_y;
		this.m = m;
		this.control_alpha = alpha;
		if (!alpha)
			return;
		var temp=this.GetComponent<Renderer>().material.color;
		temp.a = 0f;
		this.GetComponent<Renderer>().material.color=temp;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == STATE_BORN || state == STATE_DESTROY) {
			if(current_delay < delay){
				current_delay += Time.deltaTime;
				return;
			}
			//Debug.Log(this.acc+","+speed+","+Time.deltaTime);
			this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y+(target_y-this.transform.position.y)*Time.deltaTime*10,this.transform.position.z);
		}
		if (state == STATE_BORN) {
			if (this.transform.position.y >= target_y-0.1f) {
				this.transform.position = new Vector3(this.transform.position.x,target_y,this.transform.position.z);

				current_delay = 0f;
				//Debug.Log (this.m);
				state = STATE_IDLE;
				if(control_alpha){
					var temp = this.GetComponent<Renderer>().material.color;
					temp.a = 1f;
					this.GetComponent<Renderer>().material.color=temp;
				}
			}else{
				if(control_alpha){
					var temp = this.GetComponent<Renderer>().material.color;
					temp.a = temp.a+(1-temp.a)*Time.deltaTime*5;
					this.GetComponent<Renderer>().material.color=temp;
				}
				//Debug.Log(this.GetComponent<Renderer>().material.color.a);
			}
		} else if (state == STATE_DESTROY) {
			if (this.transform.position.y <= target_y + 0.1f) {
				Destroy(gameObject);
			}else{
				if(control_alpha){
					var temp = this.GetComponent<Renderer>().material.color;
					temp.a = temp.a+(0-temp.a)*Time.deltaTime*5;
					this.GetComponent<Renderer>().material.color=temp;
				}
				//Debug.Log(this.GetComponent<Renderer>().material.color.a);
			}
		}
	}
}
