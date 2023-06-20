//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System.Collections.Generic;

public class PopulateUI : MonoBehaviour
{
    public TextMeshProUGUI lobbyname;
    public TextMeshProUGUI lobbyCode;
    public TextMeshProUGUI gameMode;
    public TMP_InputField newName;
    public TMP_InputField newPlayerName;

    public GameObject PlayerInfoContainer;
    public GameObject PlayerInfoPrefab;

    private CurrentLobby _currentLobby;

    private string lobbyId;

    // Start is called before the first frame update
    void Start()
    {
        _currentLobby = GameObject.Find("LobbyManager").GetComponent<CurrentLobby>();
        PopulateUIElements();
        lobbyId = _currentLobby.currentLobby.Id;
        InvokeRepeating(nameof(PollForLobbyUpdate), 1.1f, 2f);
        Lobbystatic.LogPlayersInLobby(_currentLobby.currentLobby);

    }

    void PopulateUIElements()
    {
        lobbyname.text = _currentLobby.currentLobby.Name;
        lobbyCode.text = _currentLobby.currentLobby.LobbyCode;
        gameMode.text = _currentLobby.currentLobby.Data["GameMode"].Value;
        ClearContainer();
        foreach(Player player in _currentLobby.currentLobby.Players)
        {
            CreatePlayerInfoCard(player);
        }

    }

    void CreatePlayerInfoCard(Player player)
    {
        var text = Instantiate(PlayerInfoPrefab, Vector3.zero, Quaternion.identity);
        text.name = player.Joined.ToShortTimeString();
        text.GetComponent<TextMeshProUGUI>().text = player.Id + ":" + player.Data["PlayerLevel"].Value;
        var recTransform = text.GetComponent<RectTransform>();
        recTransform.SetParent(PlayerInfoContainer.transform);
    }


    private void ClearContainer()
    {
        if (PlayerInfoContainer is not null && PlayerInfoContainer.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in PlayerInfoContainer.transform)
            {
                Destroy(VARIABLE.gameObject);
            }
        }
    }

    async void PollForLobbyUpdate()
    {
        _currentLobby.currentLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
        PopulateUIElements();
    }


    ///BUTTON EVENTS

    public async void ChangeLobbyName()
    {
        var newLobbyName  = newName.text;

        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.Name = newLobbyName;
            _currentLobby.currentLobby = await Lobbies.Instance.UpdateLobbyAsync(lobbyId, options);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public async void ChangePlayerName()
    {
        var PlayerName = newPlayerName.text;

        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>()
        {
            { "PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerName)}
        };
            await LobbyService.Instance.UpdatePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId,options);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }
}
