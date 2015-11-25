using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//http://forum.unity3d.com/threads/unity-5-unet-multiplayer-tutorials-making-a-basic-survival-co-op.325692/
//The above link goes to a set of helpful tutorials where lots of information was gained about setting up multiplayer.

public class NetworkSetup : NetworkBehaviour {

    //[SerializeField]
    //Animator anim;
    //[SerializeField]
    //CapsuleCollider collider;
    //[SerializeField]
    //CharacterController controller;
    //[SerializeField]
    //PlayerControl pc;
    //[SerializeField]
    //Movement m;


	// Use this for initialization
	void Start () {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerControl>().enabled = false;
        GetComponent<Movement>().enabled = false;
        if (isLocalPlayer)
        {
            GetComponent<CharacterController>().enabled = true;
            GetComponent<PlayerControl>().enabled = true;
            GetComponent<Movement>().enabled = true;
        }

	}
	

}
