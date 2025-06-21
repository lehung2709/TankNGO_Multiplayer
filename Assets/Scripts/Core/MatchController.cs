using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class MatchController : NetworkBehaviour
{
    public static MatchController Instance { get; private set; }
    [SerializeField] private EndGameUI endGameUI;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private TimeCounter timeCounter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadingUI.Instance.DisableLoadingUI();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            timeCounter.OnTimeUp += EndGame;
            StartCoroutine(StartGameCoroutine());
        }
    }

    IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(3f); 

        List<ulong> clients = HostSingleton.Instance.GameManager.NetworkServer.GetListClientID();

       foreach (var clientId in clients)
        {
            GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

            NetworkObject playerNetworkObject = player.GetComponent<NetworkObject>();

            playerNetworkObject.SpawnAsPlayerObject(clientId,true);
            player.GetComponent<TankPlayer>().SetPlayerName(HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(clientId).userName);
        }
        if (BuffSpawner.Instance != null)
        {
            BuffSpawner.Instance.SpawnBuffs();
        }
        timeCounter.StartCount();
    }
    [ClientRpc]
    private void EndGameClientRpc(string winnerPlayer)
    {
        endGameUI.EndGame(winnerPlayer);
    }

    public void EndGame(string winnerPlayer)
    {
        EndGameClientRpc(winnerPlayer);
        StartCoroutine(Back2LobbySceneCoroutine());
    }    

    private  IEnumerator Back2LobbySceneCoroutine()
    {
        yield return new WaitForSeconds(4f);
        SetLobbyAcessableAndReturnLobbyScene();

    }

    private async void SetLobbyAcessableAndReturnLobbyScene()
    {
        await HostSingleton.Instance.GameManager.SetLobbyAccessAsync(true);
        NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);


    }
    
}
