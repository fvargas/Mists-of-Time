using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map {
	private int[][,,] level;
	private GameObject [,,] grid_links;
	public static float TILE_SIZE = 1f;
	private readonly string DEFAULT_FLOOR_TILE = "InterBox";
	/**
	 * List of game objects on this map
	 */
	public LinkedList<GameObject> game_objects;

	/**
	 * Types of Game Objects.
	 * Each int value corresponds to a type of object.
	 */
	public static int EMPTY = 0;
	public static int GO_LAVA = 1;
	public static int GO_ROCK = 2;
	public static int GO_DEST = 3;
	public static int GO_SAFE = 4;
	public static int GO_LADDER = 5;
	public static int GO_OBSTACLE = 6;
	public static int GO_MUD = 7;
	public static int GO_PATH = 8;

	public static readonly Dictionary<string, int> TAG_TYPE_DICT = new Dictionary<string, int>
	{
		{ "GO_LAVA", GO_LAVA },
		{"GO_ROCK", GO_ROCK },
		{"GO_DEST", GO_DEST },
		{"GO_SAFE", GO_SAFE },
		{"GO_LADDER", GO_LADDER },
		{"GO_OBSTACLE", GO_OBSTACLE },
		{"GO_MUD", GO_MUD },
		{"GO_PATH", GO_PATH },
	};

	public static readonly Dictionary<int, string> TYPE_RESOURCE_DICT = new Dictionary<int, string>
	{
		{ GO_LAVA,"LavaBox" },
		{ GO_ROCK,"InterBox" },
		{ GO_DEST,"SafeBox" },
	};

	public Map() {
		level = Level1.getLevel ();
		grid_links = new GameObject[level[0].GetLength (0), level[0].GetLength (1), level[0].GetLength (2)];
		game_objects = new LinkedList<GameObject> ();
	}

	public int[][,,] getLevel() {
		return level;
	}

	public int getGridValue(int w, int x, int y, int z) {
		return level [w][y, x, z];
	}

	public static string getTagFromType(int type){
		return TYPE_RESOURCE_DICT[type];
	}

	public void render() {
		// Render the default floor
		for (int x = 0; x < level[0].GetLength(1); x++) {
			for (int z = 0; z < level[0].GetLength(2); z++) {
				var new_go = renderGameObject(new Vector3 (x, -1, z), DEFAULT_FLOOR_TILE);
				//grid_links[0,x,z] = new_go;
			}
		}

		for (int y = 0; y < level[0].GetLength(0); y++) {
			for (int x = 0; x < level[0].GetLength(1); x++) {
				for (int z = 0; z < level[0].GetLength(2); z++) {
					string resource = Level1.tileMapping[level[0][y, x, z]];
					if (resource != "EMPTY") {
						var new_go = renderGameObject(new Vector3(x, y, z), resource);
						grid_links[y,x,z] = new_go;
					}
				}
			}
		}
	}

	public static GameObject renderGameObject(Vector3 loc, string resource) {
		GameObject obj = (GameObject) Object.Instantiate(Resources.Load(resource) as GameObject, loc, Quaternion.identity);
		if (resource == "Dragon") {
			obj.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		}
		return obj;
	}

	public int getNRows() {
		return level[0].GetLength(1);
	}

	public int getNVers() {
		return level[0].GetLength(0);
	}

	public int getNCols() {
		return level[0].GetLength(2);
	}

	public int getNLevels() {
		return level.Length;
	}

	public int getHorizontalSize(){
		return getNRows () * getNCols ();
	}

	/*public bool IsOnMap(int row,int ver, int col){
		return row >= 0 && row < getNRows () && ver >= 0 && ver < getNVers () && col >= 0 && col < getNCols () && grid [row, ver, col] != EMPTY;
	}*/

	/*public bool Reachable(int row,int ver, int col){
		return IsOnMap(row,ver,col) && grid[row,ver,col] != -1;
	}*/

	/*public Vector3 RandomPosition(){
		int rand_row = Random.Range(0,getNRows());
		int rand_ver = Random.Range(0,getNVers());
		int rand_col = Random.Range(0,getNCols());
		while(!Reachable(rand_row,rand_ver,rand_col)){
			rand_row = Random.Range(0,getNRows());
			rand_ver = Random.Range(0,getNVers());
			rand_col = Random.Range(0,getNCols());
		}
		Vector3 v = Map.getCoordinates (new Vector3 (rand_row, rand_ver, rand_col));
		return v;
	}*/

	public GameObject registerGridGameObject(GameObject go){
		//int go_type = TAG_TYPE_DICT[go.tag];
		int row = Mathf.RoundToInt(go.transform.position.x / TILE_SIZE);
		int ver = Mathf.RoundToInt(go.transform.position.y / TILE_SIZE);
		int col = Mathf.RoundToInt(go.transform.position.z / TILE_SIZE);
		
		if(row >=0 && ver >= 0 && col >= 0 && row < getNRows() && col < getNCols() && ver < getNVers()){
			grid_links[ver,row,col] = go;
		}
		return go;
	}

	public GameObject unRegisterGridGameObject(int ver,int row,int col){
		GameObject go = grid_links [ver, row, col];
		grid_links [ver, row, col] = null;
		return go;
	}

	public void registerGameObject(GameObject go){
		game_objects.AddLast (go);
	}

	public static int getRowNumber(float x) {
		return (int)Mathf.Round(x / TILE_SIZE);
	}

	public static int getVerNumber(float y) {
		return (int)Mathf.Round(y / TILE_SIZE);
	}

	public static int getColNumber(float z) {
		return (int)Mathf.Round(z / TILE_SIZE);
	}

	public static float getXCoordinate(int row){
		return row * TILE_SIZE;
	}

	public static float getYCoordinate(int ver){
		return ver * TILE_SIZE;
	}

	public static float getZCoordinate(int col){
		return col * TILE_SIZE;
	}

	/*public int getTileTypeAtCoordinates(Vector3 v){
		return grid[Map.getColNumber(v.x),Map.getVerNumber(v.y),Map.getColNumber(v.z)];
	}*/

	public static Vector4 getCoordinates(Vector4 v){
		return new Vector4 (getXCoordinate((int)v.x),getYCoordinate((int)v.y),getZCoordinate((int)v.z), v.w);
	}

	public static bool areFlippableTiles(int src,int dst){
		return (src == GO_ROCK && dst==GO_LAVA);
	}

	/**
	 * Call this method before you access the grid to make sure its up-to-date
	 * NOTICE: In order to sync game objects on the stage to Map instance, we need to tag the game objects to look up their type.
	 */
	/*public void syncCoordinates(){
		for (int i=0; i<g.GetLength(0); i++) {
			for (int j=0; j<g.GetLength(1); j++) {
				for (int k=0; k<g.GetLength(2); k++) {
					g [i, j, k] = background_g [i, j, k];
					g_links[i,j,k] = background_links[i,j,k];
				}
			}
		}
		for (LinkedListNode<GameObject> go_node = game_objects.First; game_objects.Last!=null && go_node != game_objects.Last.Next; go_node = go_node.Next) {
			GameObject go = go_node.Value;
			fillGrid(go,g,g_links);
		}
	}*/
}
