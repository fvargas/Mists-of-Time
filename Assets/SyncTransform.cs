using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//http://forum.unity3d.com/threads/unity-5-unet-multiplayer-tutorials-making-a-basic-survival-co-op.325692/
//This file uses code from the first and second videos.
//The above link goes to a set of helpful tutorials where lots of information was gained about setting up multiplayer.

public class SyncTransform : NetworkBehaviour{

    [SyncVar]
    Vector3 syncedPosition;
    [SyncVar]
    Quaternion syncedRotation;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float lerpRate = 15;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        TransmitTransforms();
        lerpTransform();
	}

    void lerpTransform()
    {
        if(!isLocalPlayer)
        {
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, syncedRotation, lerpRate * Time.deltaTime);
            playerTransform.position = Vector3.Lerp(playerTransform.position, syncedPosition, lerpRate * Time.deltaTime);
        }
    }

    [Command]
    void CmdSyncPlayerTransforms(Vector3 pos, Quaternion rot)
    {
        syncedPosition = pos;
        syncedRotation = rot;
    }

    [ClientCallback]
    void TransmitTransforms()
    {
        if(isLocalPlayer)
        {
            CmdSyncPlayerTransforms(playerTransform.position, playerTransform.rotation);
        }
    }
}
