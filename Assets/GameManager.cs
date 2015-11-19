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
	public Map m = new Map (8,3,8);
    public Button button;
	public GameObject [] cameras;
    public bool beforeGame = true;
    public bool duringGame = false;
    public bool afterGame = false;
	public float interval = 1f;
	public float current_interval = 0f;
	public int total_flips = 4;
	public int current_flips = 0;
	public float flip_tile_interval = 0.05f;
	public static Dictionary<int, int> gridValueMap = new Dictionary<int, int>
	{
		{ Map.EMPTY, -1 },
		{ Map.GO_LAVA, 3 },
		{ Map.GO_ROCK, -1 },
		{ Map.GO_DEST, 1 },
		{ Map.GO_SAFE, -1 },
		{ Map.GO_LADDER, 2 },
		{ Map.GO_OBSTACLE, -1 },
		{ Map.GO_MUD, 4 },
		{ Map.GO_PATH, 1 },
	};

	public static Dictionary<int, int> dragonGridValueMap = new Dictionary<int, int>
	{
		{ Map.EMPTY, -1 },
		{ Map.GO_LAVA, 256 },
		{ Map.GO_ROCK, -1 },
		{ Map.GO_DEST, 1 },
		{ Map.GO_SAFE, -1 },
		{ Map.GO_LADDER, 2 },
		{ Map.GO_OBSTACLE, -1 },
		{ Map.GO_MUD, 4 },
		{ Map.GO_PATH, 1 },
	};
	// Use this for initialization
	void Start () {
		LoadScene (m);
	}

	public static void setMark(Vector3 r){
		GameObject go = Instantiate(Resources.Load("book") as GameObject);
		go.transform.position = r;
	}

	void LoadScene(Map m){
		// Load background
		GameObject [] gos = GameObject.FindGameObjectsWithTag ("GO_LAVA");
		for (int i = 0; i<gos.GetLength(0); i++) {
			//Debug.Log ("LAVA Found");
			m.registerBackgroundGameObject (gos [i]);
		}
		gos = GameObject.FindGameObjectsWithTag ("GO_ROCK");
		for (int i = 0; i<gos.GetLength(0); i++) {
			m.registerBackgroundGameObject (gos [i]);
		}
		/*
		gos = GameObject.FindGameObjectsWithTag ("GO_WATER");
		for (int i = 0; i<gos.GetLength(0); i++) {
			m.registerBackgroundGameObject (gos [i]);
		}
		gos = GameObject.FindGameObjectsWithTag ("GO_PATH");
		for (int i = 0; i<gos.GetLength(0); i++) {
			m.registerBackgroundGameObject (gos [i]);
		}*/


		//Load objects
		/*
		gos = GameObject.FindGameObjectsWithTag ("GO_OBSTACLE");
		for (int i = 0; i<gos.GetLength(0); i++) {
			m.registerBackgroundGameObject (gos [i]);
		}
		gos = GameObject.FindGameObjectsWithTag ("GO_TREE");
		for (int i = 0; i<gos.GetLength(0); i++) {
			m.registerBackgroundGameObject (gos [i]);
		}
		gos = GameObject.FindGameObjectsWithTag ("GO_LADDER");
		for (int i = 0; i<gos.GetLength(0); i++) {
			m.registerBackgroundGameObject (gos [i]);
		}*/
	}

	public void SwitchChasing(){
		//Debug.Log ("COOOOL");
		if (chasing_on) {
			chasing_on = false;
			GameObject.Find ("ChasingSwitch").GetComponent<Text>().text = "Chasing Off";
		} else {
			chasing_on = true;
			GameObject.Find ("ChasingSwitch").GetComponent<Text>().text = "Chasing On";
		}

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

	private void flipTile(int row,int ver,int col,int new_tile_type,float interval){
		GameObject old_go = m.unRegisterBackgroundObject (row, ver, col);
		float below_y = (float)(Map.getYCoordinate (ver) - 0.5 * Map.TILE_SIZE);
		if(old_go != null) old_go.GetComponent<TileControl> ().destroy (interval,below_y);
		GameObject new_go = Instantiate(Resources.Load(Map.TYPE_RESOURCE_DICT[new_tile_type]) as GameObject);
		new_go.transform.position = new Vector3(Map.getXCoordinate(row),below_y,Map.getZCoordinate(col));
		new_go.GetComponent<TileControl> ().born (interval,Map.getYCoordinate (ver),m);
		m.registerBackgroundGameObject (new_go);
	}

	public void flipMap(int center_row,int center_col){
		for (int row=0; row<m.getNRows(); row++) {
			for (int ver=0; ver<m.getNVers(); ver++) {
				for (int col=0; col<m.getNCols(); col++) {
					if(m.g[row,ver,col] == Map.GO_ROCK){
						flipTile(row,ver,col,Map.GO_LAVA,(Mathf.Abs (row-center_row)+Mathf.Abs(col-center_col))*flip_tile_interval);
					}else if(m.g[row,ver,col] == Map.GO_LAVA){
						flipTile(row,ver,col,Map.GO_ROCK,(Mathf.Abs (row-center_row)+Mathf.Abs(col-center_col))*flip_tile_interval);
					}

				}
			}
		}
		current_flips += 1;
	}

    public void ChangeGame()
    {
        if (beforeGame)
        {
            //GameObject.Find("Warrior").transform.position = new Vector3(2, 4, 2);
            beforeGame = false;
            duringGame = true;
            afterGame = false;
            Debug.Log(duringGame);
            button.GetComponentInChildren<Text>().text = "Remaining Jumps:";
            Debug.Log(duringGame);
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
		m.syncCoordinates ();
		current_interval += Time.deltaTime;
		if (current_interval > interval) {
			current_interval = 0f;
			//flipMap(1,1);
		}
	}
}
