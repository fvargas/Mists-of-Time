using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Item : MonoBehaviour {
	public static int ITEM_EMPTY = 0;
	public static int ITEM_FREEZE = 1;
	public static int ITEM_SPEEDUP = 2;
	public static int ITEM_SHIELD = 3;

	public static readonly Dictionary<int, string> ITEM_NAME_DICT = new Dictionary<int, string>
	{
		{ ITEM_EMPTY, "Empty" },
		{ ITEM_FREEZE, "Freeze" },
		{ ITEM_SPEEDUP, "Speed Up" },
		{ ITEM_SHIELD, "Shield" },
	};

	public static readonly Dictionary<int, float> ITEM_COOLDOWN_DICT = new Dictionary<int, float>
	{
		{ ITEM_EMPTY, 0f },
		{ ITEM_FREEZE, 2f },
		{ ITEM_SPEEDUP, 1f },
		{ ITEM_SHIELD, 1f },
	};

	public static readonly Dictionary<int, float> ITEM_DURATION_DICT = new Dictionary<int, float>
	{
		{ ITEM_EMPTY, 0f },
		{ ITEM_FREEZE, 2f },
		{ ITEM_SPEEDUP, 4f },
		{ ITEM_SHIELD, 4f },
	};

	private GameManager gm;
	public int type;
	public float pickup_distance;
	// Use this for initialization
	void Start () {
		gm = GameObject.Find ("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player_go = gm.GetPlayer ();
		if (player_go == null)
			return;
		float dis = (player_go.transform.position - transform.position).sqrMagnitude;
		if (dis < pickup_distance) {
			PickedUp();
		}
	}

	public void PickedUp(){
		GameObject player_go = gm.GetPlayer ();
		if (player_go == null)
			return;
		PlayerControl pc = player_go.GetComponent<PlayerControl> ();
		int avail_slot = pc.GetNextAvailableItemSlot ();
		Debug.Log (avail_slot);
		if (avail_slot == -1)
			return;
		pc.PlaceItem (avail_slot, type);
		Destroy (this.gameObject);
	}

}

public class ActiveItem : Object {
	public int type;
	public float time_left;

	public ActiveItem(int t){
		type = t;
		time_left = Item.ITEM_DURATION_DICT [type];
	}
}
