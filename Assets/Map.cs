using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map {
	public int[,,] background_g;
	public GameObject[,,] background_links;
	public int [,,] g;
	public GameObject[,,] g_links;
	public static float TILE_SIZE = 1f;

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
	/**
	 * Dictionary mapping Tag to GameObject type
	 */
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
	
	public int [,,] getMatrix(){
		syncCoordinates ();
		return g;
	}

	public static string getTagFromType(int type){
		return TYPE_RESOURCE_DICT[type];
	}

	public Map() {
		//background_g = new int[nrows,ncols,nvers];
		//g = new int[nrows,ncols,nvers];
		//background_links = new GameObject[nrows,ncols,nvers];
		//g_links = new GameObject[nrows,ncols,nvers];
		game_objects = new LinkedList<GameObject> ();
		/*for (int i=0; i<background_g.GetLength(0); i++) {
			for (int j=0; j<background_g.GetLength(1); j++) {
				for (int k=0; k<background_g.GetLength(2); k++) {
					background_g[i,j,k] = EMPTY;
					g[i,j,k] = EMPTY;
					background_links[i,j,k] = null;
					g_links[i,j,k] = null;
				}
			}
		}*/
	}

	public void render() {
		int[][,,] level1 = Level1.getLevel ();
		int[,,] state1 = level1 [0];
		Debug.Log (state1.GetLength(0) + "   " + state1.GetLength(1) + "   " + state1.GetLength(2));

		for (int y = 0; y < state1.GetLength(0); y++) {
			for (int z = 0; z < state1.GetLength(1); z++) {
				for (int x = 0; x < state1.GetLength(2); x++) {
					if (y == 0) {
						renderGameObject(new Vector3(x, y, z), "InterBox");
					}
					string resource = Level1.tileMapping[state1[y, z, x]];
					if (resource != "EMPTY") {
						renderGameObject(new Vector3(x, y + 0.5f, z), resource);
					}
				}
			}
		}
	}

	private void renderGameObject(Vector3 loc, string resource) {
		Object.Instantiate(Resources.Load(resource) as GameObject, loc, Quaternion.identity);
	}

	public int getNRows(){
		return g.GetLength(0);
	}

	public int getNVers(){
		return g.GetLength(1);
	}

	public int getNCols(){
		return g.GetLength(2);
	}

	public int getHorizontalSize(){
		return getNRows () * getNCols ();
	}

	public bool IsOnMap(int row,int ver, int col){
		return row >= 0 && row < getNRows () && ver >= 0 && ver < getNVers () && col >= 0 && col < getNCols () && g [row, ver, col] != EMPTY;
	}

	public bool Reachable(int row,int ver, int col){
		return IsOnMap(row,ver,col) && g[row,ver,col] != -1;
	}

	public Vector3 RandomPosition(){
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
	}

	public void fillGrid(GameObject go, int row,int ver,int col){
		
	}

	private void fillGrid(GameObject go,int [,,] grid, GameObject [,,] link){
		int go_type = TAG_TYPE_DICT[go.tag];
		int row = (int)(go.transform.position.x / TILE_SIZE);
		int ver = (int)(go.transform.position.y / TILE_SIZE);
		int col = (int)(go.transform.position.z / TILE_SIZE);

		if(row >=0 && ver >= 0 && col >= 0 && row < getNRows() && col < getNCols() && ver < getNVers()){
			grid[row,ver,col] = go_type;
			link[row,ver,col] = go;
		}
	}
	public void registerBackgroundGameObject(GameObject go){
		fillGrid (go, background_g,background_links);
	}

	public GameObject unRegisterBackgroundObject(int row,int ver,int col){
		GameObject go = background_links [row, ver, col];
		background_links [row, ver, col] = null;
		return go;
	}

	public void registerGameObject(GameObject go){
		game_objects.AddLast (go);
	}

	public static int getRowNumber(float x){
		return (int)Mathf.Round(x / TILE_SIZE);
	}

	public static int getVerNumber(float y){
		return (int)Mathf.Round(y / TILE_SIZE);
	}

	public static int getColNumber(float z){
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

	public int getTileTypeAtCoordinates(Vector3 v){
		return g[Map.getColNumber(v.x),Map.getVerNumber(v.y),Map.getColNumber(v.z)];
	}

	public static Vector3 getCoordinates(Vector3 v){
		return new Vector3 (getXCoordinate((int)v.x),getYCoordinate((int)v.y),getZCoordinate((int)v.z));
	}

	public static bool areFlippableTiles(int src,int dst){
		return (src == GO_ROCK&&dst==GO_LAVA);
	}

	/**
	 * Call this method before you access the grid to make sure its up-to-date
	 * NOTICE: In order to sync game objects on the stage to Map instance, we need to tag the game objects to look up their type.
	 */
	public void syncCoordinates(){
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
	}
}
