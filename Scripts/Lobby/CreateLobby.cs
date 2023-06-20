using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using Unity.Services.Lobbies;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using Unity.Services.Authentication;


public class CreateLobby : MonoBehaviour
{
    
    public TMP_InputField lobbyname;
    public TMP_InputField lobbyCode;
    public TMP_Dropdown maxplayers;
    public TMP_Dropdown gamemode;
    public Toggle islobbyprivate;

    public object AuthenticationSerivce { get; private set; }



    // Start is called before the first frame update

    public async void CreateLobbyMethod()
    {
        string lobbyName = lobbyname.text;
        int maxPlayers = Convert.ToInt32(maxplayers.options[maxplayers.value].text);
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = islobbyprivate.isOn;


        //Player Creation

        options.Player = new Player(AuthenticationService.Instance.PlayerId);

        options.Player.Data = new Dictionary<string, PlayerDataObject>()
        {
            { "PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "5")}
        };

        // Lobby Data

        options.Data = new Dictionary<string, DataObject>()
        {
            {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gamemode.options[gamemode.value].text,
            DataObject.IndexOptions.S1) }
        };


        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        GetComponent<CurrentLobby>().currentLobby = lobby;
        DontDestroyOnLoad(this);
        Debug.Log("Create Lobby Done!");


        Lobbystatic.LogLobby(lobby);
        Lobbystatic.LogPlayersInLobby(lobby);
        lobbyCode.text = lobby.LobbyCode;

        StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15f));

        Lobbystatic.LoadLobbyRoom();

    }
    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new waitForSeconds(waitTimeSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;

        }
    }

 
}

class waitForSeconds
{
    private float waitTimeSeconds;

    public waitForSeconds(float waitTimeSeconds)
    {
        this.waitTimeSeconds = waitTimeSeconds;
    }
}