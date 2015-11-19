using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupManager : MonoBehaviour {
	public Dictionary<int,LinkedList<GameObject>> members = new Dictionary<int,LinkedList<GameObject>>();
	public Dictionary<int,GameObject> leaders = new Dictionary<int,GameObject>();
	public Dictionary<GameObject,int> group_ids = new Dictionary<GameObject,int>();
	public Dictionary<GameObject,Vector3> positions = new Dictionary<GameObject,Vector3> ();
	private int next_group_id = 0;
	public string formation;
	public float formation_distance = 20f;

	public int registerLeader(GameObject leader){
		if (group_ids.ContainsKey (leader)) {
			return group_ids [leader];
		}
		leaders.Add(next_group_id, leader);
		group_ids.Add (leader, next_group_id);
		next_group_id += 1;
		return next_group_id - 1;
	}

	public bool registerMember(GameObject go,int group_index){
		if (!leaders.ContainsKey (group_index)) {
			return false;
		}
		if (group_ids.ContainsKey (go)) {
			return false;
		}
		if (!members.ContainsKey (group_index)) {
			members.Add (group_index, new LinkedList<GameObject> ());
		}
		members [group_index].AddLast (go);
		positions.Add (go, go.transform.position);
		group_ids.Add (go, group_index);
		return true;
	}

	public GameObject getLeader(GameObject go){
		if (!group_ids.ContainsKey (go)) {
			return null;
		}
		return leaders [group_ids [go]];
	}

	public bool hasPosition(GameObject go){
		return positions.ContainsKey (go);
	}

	public Vector3 getPosition(GameObject go){
		return positions [go];
	}
	
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		foreach(KeyValuePair<int, GameObject> entry in leaders){
			int group_index = entry.Key;
			GameObject leader = entry.Value;
			LinkedList<GameObject> current_members = members[group_index];
			Vector3 future_position = leader.transform.position + (-1 * leader.transform.forward * Time.deltaTime * leader.GetComponent<Movement>().current_speed);
			int rank = 1;
			for (LinkedListNode<GameObject> member_node = current_members.First; member_node != current_members.Last.Next; member_node = member_node.Next){
				GameObject member = member_node.Value;
				if(rank % 2 == 1){
					//Odd members to the left
					if(formation == "Line"){
						positions[member] = future_position + leader.transform.rotation * Quaternion.Euler(0, -90, 0) * Vector3.back * (formation_distance * (rank/2+1));
					}else if(formation == "V"){
						positions[member] = future_position + leader.transform.rotation * Quaternion.Euler(0, -150, 0) * Vector3.back * (formation_distance * (rank/2+1));
					}else if(formation == "Circle"){
						positions[member] = future_position + leader.transform.rotation * Quaternion.Euler(0, -36*(rank/2+1), 0) * Vector3.back * formation_distance;
					}
				}else{
					//Even members to the right
					if(formation == "Line"){
						positions[member] = future_position + leader.transform.rotation * Quaternion.Euler(0, 90, 0) * Vector3.back * (formation_distance * (rank/2));
					}else if(formation == "V"){
						positions[member] = future_position + leader.transform.rotation * Quaternion.Euler(0, 150, 0) * Vector3.back * (formation_distance * (rank/2));
					}else if(formation == "Circle"){
						positions[member] = future_position + leader.transform.rotation * Quaternion.Euler(0, 36*(rank/2), 0) * Vector3.back * formation_distance;
					}
				}
				rank += 1;
			}
		}
	}

}
