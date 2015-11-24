using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{

    private GameObject target_go;
    public string behav;
    private float elapsed_time = 0.0f;
    private float wander_interval = 8f;
    private Vector3 wander_dest = new Vector3();
    public Quaternion target_rotation = Quaternion.AngleAxis(0, Vector3.left);
    public float max_speed = 4f;
	public float maxJumpSpeed = 2.0F;
	public float gravity = 40.0F;
    public float current_speed = 0.0f;
    public float follow_approach_time = 1f;
    private float current_acceleration;
	private GameManager game_manager;
    private GroupManager gm;
    private Map m;
    public string enemyTag;
    public bool fleeing = false;
	public int current_state;

    public List<Vector4> target_path;
	private Vector4 prev_target_loc;
	public float interval;
	private float current_interval = 0f;

	public List<Vector3> pace_points;
	public int pace_index = 0;

	private Animator anim;
	private float prev_anim_speed;
	CharacterController controller;
	public Vector3 moveDirection = Vector3.zero;
	public float prev_y_velocity = 0f;
	private int next_row;
	private int next_col;

	static int cute1State = Animator.StringToHash("Base Layer.Cute1");

	public static int UP = 1;
	public static int DOWN = 2;
	public static int LEFT = 3;
	public static int RIGHT = 4;

	public float time_travel_timer;

	SoundManager sm;
    // Use this for initialization
    void Start()
    {
		current_state = 0;
        current_acceleration = max_speed;

		current_interval = 0f;
        if (behav == "")
        {
            behav = "Wander";
        }

		game_manager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		game_manager.registerCharacter (gameObject, current_state);
		gm = game_manager.gm;
		m = game_manager.m;
		Debug.Log (m);
        wander_dest.x = transform.position.x;
        wander_dest.y = transform.position.y;
        wander_dest.z = transform.position.z;

		target_go = GameObject.Find ("player");
		Vector3 tpos = target_go.transform.position;
		prev_target_loc = new Vector4 (tpos.x, tpos.y, tpos.z, game_manager.getPlayerState());

		if (behav == "Chase") {
			Vector3 pos = this.transform.position;
			target_path = PathPlanning.Plan (new Vector4(pos.x, pos.y, pos.z, current_state), prev_target_loc,m);
		}
		anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		prev_anim_speed = anim.speed;
		next_row = m.getRowNumber(this.transform.position.x);
		next_col = m.getColNumber(this.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
		if (game_manager.freezing()) {
			//AnimatorStateInfo current_state = anim.GetCurrentAnimatorStateInfo(0);
			if(anim.speed > 0){
				prev_anim_speed = anim.speed;
				anim.speed = 0;
			}
			return;
		}

		/* Time Travel */
		if (time_travel_timer > 0) {
			time_travel_timer -= Time.deltaTime;
			if (time_travel_timer <= 0){
				time_travel_timer = 0f;
				
			}
			return;
		}


		/* Animation */

		if (anim.speed == 0) {
			anim.speed = prev_anim_speed;
		}

		/* Movement */

		var targetDirection = target_go.transform.position - this.transform.position;
		if (fleeing && (targetDirection.magnitude < 30.0f)) {
			behav = "Flee";
		} else if (fleeing && (targetDirection.magnitude > 50.0f)) {
			behav = "Wander";
		}

		if (behav == "ReachGoal") {
			ReachGoal (target_go.transform.position);
			//this.transform.position = Flee(target.transform.position);

		} else if (behav == "Wander") {
			/*
			elapsed_time += Time.deltaTime;
			if (elapsed_time > wander_interval) {
				wander_dest = m.RandomPosition ();
				Vector3 pos = this.transform.position;
				target_path = PathPlanning.Plan (new Vector4(pos.x, pos.y, pos.z, current_state), wander_dest);
				//transform.rotation *= Quaternion.Euler(0, Random.Range(-2,2), 0);
				elapsed_time = 0.0f;
			}
			followPath ();
			*/
		} else if (behav == "FlockingWander") {
			Vector3 expected_pos = gm.getPosition (this.gameObject);
			ReachGoal (expected_pos, true);
		} else if (behav == "Flee") {
			this.transform.position = Flee (target_go.transform.position);

		} else if (behav == "FleeGroup") {
			Vector3 averageDirection = Vector3.zero;
			if (enemyTag == "") {
				enemyTag = "Cube";
			}
			var enemies = GameObject.FindGameObjectsWithTag (enemyTag);
			Vector3 normalizedFlee = Vector3.zero;
			foreach (var enemy in enemies) {
				normalizedFlee = (Flee (enemy.transform.position) - this.transform.position);
				normalizedFlee.Normalize ();
				averageDirection += normalizedFlee;
			}
			var seek_rotation = Quaternion.LookRotation (averageDirection);
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, seek_rotation, Time.deltaTime * 0.02f);
			//Debug.Log (this.transform.forward);
			this.transform.position += -1 * this.transform.forward * (current_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow (Time.deltaTime, 2));
		} else if (behav == "Chase") {
			current_interval += Time.deltaTime;
			Vector3 tpos = target_go.transform.position;
			Vector4 target_loc = new Vector4(tpos.x, tpos.y, tpos.z, game_manager.getPlayerState());
			if ((Vector4.Distance(prev_target_loc, target_loc) > 2.0 && current_interval > interval) ||
			    target_loc.w != prev_target_loc.w) {
				current_interval = 0f;
				prev_target_loc = target_loc;
				Vector3 pos = this.transform.position;
				target_path = PathPlanning.Plan (new Vector4(pos.x, pos.y, pos.z, current_state), prev_target_loc,m);
			}

			followPath ();
		} else if (behav == "Pace") {
			if (pace_points != null && pace_points.Count > 0) {
				Vector3 waypoint = pace_points [pace_index];
				if (ReachGoal (waypoint)) {
					pace_index = (pace_index + 1) % pace_points.Count;
				}
			}
		} else if (behav == "Jump") {
			if(controller.isGrounded && !anim.GetBool("Jump")){
				moveDirection = new Vector3(0, 0, 0);
				this.transform.localPosition = new Vector3(Map.getXCoordinate(next_row),transform.localPosition.y,Map.getZCoordinate(next_col));
			}else{
				Vector3 future_pos = this.transform.position + moveDirection * Time.deltaTime;
				if(future_pos.y > 0.5f){
					this.transform.position = future_pos;
				}

				prev_y_velocity = moveDirection.y;
				moveDirection.y -= gravity * Time.deltaTime;
			}
			if (anim.GetCurrentAnimatorStateInfo(0).nameHash == cute1State && !anim.IsInTransition(0))
			{
				anim.SetBool("Jump", false);
			}

		}
    }

	void followPath() {
		if (!GameManager.chasing_on) {
			return;
		}

		if (target_path != null && target_path.Count > 0) {
			Vector4 waypoint = Map.getCoordinates (target_path [0]);
			if (ReachGoal (waypoint)) {
				target_path.RemoveAt (0);
			}
		}
	}

	public void addPacePoint(Vector3 v) {
		pace_points.Add (v);
	}

	public void displayOnMap(bool f){
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
			r.enabled = f;
		}
		foreach (Collider c in GetComponentsInChildren<Collider>()) {
			c.enabled = f;
		}

	}

	public void doTimeTravel(int state){
		current_state = state;
		this.GetComponent<EffectControl>().showParticleEffect("holy",1);
		if (current_state == game_manager.getPlayerState()) {
			displayOnMap(true);
		} else {
			displayOnMap(false);
		}
		time_travel_timer = 1f;
	}

	public void jump(int direction){
		int row = m.getRowNumber(this.transform.position.x);
		int col = m.getColNumber(this.transform.position.z);
		if (direction == UP)
		{
			this.transform.LookAt(this.transform.position + new Vector3(1, 0, 0));
			next_row = row + 1;
		}
		if (direction == LEFT)
		{
			this.transform.LookAt(this.transform.position + new Vector3(0, 0, 1));
			next_col = col + 1;
		}
		if (direction == DOWN)
		{
			this.transform.LookAt(this.transform.position + new Vector3(-1, 0, 0));
			next_row = row - 1;
		}
		if (direction == RIGHT)
		{
			this.transform.LookAt(this.transform.position + new Vector3(0, 0, -1));
			next_col = col - 1;
		}

		anim.SetBool("Jump",true);
		moveDirection = new Vector3(0, 0, 1);
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= 2f;
		moveDirection.y = maxJumpSpeed;
	}

    bool ReachGoal(Vector4 dest, bool following = false)
    {
		if (dest.w != current_state) {
			current_state = (int)dest.w;
			game_manager.timeTravel (gameObject, (int)dest.w);
			return true;
		} else {
			current_acceleration = 2.7f;
	        
			var obstacles = hasObstacles ();
			//*** Collision avoidance is disabled until it works reasonably
			if (false && obstacles != 0) {
				adjustDirection (this.transform.rotation, obstacles);
				this.transform.position -= this.transform.right * (max_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow (Time.deltaTime, 2));
			} else {
				var direction = (Vector3)dest - this.transform.position;
				this.transform.LookAt ((this.transform.position + direction));
				this.transform.Rotate (new Vector3 (0, 90, 0));
				direction.Normalize ();
				this.transform.position += direction * (max_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow (Time.deltaTime, 2));
			}

			var bearing = (Vector3)dest - this.transform.position;
			return bearing.magnitude < 1.0f;
		}
    }

	/**
	 * 1 for left, 0 for none, -1 for right
	 */
	int hasObstacles()
	{
		var maxDistance = 3f * this.transform.localScale.z;
		var xDistance = 0.25f * this.transform.localScale.x;
		var yDistance = 0.15f * this.transform.localScale.y;
		Vector3 leftRay = this.transform.position + this.transform.up * yDistance + this.transform.forward * xDistance;
		Vector3 rightRay = this.transform.position + this.transform.up * yDistance - this.transform.forward * xDistance;
		
		Debug.DrawRay(leftRay, -this.transform.right * maxDistance, Color.white);
		Debug.DrawRay(rightRay, -this.transform.right * maxDistance, Color.green);
		
		//Debug.Log (leftRay.y+"Y");
		bool hit = false;
		if (Physics.Linecast(leftRay, leftRay - this.transform.right * maxDistance))
		{
			return 1;
			//Debug.Log("LEFT");
		} else if (Physics.Linecast(rightRay, rightRay - this.transform.right * maxDistance))
		{
			return -1;
		}
		return 0;
	}
	
	void adjustDirection(Quaternion origRotation,int direction){
		if (direction == 1) {
			this.transform.rotation = Quaternion.Slerp (origRotation, Quaternion.AngleAxis (65, Vector3.up) * origRotation, 0.05f);
		} else if (direction == -1) {
			this.transform.rotation = Quaternion.Slerp(origRotation, Quaternion.AngleAxis(65, Vector3.down) * origRotation, 0.05f);
		}
	}

    Vector3 Flee(Vector3 target)
    {
        var bearing = this.transform.position - target;
        //Debug.Log(bearing.magnitude);
        //Acceleration threshold.
        var oldRotation = this.transform.rotation;
        current_acceleration = 2.7f;
        //var targetDirection = target - this.transform.position;
        //targetDirection.y = this.target.transform.position.y;
        var targetPosition = target;
        targetPosition.y = this.transform.position.y;
        //var seek_rotation = Quaternion.LookRotation(targetDirection);
        this.transform.LookAt(targetPosition);
        //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, seek_rotation, .05f);



        current_speed += current_acceleration * Time.deltaTime;
        //Debug.Log(current_speed);
        if (current_speed > max_speed)
        {
            current_speed = max_speed;
        }
        else if (current_speed < 0)
        {
            current_speed = 0f;
        }
        //Debug.Log(current_speed);
        var new_position = this.transform.position - this.transform.forward * (current_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow(Time.deltaTime, 2));
        var seek_rotation = Quaternion.LookRotation(this.transform.position - new_position);
        //Debug.Log(new_position);
        float angle = Quaternion.Angle(transform.rotation, seek_rotation);
        float remainingTime = angle / 0.1f;
        float rotationPercentage = Mathf.Min(1.0f, Time.deltaTime / remainingTime);
        //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, seek_rotation, rotationPercentage);
        this.transform.rotation = oldRotation;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, seek_rotation, 0.02f);
        //new_position = this.transform.position - this.transform.forward * (current_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow(Time.deltaTime, 2));
        return new_position;
    }

	private void DrawPath()
	{
		/*List<Vector3> res = target_path;
        foreach (var r in res)
        {
            //Debug.Log(r.x + ", " + r.y + ", " + r.z);
            GameObject go = Instantiate(Resources.Load("book") as GameObject);
            go.transform.position = new Vector3(Map.getXCoordinate((int)r.x), Map.getYCoordinate((int)r.y), Map.getZCoordinate((int)r.z));
        }
		Debug.Log ("Just Planned");*/
	}
}
