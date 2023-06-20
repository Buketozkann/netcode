//using System.Collections;
//using System.Collections.Generic;
using Unity.Netcode;
using TMPro;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    private NetworkManager NetworkManager;
    public TextMeshProUGUI text;
    
    void Start()
    {
        NetworkManager = GetComponentInParent<NetworkManager>();

    }

    public void StartHost()
    {
        NetworkManager.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.StartClient();
        InitMovementText();

    }

    public void SubmitNewPosition()
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        var player = playerObject.GetComponent<PlayerMovement>();
        player.Move();
    } 
    private void InitMovementText()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            text.text = "Move";
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            text.text = "Request Move";
        }
    }
}
