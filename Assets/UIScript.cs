using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour {

    public GameObject gameCanvas;
    public GameObject menuCanvas;
    public bool menuEnabled = true;

	// Use this for initialization
	void Start () {
        //this.gameObject.SetActive(false);
        GameObject.Find("GameCanvas").SetActive(false);
        Button[] buttons;
        buttons = menuCanvas.GetComponentsInChildren<Button>();
        foreach( Button button in buttons )
        {
            Debug.Log(button.name);
            if (button.name == "Start_Btn")
            {
                button.GetComponentInChildren<Text>().text = "Start Game";
                button.onClick.AddListener(EnableGameUI);
            }
            else if (button.name == "Quit_Btn")
            {
                button.GetComponentInChildren<Text>().text = "Quit Game";
                button.onClick.AddListener(QuitGame);
            }
            Debug.Log(button.GetComponentInChildren<Text>().text);
        }
        menuCanvas.GetComponentInChildren<Text>().color = Color.white;
        GameObject.Find("Title").GetComponent<Text>().text = "Mists of Time";
        //menuCanvas.GetComponentInChildren<Text>().fontSize = 27;
	}
	
	public void EnableMenuUI()
    {
        menuEnabled = true;
        gameCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void EnableGameUI()
    {
        menuEnabled = false;
        gameCanvas.SetActive(true);
        menuCanvas.SetActive(false);
        }

    public void QuitGame()
    {
        Application.Quit();
    }
}
