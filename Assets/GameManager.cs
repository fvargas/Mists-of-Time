using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
	public GroupManager gm;
	public GameObject [] prefabs;
	public static bool chasing_on = true;

	public static int ISOMETRIC_VIEW = 1;
	public static int THIRD_PERSON_VIEW = 2;
	public static int camera_view = ISOMETRIC_VIEW;//1 - Isometric; 2 - Third Person
	public Map m;
    public Button button;
	public GameObject [] cameras;
    public bool beforeGame = true;
    public bool duringGame = false;
    public bool afterGame = false;
	public float interval = 10f;
	public float current_interval = 0f;
	public int total_flips = 4;
	public int current_flips = 0;
	public float flip_map_interval = 0.5f;
	private Text status_txt;
	public float freeze_timer = 0f;
	private int player_state = 0;
	
	private Hashtable gos = new Hashtable();

	public static Dictionary<int, int> gridValueMap = new Dictionary<int, int>
	{
		{ Map.EMPTY, 1 },
		{ Map.GO_LAVA, -1 },
		{ Map.GO_ROCK, 1 },
		{ Map.GO_DEST, 1 },
		{ Map.GO_SAFE, -1 },
		{ Map.GO_LADDER, 2 },
		{ Map.GO_OBSTACLE, -1 },
		{ Map.GO_MUD, 4 },
		{ Map.GO_PATH, 1 },
	};

	public static Dictionary<int, int> dragonGridValueMap = new Dictionary<int, int>
	{
		{ Map.EMPTY, 1 },
		{ Map.GO_LAVA, -1 },
		{ Map.GO_ROCK, 1 },
		{ Map.GO_DEST, 1 },
		{ Map.GO_SAFE, -1 },
		{ Map.GO_LADDER, 2 },
		{ Map.GO_OBSTACLE, -1 },
		{ Map.GO_MUD, 256 },
		{ Map.GO_PATH, 1 },
	};
	// Use this for initialization
	void Start () {
		//Debug.Log (GameObject.Find ("magic_archer"));
		//Debug.Log (player_movement);
		/*Component [] comps = GameObject.Find ("mon05 (2)").GetComponents(typeof(Component));
		for (int i = 0; i<comps.GetLength(0); i++) {
			Debug.Log (comps[i]);
		}
		*/

		m = new Map ();
		m.render ();

		//status_txt = GameObject.Find ("Status_Text").GetComponent<Text> ();
		//status_txt.text = "HP:3";
	}

	public void registerCharacter(GameObject obj, int state) {
		gos.Add (obj, state);
	}

	public void timeTravel(GameObject obj, int state) {
		gos [obj] = state;

		foreach (DictionaryEntry de in gos) {
			Renderer r = ((GameObject)de.Key).GetComponentsInChildren<Renderer>()[0];
			if ((int)de.Value == player_state) {
				r.enabled = true;
			} else {
				r.enabled = false;
			}
		}
	}

	public int getPlayerState(){
		return player_state;
	}

	public static void setMark(Vector3 r) {
		GameObject go = Instantiate(Resources.Load("book") as GameObject);
		go.transform.position = r;
	}


	public void freeze(float time_in_sec){
		freeze_timer = time_in_sec;
	}

	public bool freezing(){
		return freeze_timer > 0;
	}

	public void SwitchCamera(){
		if (camera_view == ISOMETRIC_VIEW) {
			camera_view = THIRD_PERSON_VIEW;
			GameObject.Find ("CameraSwitch").GetComponent<Text>().text = "Third Person";
			GameObject.Find("Warrior").GetComponent<PlayerControl>().movement = "Move";
			cameras[0].SetActive(true);
			cameras[1].SetActive(false);
		} else if(camera_view == THIRD_PERSON_VIEW){
			camera_view = ISOMETRIC_VIEW;
			GameObject.Find ("CameraSwitch").GetComponent<Text>().text = "Isometric";
			GameObject.Find("Warrior").GetComponent<PlayerControl>().movement = "Iso";
			cameras[0].SetActive(false);
			cameras[1].SetActive(true);
		}

	}

	private void flipTile(int ver,int row,int col,int new_tile_type,float interval){
		GameObject old_go = m.unRegisterGridGameObject (ver, row, col);
		float below_y = (float)(Map.getYCoordinate (ver) - 0.5 * Map.TILE_SIZE);
		if(old_go != null) old_go.GetComponent<TileControl> ().destroy (interval,below_y);
		if (new_tile_type != Map.EMPTY) {
			if(Level1.tileMapping [new_tile_type].Equals("EMPTY")) return;
			//Debug.Log (new_tile_type);
			//Debug.Log (Level1.tileMapping [new_tile_type]);
			GameObject new_go = Instantiate (Resources.Load (Level1.tileMapping [new_tile_type]) as GameObject);
			new_go.transform.position = new Vector3 (Map.getXCoordinate (row), below_y, Map.getZCoordinate (col));
			new_go.GetComponent<TileControl> ().born (interval, Map.getYCoordinate (ver), m);
			m.registerGridGameObject (new_go);
		}
	}

	public bool switchState(int center_row,int center_col) {
		if (center_row < 0 || center_row >= m.getNRows () || center_col < 0 || center_col >= m.getNCols ()) {
			return false;
		}

		int next_player_state = (player_state + 1) % m.getNLevels();
		int [,,] current_grid = m.getLevel () [player_state];
		int [,,] next_grid = m.getLevel () [next_player_state];
		int new_tile_type = next_grid [0, center_row, center_col];
		if (new_tile_type == 1) {
			//TODO: replace hardcoded 0 and 1
			var warning_tile = Map.renderGameObject (new Vector3 (Map.getXCoordinate (center_row), Map.getYCoordinate (0), Map.getZCoordinate (center_col)),Level1.tileMapping [new_tile_type]);
			//TODO: replace hardcoded 0
			Destroy (warning_tile.GetComponent<Rigidbody>());
			Destroy (warning_tile.GetComponent<BoxCollider>());
			warning_tile.GetComponent<TileControl> ().destroy (0.5f, -0.5f);
			return false;
		} else {
			float flip_tile_interval = flip_map_interval / (m.getNRows () + m.getNCols ());
			for (int row=0; row<m.getNRows(); row++) {
				for (int ver=0; ver<m.getNVers(); ver++) {
					for (int col=0; col<m.getNCols(); col++) {
						//Debug.Log (row+" "+ver+" "+col);
						if (current_grid [ver, row, col] != next_grid [ver, row, col]) {
							flipTile (ver, row, col, next_grid [ver, row, col], (Mathf.Abs (row - center_row) + Mathf.Abs (col - center_col)) * flip_tile_interval);
						}
					}
				}
			}
			current_flips += 1;
			player_state = next_player_state;
			return true;
		}
	}

    public void ChangeGame()
    {
        if (beforeGame)
        {
            //GameObject.Find("Warrior").transform.position = new Vector3(2, 4, 2);
            beforeGame = false;
            duringGame = true;
            afterGame = false;
            //Debug.Log(duringGame);
            button.GetComponentInChildren<Text>().text = "Remaining Jumps:";
            //Debug.Log(duringGame);
        }
        if (afterGame)
        {
            GameObject.Find("Warrior").GetComponent<PlayerControl>().respawn();
            beforeGame = false;
            duringGame = true;
            afterGame = false;
            button.GetComponentInChildren<Text>().text = "Remaining Jumps:";
        }
    }

    public void EndGame()
    {
        beforeGame = false;
        duringGame = false;
        afterGame = true;
        button.GetComponentInChildren<Text>().text = "Restart";
    }


	public bool hasFlip(){
		//Debug.Log (total_flips);
		return current_flips < total_flips;
	}

	// Update is called once per frame
	void Update () {
		//m.syncCoordinates (); 

		if(freeze_timer >0) freeze_timer -= Time.deltaTime;
		if(freeze_timer<0) freeze_timer = 0f;
		current_interval += Time.deltaTime;
		if (current_interval > interval) {
			current_interval = 0f;
			//switchState(1,1);
		}
	}
}
