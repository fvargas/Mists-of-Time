using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

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


	public float item_cooldown = 0;
	public int [] items;
	public List<ActiveItem> active_items;
	private Text [] item_texts;
	private Text current_potion_stat_text;

	public float stun_timer = 0f;

	private float speed_boost = 0f;
	public bool shield_on = false;
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
		items = new int[3];
		for (int i=0; i<items.GetLength(0); i++) {
			items[i] = Item.ITEM_EMPTY;
		}
		item_texts = new Text[3];
		item_texts [0] = GameObject.Find ("Item_0_Txt").GetComponent<Text> ();
		item_texts [1] = GameObject.Find ("Item_1_Txt").GetComponent<Text> ();
		item_texts [2] = GameObject.Find ("Item_2_Txt").GetComponent<Text> ();
		item_texts [0].text = item_texts [1].text = item_texts [2].text = Item.ITEM_NAME_DICT [Item.ITEM_EMPTY];
		item_texts [0].color = item_texts [1].color = item_texts[2].color = Color.blue;
		active_items = new List<ActiveItem> ();
		current_potion_stat_text = GameObject.Find ("Potion_Stat_Txt").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
		if (gm.duringGame) {
			// Item cooldown
			if(item_cooldown > 0){
				item_cooldown -= Time.deltaTime;
				if(item_cooldown < 0)
					item_cooldown = 0;
				if(item_cooldown == 0){
					for(int i=0;i<item_texts.GetLength(0);i++){
						item_texts[i].color = Color.blue;
					}
				}else{
					for(int i=0;i<item_texts.GetLength(0);i++){
						item_texts[i].color = Color.red;
					}
				}

			}

			// Process active items
			speed_boost = 0f;
			shield_on = false;
			for (int i = active_items.Count-1; i >= 0; i--) {
				ActiveItem ai = active_items[i];
				if(ai.time_left > 0){
					//Potion Effects

					shield_on = false;
					if(ai.type == Item.ITEM_SPEEDUP){
						speed_boost += 2f;
					}else if(ai.type == Item.ITEM_SHIELD){
						shield_on = true;
					}
					//Update Potion
					ai.time_left -= Time.deltaTime;
					if(ai.time_left < 0){
						ai.time_left = 0;
					}
					if(ai.time_left == 0){
						active_items.RemoveAt(i);
						UpdatePotionStatText();
					}
				}
			}
		}


        if (gm.duringGame && !gm.freezingPlayer())
        {
			if(stun_timer > 0){
				stun_timer -= Time.deltaTime;
				if(stun_timer < 0){
					stun_timer = 0;
				}
				if(stun_timer == 0){
					UpdatePotionStatText();
				}
			}

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

			/* Stun Effect */
			if(stun_timer > 0 && current_speed > 1f){
				current_speed = 1f;
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
					velocity += (this.transform.forward * ((current_speed+speed_boost) * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow(Time.deltaTime, 2)));
				}

			}
			
			if (Input.GetKey(KeyCode.Space) || UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetButtonDown("Jump"))
			{
				//if (isLocalPlayer)
                //{
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
				//}
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

			//Item Control
			if (Input.GetKey(KeyCode.Q)&&!Input.GetKey(KeyCode.E)){
				ConsumeItem(0);
			}else if(!Input.GetKey(KeyCode.Q)&&Input.GetKey(KeyCode.E)){
				ConsumeItem(1);
			}else if(Input.GetKey(KeyCode.Q)&&Input.GetKey(KeyCode.E)){
				ConsumeItem(2);
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
        //Debug.Log(state+","+player_state);
        gos[obj] = state;
		obj.GetComponent<EffectControl>().showParticleEffect("holy",1);
		obj.GetComponent<Movement>().time_travel_timer = 1f;
        //int current_state = obj.GetComponent<PlayerControl>().player_state;
        foreach (DictionaryEntry de in gos)
        {
			//Debug.Log(de.Key+" "+de.Value);
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

	/* 
	 * Return the index of the empty slot, if no empty slot then return -1
	 */
	public int GetNextAvailableItemSlot(){
		int i = 0;
		while (i < items.GetLength(0)) {
			if(items[i] == Item.ITEM_EMPTY)
				return i;
			i += 1;
		}
		return -1;
	}

	public void PlaceItem(int slot_index,int item_id){
		items [slot_index] = item_id;
		item_texts [slot_index].text = Item.ITEM_NAME_DICT [item_id];
	}

	public void ConsumeItem(int slot_index){
		if (items [slot_index] == Item.ITEM_EMPTY || item_cooldown > 0)
			return;
		if (items [slot_index] == Item.ITEM_FREEZE) {
			Debug.Log ("FREEZE CONSUMED");
			gm.freeze(2f,false);
		} else if (items [slot_index] == Item.ITEM_SPEEDUP) {
			Debug.Log ("SPEEDUP CONSUMED");
			GetComponent<EffectControl>().showParticleEffect("wind",Item.ITEM_DURATION_DICT[items[slot_index]]);
		} else if (items [slot_index] == Item.ITEM_SHIELD) {
			Debug.Log ("SHIELD CONSUMED");
		}
		active_items.Add(new ActiveItem(items [slot_index]));
		item_cooldown = Item.ITEM_COOLDOWN_DICT[items [slot_index]];
		items[slot_index] = Item.ITEM_EMPTY;
		item_texts [slot_index].text = Item.ITEM_NAME_DICT [Item.ITEM_EMPTY];
		UpdatePotionStatText ();
	}

	private void UpdatePotionStatText(){
		//Debug.Log ("PotionTxt");
		//Debug.Log (stun_timer);
		if (active_items.Count == 0 && stun_timer == 0) {
			current_potion_stat_text.text = "";
			return;
		}
		current_potion_stat_text.text = "|";
		if (stun_timer > 0) {
			current_potion_stat_text.text += " Stunned |";
		}
		for (int i = active_items.Count-1; i >= 0; i--) {
			current_potion_stat_text.text += " "+Item.ITEM_NAME_DICT[active_items[i].type]+" |";
		}
	}

	public void Stunned(){
		stun_timer = 2f;
		UpdatePotionStatText ();
	}
	
}