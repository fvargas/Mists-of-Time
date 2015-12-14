using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map {
	private int[][,,] level;
	private int[][,,] influence_map;
	private GameObject [,,] grid_links;
	public static float TILE_SIZE = 1f;
	private readonly string DEFAULT_FLOOR_TILE = "SafeBox";

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

	private static readonly int[,] Matrix2D = {
		{ 1, 7, 15, 7, 1 },
		{ 7, 30, 60, 30, 7 },
		{ 15, 60, 100, 60, 15 },
		{ 7, 30, 60, 30, 7 },
		{ 1, 7, 15, 7, 1 },
	};
	private static readonly int MATRIX_2D_OFFSET = Matrix2D.GetLength(0) / 2;

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
		influence_map = new int[Level1.w][,,];
		for (int i = 0; i < influence_map.Length; i++) {
			influence_map[i] = new int[Level1.y, Level1.x, Level1.z];
		}
		grid_links = new GameObject[level[0].GetLength (0), level[0].GetLength (1), level[0].GetLength (2)];
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

		GameObject obj = (GameObject) UnityEngine.Object.Instantiate(Resources.Load(resource) as GameObject, loc, Quaternion.identity);
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

	public int getNStates() {
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

	public static bool isInInfMap(Vector4 loc, int[][,,] inf_map)
	{
		return (0 <= loc.w && loc.w < inf_map.Length && 0 <= loc.y && loc.y < inf_map[0].GetLength(0) &&
		        0 <= loc.x && loc.x < inf_map[0].GetLength(1) && 0 <= loc.z && loc.z < inf_map[0].GetLength(2));
	}

	public int[][,,] getInfluenceMapCopy()
	{
		int[][,,] copy = new int[influence_map.Length][,,];
		for (int i = 0; i < copy.Length; i++) {
			copy[i] = new int[influence_map[0].GetLength(0), influence_map[0].GetLength(1), influence_map[0].GetLength(2)];
			Array.Copy(influence_map[i], copy[i], influence_map[i].Length);
		}

		return copy;
	}

	public static int getInfluenceMapValue(Vector4 loc, int[][,,] inf_map)
	{
		if (isInInfMap (loc, inf_map)) {
			return inf_map [(int)loc.w] [(int)loc.y, (int)loc.x, (int)loc.z];
		} else {
			return 999; // This is an arbitrarily high value to represent an error
						// since a low value is considered more desirable.
		}
	}

	public void updateInfluenceMap(Vector4 old_loc, Vector4 new_loc, int strength) {
		if (old_loc.w != -1) {
			subtractInfluenceMap (old_loc, strength, influence_map);
		}
		addToInfluenceMap (new_loc, strength, influence_map);
	}

	public static void drawInfluenceMatrix(int[][,,] influence_map)
	{
		Debug.Log ("=============START=============");
		for (int r = 0; r < influence_map[0].GetLength(1); r++) {
			Debug.Log (influence_map[0][0,r,0] + " " + influence_map[0][0,r,1] + " " + influence_map[0][0,r,2] + " " + influence_map[0][0,r,3] + " "
			           + influence_map[0][0,r,4] + " " + influence_map[0][0,r,5] + " " + influence_map[0][0,r,6] + " " + influence_map[0][0,r,7] + " "
			           + influence_map[0][0,r,8] + " " + influence_map[0][0,r,9] + " " + influence_map[0][0,r,10] + " " + influence_map[0][0,r,11] + " "
			           + influence_map[0][0,r,12] + " " + influence_map[0][0,r,13] + " " + influence_map[0][0,r,14] + " " + influence_map[0][0,r,15] + " "
			           + influence_map[0][0,r,16] + " " + influence_map[0][0,r,17] + " " + influence_map[0][0,r,18] + " " + influence_map[0][0,r,19] + "\n");
		}
		Debug.Log ("==============END==============");
	}

	public static void subtractInfluenceMap(Vector4 loc, int weight, int[][,,] inf_map)
	{
		subtractInfluence3D (loc, weight, inf_map, (int)loc.w);

		int numStates = inf_map.Length;
		if (numStates >= 2) {
			int w = ((int)loc.w + 1) % numStates;
			subtractInfluence3D (loc, weight, inf_map, w);
		}

		if (inf_map.Length >= 3) {
			int w = ((int)loc.w - 1 + numStates) % numStates;
			subtractInfluence3D (loc, weight, inf_map, w);
		}
	}

	private static void subtractInfluence3D(Vector4 loc, int weight, int [][,,] inf_map, int w)
	{
		for (int x_offset = -2; x_offset <= 2; x_offset++) {
			for (int z_offset = -2; z_offset <= 2; z_offset++) {
				Vector4 new_loc = new Vector4(loc.x + x_offset, loc.y, loc.z + z_offset, w);
				if (isInInfMap(new_loc, inf_map)) {
					subtractLocation(new_loc, Matrix2D[x_offset + MATRIX_2D_OFFSET, z_offset + MATRIX_2D_OFFSET] * weight, inf_map);
				}
			}
		}
	}

	public static void subtractLocation(Vector4 loc, int value, int[][,,] inf_map)
	{
		inf_map [(int)loc.w] [(int)loc.y, (int)loc.x, (int)loc.z] -= value;
	}


	private void addToInfluenceMap(Vector4 loc, int weight, int[][,,] inf_map)
	{
		addToInfluence3D (loc, weight, inf_map, (int)loc.w);

		int numStates = inf_map.Length;
		if (numStates >= 2) {
			int w = ((int)loc.w + 1) % numStates;
			addToInfluence3D (loc, weight, inf_map, w);
		}
		
		if (inf_map.Length >= 3) {
			int w = ((int)loc.w - 1 + numStates) % numStates;
			addToInfluence3D (loc, weight, inf_map, w);
		}
	}

	private void addToInfluence3D(Vector4 loc, int weight, int[][,,] inf_map, int w)
	{
		for (int x_offset = -2; x_offset <= 2; x_offset++) {
			for (int z_offset = -2; z_offset <= 2; z_offset++) {
				Vector4 new_loc = new Vector4(loc.x + x_offset, loc.y, loc.z + z_offset, w);
				if (isInInfMap(new_loc, inf_map)) {
					addToLocation(new_loc, Matrix2D[x_offset + MATRIX_2D_OFFSET, z_offset + MATRIX_2D_OFFSET] * weight, inf_map);
				}
			}
		}
	}

	private void addToLocation(Vector4 loc, int value, int[][,,] inf_map)
	{
		inf_map [(int)loc.w] [(int)loc.y, (int)loc.x, (int)loc.z] += value;
	}

	public GameObject registerGridGameObject(GameObject go,int row,int ver,int col){
		//int go_type = TAG_TYPE_DICT[go.tag];
		
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

	public int getRowNumber(float x) {
		int num = (int)Mathf.Round (x / TILE_SIZE);
		num = num >= 0 ? num : 0;
		int numRows = getNRows ();
		return num >= numRows ? numRows - 1 : num; 
	}

	public int getVerNumber(float y) {
		int num = (int)Mathf.Round (y / TILE_SIZE);
		num = num >= 0 ? num : 0; 
		int numVers = getNVers ();
		return num >= numVers ? numVers - 1 : num;
	}

	public int getColNumber(float z) {
		int num = (int)Mathf.Round (z / TILE_SIZE);
		num = num >= 0 ? num : 0;
		int numCols = getNCols ();
		return num >= numCols ? numCols - 1 : num;
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
}
