using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
//Thanks to http://answers.unity3d.com/questions/780097/how-to-use-the-new-input-field.html for some inputfield help.
public class NetworkScript : NetworkManager
{

    bool toggle = false;
    public GameObject button;
    

    public static short MSGType = 555;

    // Use this for initialization
    void Start()
    {
        button = GameObject.Find("ToggleButton");
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnStartClient(NetworkClient mClient)
    {

        mClient.RegisterHandler(MSGType, OnClientChatMessage);
    }



    // hook into NetManagers server setup process
    public override void OnStartServer()
    {
        base.OnStartServer(); //base is empty
        NetworkServer.RegisterHandler(MSGType, OnServerChatMessage);
    }

    private void OnServerChatMessage(NetworkMessage netMsg)
    {
        StringMessage msg = netMsg.ReadMessage<StringMessage>();
        button.GetComponent<ToggleScript>().ToggleColor();
        //button.GetComponent<ToggleScript>().DisplayTextData(msg.value);
        NetworkServer.SendToAll(NetworkScript.MSGType, new StringMessage(msg.value));
    }

    private void OnClientChatMessage(NetworkMessage netMsg)
    {
        StringMessage msg = netMsg.ReadMessage<StringMessage>();
        button.GetComponent<ToggleScript>().ToggleColor();
        button.GetComponent<ToggleScript>().DisplayTextData(msg.value);
    }
    
    
}
