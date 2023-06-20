//using System.Collections;
//using System.Collections.Generic;
using Unity.Netcode;
//using TMPro;
using UnityEngine;

public class TestServerRPC : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if(!IsServer)
        {
            TestServerRpc(0);
        }
    }

    [ClientRpc]
    void TestClientRpc(int value)
    {
        if(IsClient)
        {
            Debug.Log("Client Recieved the RPC #" + value);
            TestServerRpc(value + 1);

        }
    }
    [ServerRpc]
    void TestServerRpc(int value)
    {
        if(IsServer)
        {
            Debug.Log("Server Recieved the RPC #" + value);
            TestClientRpc(value + 1);
        }
    }

}
