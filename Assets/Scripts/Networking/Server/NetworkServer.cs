using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static Unity.Networking.Transport.NetworkPipelineStage;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public event Action<ulong, UserData> OnClientApproved;
    public Action<string> OnClientLeft;


    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback = ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);


        AddClient(request.ClientNetworkId, userData);

        response.Approved = true;
        response.CreatePlayerObject = true;

        OnClientApproved?.Invoke(request.ClientNetworkId,userData);
    }

    private void AddClient(ulong clientNetworkId,UserData userData)
    {
        clientIdToAuth[clientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;

        foreach (var kvp in clientIdToAuth)
        {
            string name = authIdToUserData.TryGetValue(kvp.Value, out var ud) ? ud.userName : "Unknown";
            Debug.Log($"ClientId: {kvp.Key}, AuthId: {kvp.Value}, Name: {name}");
        }

    }

    public List<ulong>  GetListClientID()
    {
        return clientIdToAuth.Keys.ToList();
    }

    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            authIdToUserData.Remove(authId);
            OnClientLeft?.Invoke(authId);
        }
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            if (authIdToUserData.TryGetValue(authId, out UserData data))
            {
                return data;
            }

            return null;
        }

        return null;
    }


    public void Dispose()
    {
        if (networkManager == null) { return; }

        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        networkManager.OnServerStarted -= OnNetworkReady;

        if (networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }
}

