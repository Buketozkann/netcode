//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
using System;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class RelayManager : MonoBehaviour
{

    private string PlayerID;
    private RelayHostData _hostData;
    private RelayJoinData _joinData;

    public TextMeshProUGUI JoinCodeTest;
    public TextMeshProUGUI IdTest;
    public TMP_InputField inputField;
    public TMP_Dropdown playerCount;
    public object UnityTransform;
    

    public UnityTransport UnityTransport { get; private set; }

    async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log("Unity Service Init");
        SignIn();
    }

    async void SignIn()
    {
        Debug.Log("Signing In");
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        PlayerID = AuthenticationService.Instance.PlayerId;
        Debug.Log("Signed In :" + PlayerID);
        IdTest.text = PlayerID;
    }

    async public void OnHostClient(NetworkManager networkManager)
    {
        int maxplayerCount = Convert.ToInt32(playerCount.options[playerCount.value].text);

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxplayerCount);

        _hostData = new RelayHostData()
        {
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,

            allocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            Key = allocation.Key,



        };

        _hostData.JoinCode = await RelayService.Instance.GetJoinCodeAsync(_hostData.allocationID);
        Debug.Log("Allocate Complete :" + _hostData.allocationID);
        Debug.LogWarning("JoinCode" + _hostData.JoinCode);

        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

        transport.SetRelayServerData(_hostData.IPv4Address, _hostData.Port, _hostData.AllocationIDBytes, _hostData.Key, _hostData.ConnectionData);

        NetworkManager.Singleton.StartHost();

    }

        public async void OnJoinClick()
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(inputField.text);

            _joinData = new RelayJoinData()
            {
                IPv4Address = allocation.RelayServer.IpV4,
                Port = (ushort)allocation.RelayServer.Port,

                allocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                Key = allocation.Key,
            };
            
            Debug.Log("Join success:" + _joinData.allocationID);
            UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            transport.SetRelayServerData(_joinData.IPv4Address, _joinData.Port, _joinData.AllocationIDBytes, _joinData.Key, _joinData.ConnectionData, _joinData.HostConnectionData);

            NetworkManager.Singleton.StartClient();

        
        }
    }

    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid allocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] Key;
    }

    public struct RelayJoinData
    {
        public string IPv4Address;
        public ushort Port;
        public Guid allocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }

