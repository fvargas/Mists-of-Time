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
	public float maxJumpSpeed = 8.0F;
	public float max_accel = 2.7F;
	public float jump_accel = 2.7F;
	public float jumpDistance = 8.0F;
	public float gravity = 20.0F;
	private Map m;
	GameManager gm;
	public Vector3 moveDirection = Vector3.zero;
	private Vector3 velocity = Vector3.zero;
	public int maxJumps = 2;
	private int currentJumps = 0;
	private bool is_jumping = false;
	private float prev_y_velocity = 0f;
	private bool flip_map = false;
	private Animator anim;
	// Use this for initialization
	void Start()
	{
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (gm.duringGame)
		{
			CharacterController controller = GetComponent<CharacterController>();
			m = gm.m;
			//Debug.Log("x "+this.transform.position.x+" z "+this.transform.position.z);
			if(controller.isGrounded){
				moveDirection = new Vector3(0, 0, 0);
			}
			if (controller.isGrounded && !anim.IsInTransition(0))
			{
				anim.SetBool("Jump", false);

				int row = Map.getRowNumber(this.transform.position.x);
				int ver = 0;
				int col = Map.getColNumber(this.transform.position.z);
				//this.transform.localPosition = new Vector3(Map.getXCoordinate(row),transform.localPosition.y,Map.getZCoordinate(col));
				//Debug.Log("x "+this.transform.position.x+", row "+row+"z "+this.transform.position.z+", col "+col);
				if (m.g[row, ver, col] == Map.GO_LAVA)
				{
					currentJumps = 0;
					//respawn();
				}
				if (m.g[row, ver, col] == Map.GO_DEST)
				{
					Debug.Log("You Win.");
					currentJumps = 0;
					GameObject.Find("GameManager").GetComponent<GameManager>().EndGame();
				}
				//this.transform.position = new Vector3(Map.getXCoordinate(row),this.transform.position.y,Map.getZCoordinate(col));
				flip_map = false;
				if (Input.GetKey(KeyCode.W))
				{
					
					this.transform.LookAt(this.transform.position + new Vector3(1, 0, 0));
					int next_col = col + 1;
					if(next_col < m.getNCols()){
						if(Map.areFlippableTiles(m.g[row,ver,col],m.g[row,ver,next_col])){
							flip_map = true;
						}
					}
					
				}
				if (Input.GetKey(KeyCode.A))
				{
					this.transform.LookAt(this.transform.position + new Vector3(0, 0, 1));
					int next_row = row - 1;
					if(next_row >= 0){
						if(Map.areFlippableTiles(m.g[row,ver,col],m.g[next_row,ver,col])){
							flip_map = true;
						}
					}
				}
				if (Input.GetKey(KeyCode.S))
				{
					this.transform.LookAt(this.transform.position + new Vector3(-1, 0, 0));
					int next_col = col - 1;
					if(next_col >= 0){
						if(Map.areFlippableTiles(m.g[row,ver,col],m.g[row,ver,next_col])){
							flip_map = true;
						}
					}
					
				}
				if (Input.GetKey(KeyCode.D))
				{
					this.transform.LookAt(this.transform.position + new Vector3(0, 0, -1));
					int next_row = row + 1;
					if(next_row < m.getNRows()){
						if(Map.areFlippableTiles(m.g[row,ver,col],m.g[next_row,ver,col])){
							flip_map = true;
						}
					}
				}
				if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
				{
					if(flip_map && !gm.hasFlip()){
						return;
					}
					anim.SetBool("Jump",true);
					moveDirection = new Vector3(0, 0, 1);
					moveDirection = transform.TransformDirection(moveDirection);
					moveDirection *= 1.8f;
					moveDirection.y = maxJumpSpeed;
				}
			}

			controller.Move(moveDirection * Time.deltaTime);

			prev_y_velocity = moveDirection.y;
			moveDirection.y -= gravity * Time.deltaTime;
			if(prev_y_velocity > 0f && moveDirection.y < 0f){
				if(flip_map){
					flip_map = false;
					int row = Map.getRowNumber(this.transform.position.x);
					int col = Map.getColNumber(this.transform.position.z);
					gm.flipMap(row,col);
				}
			}

			if (this.transform.position.y < -20)
			{
				respawn();
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
