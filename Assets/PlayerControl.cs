using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
	private float current_speed = 0.0f;
	private float jump_speed = 0.0f;
	private float current_acceleration = 0.0f;
	private float currentJumpAcceleration = 0.0f;
	public float max_speed = 0.5f;
	public string movement = "Iso";

	public float max_accel = 2.7F;
	public float jump_accel = 2.7F;
	public float jumpDistance = 8.0F;

	private Map m;
	GameManager gm;
	Movement mv;

	public Vector3 velocity = Vector3.zero;
	public int maxJumps = 2;
	private int currentJumps = 0;
	private bool is_jumping = false;

	private bool flip_map = false;
	private Animator anim;
	CharacterController controller;

	// Use this for initialization
	void Start()
	{
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		m = gm.m;
		mv = GetComponent<Movement> ();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (gm.duringGame && !gm.freezing())
		{

			if (!Input.anyKey)
			{
				//current_speed = 0;
				current_acceleration = -max_accel * 2;
				current_speed += current_acceleration * Time.deltaTime;

			}
			else
			{

				current_acceleration = max_accel;
				current_speed += current_acceleration * Time.deltaTime;
			}
			if (current_speed > max_speed)
			{
				current_speed = max_speed;
			}
			if (current_speed <= 0)
			{
				current_speed = 0;
			}
			//Debug.Log("Falling");
			
			//velocity.y -= (jump_speed * Time.deltaTime + 0.5f * gravity * Mathf.Pow(Time.deltaTime, 2));
			//if (jump_speed <= 0)
			//{
			//jump_speed = 0;
			//}
			//Debug.Log("Grounded: "+controller.isGrounded);
			if (controller.isGrounded)
			{
				//jump_speed = 0;
				currentJumpAcceleration = 0;
			}
			//Debug.Log(Time.deltaTime);
			//Debug.Log(velocity.y);
			if (controller.isGrounded)
			{
				velocity = Vector3.zero;
				if (Input.GetKey(KeyCode.W))
				{
					this.transform.LookAt(this.transform.position + new Vector3(1, 0, 0));



				}
				else if (Input.GetKey(KeyCode.A))
				{
					this.transform.LookAt(this.transform.position + new Vector3(0, 0, 1));



				}
				else if (Input.GetKey(KeyCode.S))
				{
					this.transform.LookAt(this.transform.position + new Vector3(-1, 0, 0));
				}
				else if (Input.GetKey(KeyCode.D))
				{
					this.transform.LookAt(this.transform.position + new Vector3(0, 0, -1));
				}
				if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)){
					velocity += (this.transform.forward * (current_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow(Time.deltaTime, 2)));
				}

			}
			
			if (Input.GetKey(KeyCode.Space))
			{

				gm.freeze(1f);
				int ver = m.getVerNumber(transform.position.y);
				int row = m.getRowNumber(transform.position.x);
				int col = m.getColNumber(transform.position.z);
				if(gm.switchState(row,col)){
					gm.timeTravel(gameObject, gm.getPlayerState());
				} else {
					
				}
			}
			anim.SetFloat("Speed", velocity.magnitude);
			controller.Move(velocity);
			if ((controller.collisionFlags & CollisionFlags.Sides) == 0)
			{
				if (controller.isGrounded)
				{
					//Camera.main.transform.position += new Vector3(velocity.x, 0, velocity.z);
				}
				else
				{
					//Camera.main.transform.position += velocity;

				}
			}
			if (this.transform.position.y < -20)
			{
				currentJumps = 0;
				this.transform.position = new Vector3(2, 4, 2);
			}

		}
	}
	
	public void respawn(){
		Debug.Log ("Respawning.");
		this.transform.position = new Vector3 (0.5f*Map.TILE_SIZE,0f,(float)(m.getNCols()-0.5)*Map.TILE_SIZE);
	}
	
	public int getJumpsRemaining()
	{
		return maxJumps - currentJumps;
	}
}
