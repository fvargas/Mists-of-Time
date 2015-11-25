using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
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

    public int player_state;

    public Hashtable gos = new Hashtable();

    private Map m;
    GameManager gm;
    Movement mv;
    SoundManager sm;

    public Vector3 velocity = Vector3.zero;
    public int maxJumps = 2;
    private int currentJumps = 0;
    private bool is_jumping = false;

    private bool flip_map = false;
    private Animator anim;
    CharacterController controller;
    public AudioClip time_travel_fail;

    // Use this for initialization
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        m = gm.m;
        mv = GetComponent<Movement>();
        sm = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        player_state = 0;
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

				float z_axis = UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis("Vertical");
				float x_axis = UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis("Horizontal");
				if(z_axis==0 && x_axis == 0){
					z_axis = Input.GetAxis("Vertical");
					x_axis = Input.GetAxis("Horizontal");
					//Debug.Log (z_axis+" "+x_axis);
				}
				if (z_axis > 0 && (Mathf.Abs(z_axis) >= Mathf.Abs(x_axis)))
				{
					this.transform.LookAt(this.transform.position + new Vector3(1, 0, 0));
				}
				else if (x_axis < 0 && (Mathf.Abs(x_axis) > Mathf.Abs(z_axis)))
				{
					this.transform.LookAt(this.transform.position + new Vector3(0, 0, 1));
				}
				else if (z_axis < 0 && (Mathf.Abs(z_axis) >= Mathf.Abs(x_axis)))
				{
					this.transform.LookAt(this.transform.position + new Vector3(-1, 0, 0));
				}
				else if (x_axis > 0 && (Mathf.Abs(x_axis) > Mathf.Abs(z_axis)))
				{
					this.transform.LookAt(this.transform.position + new Vector3(0, 0, -1));
				}
				if (x_axis != 0 || z_axis != 0){
					velocity += (this.transform.forward * (current_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow(Time.deltaTime, 2)));
				}

			}
			
			if (Input.GetKey(KeyCode.Space) || UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetButtonDown("Jump"))
			{
				if (isLocalPlayer)
                {
					gm.freeze(1f);
					int ver = m.getVerNumber(transform.position.y);
					int row = m.getRowNumber(transform.position.x);
					int col = m.getColNumber(transform.position.z);
					int next_state = gm.switchState(row, col, player_state);
					if (player_state != next_state){
						player_state = next_state;
						//gm.timeTravel(gameObject, gm.getPlayerState());
						foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
	                    {
	                        timeTravel(gameObject, player_state);
	                    }
						DoubleVision dv = Camera.main.GetComponent<DoubleVision> ();
						dv.enabled = true;
						dv.resetEffect (1.5f);
						RGBSplit rgb_sp = Camera.main.GetComponent<RGBSplit> ();
						rgb_sp.enabled = true;
						rgb_sp.resetEffect(1.5f);

					}else{
						sm.PlaySingle(time_travel_fail);
					}
				}
			}
            anim.SetFloat("Speed", Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z));
            //velocity.y -= 10f*Time.deltaTime;//TODO:replace hardcoded 40
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

    public void respawn()
    {
        Debug.Log("Respawning.");
        this.transform.position = new Vector3(0.5f * Map.TILE_SIZE, 0f, (float)(m.getNCols() - 0.5) * Map.TILE_SIZE);
    }

    public int getJumpsRemaining()
    {
        return maxJumps - currentJumps;
    }

    public void timeTravel(GameObject obj, int state)
    {
        Debug.Log(state);
        gos[obj] = state;
        //int current_state = obj.GetComponent<PlayerControl>().player_state;
        foreach (DictionaryEntry de in gos)
        {
			((GameObject)de.Key).GetComponent<EffectControl>().showParticleEffect("holy",1);
			((GameObject)de.Key).GetComponent<Movement>().time_travel_timer = 1f;
            if ((int)de.Value == player_state)
            {
                foreach (Renderer r in ((GameObject)de.Key).GetComponentsInChildren<Renderer>())
                {
                    r.enabled = true;
                }
                foreach (Collider c in ((GameObject)de.Key).GetComponentsInChildren<Collider>())
                {
                    c.enabled = true;
                }
            }
            else
            {
                foreach (Renderer r in ((GameObject)de.Key).GetComponentsInChildren<Renderer>())
                {
                    r.enabled = false;
                }
                foreach (Collider c in ((GameObject)de.Key).GetComponentsInChildren<Collider>())
                {
                    c.enabled = false;
                }
            }
        }
    }
}