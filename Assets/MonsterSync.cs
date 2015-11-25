using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//http://forum.unity3d.com/threads/unity-5-unet-multiplayer-tutorials-making-a-basic-survival-co-op.325692/
//This file uses code from the first, second, and sixteenth videos.
//The above link goes to a set of helpful tutorials where lots of information was gained about setting up multiplayer.

public class MonsterSync: NetworkBehaviour
{
    [SyncVar]
    Vector3 syncedPosition;
    [SyncVar]
    Quaternion syncedRotation;
    [SerializeField]
    private Transform myTransform;
    [SerializeField]
    private float lerpRate = 15;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TransmitTransforms();
        lerpTransform();
    }

    void lerpTransform()
    {
        if (!isServer)
        {
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, syncedRotation, lerpRate * Time.deltaTime);
            myTransform.position = Vector3.Lerp(myTransform.position, syncedPosition, lerpRate * Time.deltaTime);
        }
    }

    void TransmitTransforms()
    {
        if (isServer)
        {
            syncedPosition = myTransform.position;
            syncedRotation = myTransform.rotation;
        }
    }
}
