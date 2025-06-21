using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    public static Leaderboard Instance { get; private set; }

    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderBoardEntry leaderboardEntryPrefab;
    [SerializeField] private int entitiesToDisplay = 8;
    [SerializeField] private Color ownerColour;

    private NetworkList<LeaderBoardData> leaderboardDatas;
    private List<LeaderBoardEntry> leaderBoardEntries = new List<LeaderBoardEntry>();

    private void Awake()
    {
        if (Instance == null)
        {
            leaderboardDatas = new NetworkList<LeaderBoardData>();
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            leaderboardDatas.OnListChanged += HandleLeaderboardDatasChanged;
            List<ulong> clientList = HostSingleton.Instance.GameManager.NetworkServer.GetListClientID();
            foreach (ulong clientID in clientList)
            {
                LeaderBoardData data = new LeaderBoardData();
                data.ClientId = clientID;
                data.PlayerName= HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(clientID).userName;
                data.Scores = 0;
                leaderboardDatas.Add(data);
                Debug.Log(clientID);
            }    
        } 
        else if (IsClient)
        {
           
            
            foreach (LeaderBoardData entity in leaderboardDatas)
            {
                HandleLeaderboardDatasChanged(new NetworkListEvent<LeaderBoardData>
                {
                    Type = NetworkListEvent<LeaderBoardData>.EventType.Add,
                    Value = entity
                });
            }
            leaderboardDatas.OnListChanged += HandleLeaderboardDatasChanged;
        }
        
            

        
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardDatas.OnListChanged -= HandleLeaderboardDatasChanged;
        }

       
    }

    private void HandleLeaderboardDatasChanged(NetworkListEvent<LeaderBoardData> changeEvent)
    {
        if (!gameObject.scene.isLoaded) { return; }

        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderBoardData>.EventType.Add:
                if (!leaderBoardEntries.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    LeaderBoardEntry leaderBoardEntry =
                        Instantiate(leaderboardEntryPrefab, leaderboardEntityHolder);
                    leaderBoardEntry.Initialise(
                        changeEvent.Value.ClientId,
                        changeEvent.Value.PlayerName,
                        changeEvent.Value.Scores);
                    if (NetworkManager.Singleton.LocalClientId == changeEvent.Value.ClientId)
                    {
                        leaderBoardEntry.SetColour(ownerColour);
                    }
                    leaderBoardEntries.Add(leaderBoardEntry);
                }
                break;
            
            case NetworkListEvent<LeaderBoardData>.EventType.Value:
                LeaderBoardEntry displayToUpdate =
                    leaderBoardEntries.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateScores(changeEvent.Value.Scores);
                }
                break;
        }

        leaderBoardEntries.Sort((x, y) => y.Scores.CompareTo(x.Scores));

        for (int i = 0; i < leaderBoardEntries.Count; i++)
        {
            leaderBoardEntries[i].transform.SetSiblingIndex(i);
            leaderBoardEntries[i].UpdateText();
            leaderBoardEntries[i].gameObject.SetActive(i <= entitiesToDisplay - 1);
        }

        LeaderBoardEntry myDisplay =
            leaderBoardEntries.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        if (myDisplay != null)
        {
            if (myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                leaderboardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
        }

        
    }

    

    

    public void AddScores(ulong clientId, int additionScores)
    {
        for (int i = 0; i < leaderboardDatas.Count; i++)
        {
            if (leaderboardDatas[i].ClientId != clientId) { continue; }
            if(leaderboardDatas[i].Scores+additionScores == 20)
            {
                MatchController.Instance.EndGame(leaderboardDatas[i].PlayerName.ToString());
            }
            leaderboardDatas[i] = new LeaderBoardData
            {
                ClientId = leaderboardDatas[i].ClientId,
                PlayerName = leaderboardDatas[i].PlayerName,
                Scores = leaderboardDatas[i].Scores+additionScores,
            };

            return;
        }
    }

    public List<string> GetPlayersHaveHighestScore()
    {
        List<string> highestScorers = new List<string>();

        if (leaderboardDatas.Count == 0) return highestScorers;

        int highestScore = int.MinValue;

        foreach (var data in leaderboardDatas)
        {
            if (data.Scores > highestScore)
            {
                highestScore = data.Scores;
                highestScorers.Clear();
                highestScorers.Add(data.PlayerName.ToString());
            }
            else if (data.Scores == highestScore)
            {
                highestScorers.Add(data.PlayerName.ToString());
            }
        }

        return highestScorers;  
    }

}
