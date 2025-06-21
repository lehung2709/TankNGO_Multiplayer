using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LobbyReadyManager : NetworkBehaviour
{
    public static LobbyReadyManager Instance { get; private set; }

    [SerializeField] private Transform playerEntriesContainer;
    [SerializeField] private GameObject playerEntryPrefab;
    [SerializeField] private GameObject hostUI;
    [SerializeField] private GameObject clientUI;
    [SerializeField] private TextMeshProUGUI readyCountTMP;

    private NetworkVariable<int> readyCount = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);


    private PlayerEntry clientPlayerEntry;


    


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;

        }    
        else
        {
            Destroy(this);
        }    
    }

    private void Start()
    {
        LoadingUI.Instance.DisableLoadingUI();

    }
    public override void OnNetworkSpawn()
    {
        readyCount.OnValueChanged += OnReadyCountChange;
        playerCount.OnValueChanged += OnPlayerCountChange;
        if (IsServer)
        {
            hostUI.SetActive(true);
            HostSingleton.Instance.GameManager.NetworkServer.OnClientApproved += SpawnPlayerEntry;
            StartCoroutine(SpawnPlayerEntryCoroutine());
        }
        else if (IsClient) clientUI.SetActive(true);

        readyCountTMP.text = readyCount.Value.ToString() + "/" + playerCount.Value.ToString();


    }
    private void SpawnPlayerEntry4Host()
    {
        GameObject newPlayerEntry = Instantiate(playerEntryPrefab, Vector2.zero, Quaternion.identity);
        newPlayerEntry.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId,true);
        newPlayerEntry.GetComponent<PlayerEntry>().SetPlayerName(HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId).userName);
        SetOwnerPlayerEntry(newPlayerEntry.GetComponent<PlayerEntry>());
        playerCount.Value++;
        ToggleReady();

    }    

    private void SpawnPlayerEntries()
    {
        SpawnPlayerEntry4Host();
        List<ulong> clientsIDs = HostSingleton.Instance.GameManager.NetworkServer.GetListClientID();
        foreach (ulong clientID in clientsIDs)
        {
            if(clientID==OwnerClientId) continue;
            SpawnPlayerEntry(clientID, HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(clientID));
        } 
            
    }  
    private IEnumerator SpawnPlayerEntryCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        SpawnPlayerEntries();
    }    
    private void SpawnPlayerEntry(ulong clientID,UserData userData)
    {
        GameObject newPlayerEntry= Instantiate(playerEntryPrefab,Vector2.zero,Quaternion.identity);
        newPlayerEntry.GetComponent<NetworkObject>().SpawnWithOwnership(clientID,true);
        newPlayerEntry.GetComponent<PlayerEntry>().SetPlayerName(userData.userName);
        playerCount.Value++;
        Debug.Log(playerCount.Value);

    }

    public void SetParennt(Transform transform)
    {
        transform.SetParent(playerEntriesContainer,true);
    }    
    public void SetOwnerPlayerEntry(PlayerEntry clientPlayerEntry)
    {
        this.clientPlayerEntry = clientPlayerEntry;
    }


    public bool ToggleReady()
    {
        bool preReadyStatus = clientPlayerEntry.IsReady;
        clientPlayerEntry.ToggleReadyStatusServerRPC();
        return !preReadyStatus;
    }
    public void ChangeReadyCount(bool isReady)
    {
        if (isReady) readyCount.Value++;
        else readyCount.Value--;
    }

    private void OnReadyCountChange(int oldValue,int newValue)
    {
        readyCountTMP.text=newValue.ToString()+"/"+ playerCount.Value.ToString();
    }

    private void OnPlayerCountChange(int oldValue, int newValue)
    {
        readyCountTMP.text = readyCount.Value.ToString() + "/" + newValue.ToString();
    }
    public async void StartGame()
    {
        if (IsServer && readyCount.Value==playerCount.Value)
        {
            await HostSingleton.Instance.GameManager.SetLobbyAccessAsync(false);
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }
    
    public void QuitLobby()
    {
        ClientDisconectedServerRPC(clientPlayerEntry.IsReady);
        ClientSingleton.Instance.GameManager.NetworkClient.Disconnect();
    }
    public void CloseLobby()
    {
        HostSingleton.Instance.GameManager.Shutdown();
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        HostSingleton.Instance.GameManager.NetworkServer.OnClientApproved -= SpawnPlayerEntry;

    }

    [ServerRpc(RequireOwnership =false)]
    public void ClientDisconectedServerRPC(bool isReady)
    {
        playerCount.Value--;
        if(isReady) readyCount.Value--;
    }
    



}
