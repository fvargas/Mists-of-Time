using UnityEngine;
using UnityEditor;
using System.Diagnostics;
public class ScenePlugin {
	[MenuItem("Tools/Build Scene")]
	public static void DownscaleRefTextures() {
		Process p = new Process();
		p.StartInfo.FileName = "python";
		p.StartInfo.Arguments = "level.py 16 16 1";    
		// Pipe the output to itself - we will catch this later
		p.StartInfo.RedirectStandardError=true;
		p.StartInfo.RedirectStandardOutput=true;
		p.StartInfo.CreateNoWindow = true;
		
		// Where the script lives
		p.StartInfo.WorkingDirectory = Application.dataPath; 
		p.StartInfo.UseShellExecute = false;
		
		p.Start();
		// Read the output - this will show is a single entry in the console - you could get  fancy and make it log for each line - but thats not why we're here
		string output = p.StandardOutput.ReadToEnd();
		GameObject[] obj = GameObject.FindGameObjectsWithTag("GO_WATER");
		for (int i=0; i<obj.GetLength(0); i++) {
			Editor.DestroyImmediate(obj[i]);
		}
		obj = GameObject.FindGameObjectsWithTag("GO_ROCK");
		for (int i=0; i<obj.GetLength(0); i++) {
			Editor.DestroyImmediate(obj[i]);
		}
		obj = GameObject.FindGameObjectsWithTag("GO_GRASS");
		for (int i=0; i<obj.GetLength(0); i++) {
			Editor.DestroyImmediate(obj[i]);
		}
		int n_col = 0;
		string [] lines = output.Split ('\n');
		for (int i=0; i<lines.GetLength(0)-1; i++) {
			string [] cols = lines[i].Split(' ');
			n_col = cols.GetLength(0);
			for(int j=0;j<cols.GetLength(0);j++){
				int val = int.Parse(cols [j]);
				if(val == 1){
					//Lava
					GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(Resources.Load(Map.TYPE_RESOURCE_DICT[Map.GO_LAVA]) as GameObject);
					go.transform.position = new Vector3((i+1)*Map.TILE_SIZE,0,(j+1)*Map.TILE_SIZE);
				}else if(val == 0){
					//Rock
					GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(Resources.Load(Map.TYPE_RESOURCE_DICT[Map.GO_ROCK]) as GameObject);
					go.transform.position = new Vector3((i+1)*Map.TILE_SIZE,0,(j+1)*Map.TILE_SIZE);
				}else if(val == 2){
					//Start
					GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(Resources.Load(Map.TYPE_RESOURCE_DICT[Map.GO_ROCK]) as GameObject);
					go.transform.position = new Vector3((i+1)*Map.TILE_SIZE,0,(j+1)*Map.TILE_SIZE);
				}else if(val == 3){
					//Finish
					GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(Resources.Load(Map.TYPE_RESOURCE_DICT[Map.GO_DEST]) as GameObject);
					go.transform.position = new Vector3((i+1)*Map.TILE_SIZE,0,(j+1)*Map.TILE_SIZE);
				}
			
			}

		}
		GameObject.Find ("Warrior").transform.position = new Vector3 (0.5f*Map.TILE_SIZE,0f,(float)((float)n_col-0.5)*Map.TILE_SIZE);
		p.WaitForExit();
		p.Close();
	}
	[MenuItem("Tools/Sample Scene")]
	public static void SampleScene() {
		GameManager gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		//UnityEditor.Debug.Log(Utils.toString (gm.m.background_g));
	}
}
