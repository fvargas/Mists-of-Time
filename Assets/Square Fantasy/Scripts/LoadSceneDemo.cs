using UnityEngine;
using System.Collections;

public class LoadSceneDemo : MonoBehaviour {

	// Update is called once per frame
	public void LoadScene (int levelNumber)
	{
		Application.LoadLevel(levelNumber);
	}
}
