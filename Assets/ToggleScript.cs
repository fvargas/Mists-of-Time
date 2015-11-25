using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

public class ToggleScript : MonoBehaviour {

    bool toggle = false;
    [SerializeField] public Button submitButton;
    [SerializeField] public Button clearButton;
    [SerializeField] public InputField input;
    [SerializeField] public Text textField;
    [SerializeField] public ScrollRect scrollField;
    //http://answers.unity3d.com/answers/790847/view.html was very helpful in getting the field scroll to work.
    [SerializeField] public RectTransform rt;

	// Use this for initialization
	void Start () {
        GetComponent<Image>().color = Color.red;
        //submitButton = Button.Find("SubmitButton");
        //input = GameObject.Find("InputField");
        //submitButton.onClick.AddListener(() => DisplayTextData(input.text));
        submitButton.onClick.AddListener(() => SendMessage(input.text));
        submitButton.GetComponentInChildren<Text>().text = "Submit";
        clearButton.onClick.AddListener(() => Clear());
        clearButton.GetComponentInChildren<Text>().text = "Clear Chat";
        textField.text = "";
        //rt = gameObject.GetComponent<RectTransform>(); // Acessing the RectTransform 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void clickButton()
    {
        //Toggle yourself first
        ToggleColor();

        //Then toggle the other peer
        if (NetworkManager.singleton.IsClientConnected())
        {
            //If this is a client
            NetworkManager.singleton.client.Send(NetworkScript.MSGType, new IntegerMessage());
        }
        else if (NetworkManager.singleton.isNetworkActive)
        {
            //If this is a server
            NetworkServer.SendToAll(NetworkScript.MSGType, new IntegerMessage());
        }
    }

    public void SendMessage(string message)
    {
        //DisplayTextData(message);
        //Then toggle the other peer
        if (NetworkManager.singleton.IsClientConnected())
        {
            //If this is a client
            NetworkManager.singleton.client.Send(NetworkScript.MSGType, new StringMessage(message));
        }
        else if (NetworkManager.singleton.isNetworkActive)
        {
            //If this is a server
            NetworkServer.SendToAll(NetworkScript.MSGType, new StringMessage(message));
        }
    }

    public void ToggleColor()
    {
        if (toggle)
        {
            GetComponent<Image>().color = Color.red;
            toggle = false;

        }
        else
        {
            GetComponent<Image>().color = Color.green;
            toggle = true;
        }
 
 
    }

    public void DisplayTextData(string data)
    {
        Debug.Log("Stuff.");
        Debug.Log(data);
        //scrollField.content
        textField.text = textField.text + "\n" + data;
        input.text = "";
        //rt.sizeDelta = new Vector2(rt.rect.width, textField.preferredHeight); // Setting the height to equal the height of text
        textField.rectTransform.sizeDelta = new Vector2(textField.rectTransform.rect.width, textField.preferredHeight); // Setting the height to equal the height of text
    }
    
    public void Clear()
    {
        textField.text = "";
    }
}

