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
    public float current_speed = 0.0f;
    public float follow_approach_time = 1f;
    private float current_acceleration;
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

    // Use this for initialization
    void Start()
    {
		m = new Map (); // Yes, this is kind of a problem
		current_state = 0;

		target_go = GameObject.Find ("magic_archer");
		Vector3 tpos = target_go.transform.position;
		prev_target_loc = new Vector4 (tpos.x, tpos.y, tpos.z, 0);

        current_acceleration = max_speed;
        	if (behav == "") {
			behav = "Wander";
		}
		current_interval = 0f;
        wander_dest.x = transform.position.x;
        wander_dest.y = transform.position.y;
        wander_dest.z = transform.position.z;
        //Debug.Log(wander_dest);

		gm = GameObject.Find("GameManager").GetComponent<GameManager>().gm;
		//m = GameObject.Find("GameManager").GetComponent<GameManager>().m;

		if (behav == "Chase") {
			Plan (prev_target_loc);
		}
    }

    // Update is called once per frame
    void Update()
    {
        var targetDirection = target_go.transform.position - this.transform.position;

        if (fleeing && (targetDirection.magnitude < 30.0f))
        {
            behav = "Flee";
        }
        else if (fleeing && (targetDirection.magnitude > 50.0f))
        {
            behav = "Wander";
        }

        if (behav == "ReachGoal")
        {
            ReachGoal(target_go.transform.position);
            //this.transform.position = Flee(target.transform.position);

        }
		else if (behav == "Wander") {
			elapsed_time += Time.deltaTime;
			if (elapsed_time > wander_interval) {
				wander_dest = m.RandomPosition();
				Plan (wander_dest);
				//transform.rotation *= Quaternion.Euler(0, Random.Range(-2,2), 0);
				elapsed_time = 0.0f;
			}
			followPath ();
		} 
        else if (behav == "FlockingWander")
        {
            Vector3 expected_pos = gm.getPosition(this.gameObject);
            ReachGoal(expected_pos, true);
        }
        else if (behav == "Flee")
        {
            this.transform.position = Flee(target_go.transform.position);

        }
        else if (behav == "FleeGroup")
        {
            Vector3 averageDirection = Vector3.zero;
            if (enemyTag == "")
            {
                enemyTag = "Cube";
            }
            var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            Vector3 normalizedFlee = Vector3.zero;
            foreach (var enemy in enemies)
            {
                normalizedFlee = (Flee(enemy.transform.position) - this.transform.position);
                normalizedFlee.Normalize();
                averageDirection += normalizedFlee;
            }
            var seek_rotation = Quaternion.LookRotation(averageDirection);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, seek_rotation, Time.deltaTime * 0.02f);
            //Debug.Log (this.transform.forward);
            this.transform.position += -1 * this.transform.forward * (current_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow(Time.deltaTime, 2));
        }
        else if (behav == "Chase")
        {
			current_interval += Time.deltaTime;
			Vector3 tpos = target_go.transform.position;
			Debug.Log (target_go);
			Debug.Log(target_go.GetComponent<Movement>());
			//*** Last argument should really be the current state of the target
			Vector4 target_loc = new Vector4(tpos.x, tpos.y, tpos.z, 0);

			if (Vector4.Distance(prev_target_loc, target_loc) > 2.0 && current_interval > interval) {
				current_interval = 0f;
				prev_target_loc = target_loc;
				Plan (prev_target_loc);
			}

			followPath ();
        }
		else if (behav == "Pace") {
			if(pace_points != null && pace_points.Count > 0){
				Vector3 waypoint = pace_points[pace_index];
				if (ReachGoal (waypoint)) {
					pace_index = (pace_index + 1) % pace_points.Count;
				}
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


    bool ReachGoal(Vector3 destination, bool following = false)
    {
        current_acceleration = 2.7f;
        
        var obstacles = hasObstacles();
        if (obstacles != 0)
        {
            adjustDirection(this.transform.rotation, obstacles);
            this.transform.position -= this.transform.right * (max_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow(Time.deltaTime, 2));
        }
        else
        {
            var direction = destination - this.transform.position;
            this.transform.LookAt((this.transform.position + direction));
            this.transform.Rotate(new Vector3(0,90,0));
            direction.Normalize();
            this.transform.position += direction * (max_speed * Time.deltaTime + 0.5f * current_acceleration * Mathf.Pow(Time.deltaTime, 2));
        }

		var bearing = destination - this.transform.position;
		return bearing.magnitude < 1.0f;
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

	public void Plan (Vector4 target)
	{
		PQ pq = new PQ();
		HashSet<Vector4> visited = new HashSet<Vector4>();
		int target_x = Map.getRowNumber(target.x);
		int target_y = Map.getVerNumber(target.y);
		int target_z = Map.getColNumber(target.z);
		int target_w = (int)target.w;
		int start_x = Map.getRowNumber(this.transform.position.x);
		int start_y = Map.getVerNumber(this.transform.position.y);
		int start_z = Map.getColNumber(this.transform.position.z);
		int start_w = current_state;
		
		float md = Node.euclideanDistance(start_x, start_y, start_z, start_w, target_x, target_y, target_z, target_w);
		Node init = new Node(start_x, start_y, start_z, start_w, 0, md, new List<Vector4>());
		pq.insert(init);

		while (!pq.isEmpty())
		{
			Node n = pq.pop();
			visited.Add(new Vector4(n.x, n.y, n.z, n.w));
			if (n.x == target_x && n.y == target_y && n.z == target_z && n.w == target_w)
			{
				target_path = n.path;
				//DrawPath();
				return;
			}

			bool atLadder = m.getGridValue(n.w, n.x, n.y, n.z) == Map.GO_LADDER;
			
			// Set of actions for 6-way connected grid
			if (isValid(n.x - 1, n.y, n.z, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x - 1, n.y, n.z, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x - 1, n.y, n.z, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z)];
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z)];
				}
				Node newNode = new Node(n.x - 1, n.y, n.z, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
			if (isValid(n.x + 1, n.y, n.z, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x + 1, n.y, n.z, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x + 1, n.y, n.z, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z)];
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z)];
				}
				Node newNode = new Node(n.x + 1, n.y, n.z, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
			if (atLadder && isValid(n.x, n.y - 1, n.z, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x, n.y - 1, n.z, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x, n.y - 1, n.z, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x, n.y - 1, n.z)];
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x, n.y - 1, n.z)];
				}
				Node newNode = new Node(n.x, n.y - 1, n.z, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
			if (atLadder && isValid(n.x, n.y + 1, n.z, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x, n.y + 1, n.z, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x, n.y + 1, n.z, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x, n.y + 1, n.z)];
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x, n.y + 1, n.z)];
				}
				Node newNode = new Node(n.x, n.y + 1, n.z, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
			if (isValid(n.x, n.y, n.z - 1, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x, n.y, n.z - 1, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x, n.y, n.z - 1, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x, n.y, n.z - 1)];
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x, n.y, n.z - 1)];
				}
				Node newNode = new Node(n.x, n.y, n.z - 1, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
			if (isValid(n.x, n.y, n.z + 1, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x, n.y, n.z + 1, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x, n.y, n.z + 1, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x, n.y, n.z + 1)];
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x, n.y, n.z + 1)];
				}
				Node newNode = new Node(n.x, n.y, n.z + 1, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
			
			// Additional set of actions for 10-way connected grid
			if (isValid(n.x - 1, n.y, n.z - 1, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x - 1, n.y, n.z - 1, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x - 1, n.y, n.z - 1, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z - 1)] * 1.4f;
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z - 1)] * 1.4f;
				}
				Node newNode = new Node(n.x - 1, n.y, n.z - 1, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
			if (isValid(n.x - 1, n.y, n.z + 1, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x - 1, n.y, n.z + 1, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x - 1, n.y, n.z + 1, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z + 1)] * 1.4f;
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z + 1)] * 1.4f;
				}
				Node newNode = new Node(n.x - 1, n.y, n.z + 1, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
			if (isValid(n.x + 1, n.y, n.z - 1, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x + 1, n.y, n.z - 1, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x + 1, n.y, n.z - 1, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z - 1)] * 1.4f;
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z - 1)] * 1.4f;
				}
				Node newNode = new Node(n.x + 1, n.y, n.z - 1, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
			if (isValid(n.x + 1, n.y, n.z + 1, n.w, visited))
			{
				float h = Node.euclideanDistance(n.x + 1, n.y, n.z + 1, n.w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				newPath.Add(new Vector4(n.x + 1, n.y, n.z + 1, n.w));
				
				float cost;
				if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z + 1)] * 1.4f;
				} else {
					cost = n.g + GameManager.gridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z + 1)] * 1.4f;
				}
				Node newNode = new Node(n.x + 1, n.y, n.z + 1, n.w, cost, h, newPath);
				pq.insert(newNode);
			}
		}

		//DrawPath();
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
	
	private bool isValid(int x, int y, int z, int w, HashSet<Vector4> visited)
	{
		int numRows = m.getNRows();
		int numCols = m.getNCols();
		int numVers = m.getNVers();
		int numLevels = m.getNLevels ();
		
		bool squareInBounds = 0 <= w && w < numLevels && 0 <= x && x < numRows && 0 <= y && y < numVers && 0 <= z && z < numCols;
		bool haveVisited = visited.Contains(new Vector4(x, y, z, w));
		bool ret = squareInBounds && !haveVisited && GameManager.gridValueMap[m.getGridValue(w, x, y, z)] >= 0;

		return ret;
	}
	
	private class Node
	{
		public float g;
		public float h;
		public float f;
		public List<Vector4> path;
		public int x;
		public int y;
		public int z;
		public int w;
		
		public Node(int x, int y, int z, int w, float g, float h, List<Vector4> path)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
			this.g = g;
			this.h = h;
			this.f = g + h;
			this.path = path;
		}

		public bool equals(Node n) {
			return x == n.x && y == n.y && z == n.z && w == n.w;
		}
		
		public static float euclideanDistance(int start_x, int start_y, int start_z, int start_w,
		                                      int target_x, int target_y, int target_z, int target_w)
		{
			return Vector4.Distance (new Vector4(start_x, start_y, start_z, start_w), new Vector4(target_x, target_y, target_z, target_w));
		}
	}
	
	private class PQ
	{
		public List<Node> queue;
		
		public PQ()
		{
			queue = new List<Node>();
		}
		
		public bool isEmpty()
		{
			return queue.Count <= 0;
		}
		
		public void insert (Node newNode)
		{
			foreach (Node n in queue)
			{
				if (newNode.equals(n))
				{
					if (newNode.f < n.f)
					{
						n.f = newNode.f;
						n.g = newNode.g;
						n.path = newNode.path;
					}
					return;
				}
			}
			
			queue.Add(newNode);
		}
		
		public Node pop()
		{
			float min_f = queue[0].f;
			Node min = queue[0];
			
			foreach (Node n in queue)
			{
				if (n.f < min_f)
				{
					min_f = n.f;
					min = n;
				}
			}
			
			queue.Remove(min);
			return min;
		}
	}
}
