//using System.Collections;
//using System.Collections.Generic;
using Unity.Netcode;
//using TMPro;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        if(IsOwner && IsServer)
        {
           transform.RotateAround(GetComponentInParent<Transform>().position, Vector3.up, 100f * Time.deltaTime);
        }
        
    }
}
