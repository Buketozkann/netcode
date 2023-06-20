using System;
using System.Collections;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;

public class JoinLobby : MonoBehaviour
{

    public TMPro.TMP_InputField InputField;



    public async void JoinLobbywithLobbyCode(string lobbyCode)
    {
        var code = InputField.text;

        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            options.Player = new Player(AuthenticationService.Instance.PlayerId);

            options.Player.Data = new Dictionary<string, PlayerDataObject>()
        {
            { "PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "8")}
        };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
            Debug.Log("Joined Lobby with code : " + code);

            DontDestroyOnLoad(this);
            GetComponent<CurrentLobby>().currentLobby = lobby;

            Lobbystatic.LogPlayersInLobby(lobby);
            Lobbystatic.LoadLobbyRoom();
            

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);

        }

    }
    public async void JoinLobbywithLobbyId(string lobbyId)
    {


        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();
            options.Player = new Player(AuthenticationService.Instance.PlayerId);

            options.Player.Data = new Dictionary<string, PlayerDataObject>()
        {
            { "PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "8")}
        };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
           
            DontDestroyOnLoad(this);
            GetComponent<CurrentLobby>().currentLobby = lobby;

            Debug.Log("Joined Lobby with ID : " + lobbyId);
            Debug.LogWarning("Lobby Code: " + lobby.LobbyCode);


            Lobbystatic.LogPlayersInLobby(lobby);
            Lobbystatic.LoadLobbyRoom();

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);

        }

    }

    public async void QuickJoinMethod()
    {
        try
        {

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            GetComponent<CurrentLobby>().currentLobby = lobby;
            DontDestroyOnLoad(this);
            Debug.Log("Joined Lobby with Quick Join : " + lobby.Id);
            Debug.LogWarning("Lobby Code: " + lobby.LobbyCode);
            Lobbystatic.LoadLobbyRoom();

        }

        catch (LobbyServiceException e)
        {
            Debug.LogError(e);

        }
    }
}