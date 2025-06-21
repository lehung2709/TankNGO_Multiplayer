using NUnit.Framework;
using System;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TimeCounter : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI timeTMP;

    private NetworkVariable<int> timeCounter = new NetworkVariable<int>(
        300,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public event Action<string> OnTimeUp;

    private float timer=0f;

    private bool isStart=false;

    public override void OnNetworkSpawn()
    {
        timeCounter.OnValueChanged += OnTimeChange;
        timeTMP.text = timeCounter.Value.ToString();

    }

    public override void OnNetworkDespawn()
    {
        timeCounter.OnValueChanged -= OnTimeChange;
    }



    private void Update()
    {
        if (!isStart) return;
        if (!IsServer || !(timeCounter.Value>0)) return;

        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer = 0f;
            timeCounter.Value--;
            if(timeCounter.Value <= 0f)
            {
                List<string> playersHaveHighestScoreList =  Leaderboard.Instance.GetPlayersHaveHighestScore();
                string message = string.Join("\n", playersHaveHighestScoreList);
                OnTimeUp?.Invoke(message);
            }    
        }
    }

    private void OnTimeChange(int oldValue, int newValue)
    {
        
        timeTMP.text = newValue.ToString();
    }

    public void StartCount()
    {
        isStart = true;
    }


}
