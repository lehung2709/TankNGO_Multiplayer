using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [field: SerializeField] public float DefautSpeed { get; private set; } = 5f;

    public NetworkVariable<float> movementSpeed = new NetworkVariable<float>(
        5f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [field: SerializeField] public float DefautFireRate { get; private set; } = 1f;
    public NetworkVariable<float> fireRate = new NetworkVariable<float>(
        1f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<bool> shield = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public float turningRate = 300f;
    public float projectileSpeed = 20;
    


    private Dictionary<string, Coroutine> buffCoroutinesDic = new Dictionary<string, Coroutine>();


    public void GetBuff(string ID)
    {
        if (buffCoroutinesDic.ContainsKey(ID))
        {
            if(buffCoroutinesDic[ID] != null) StopCoroutine(buffCoroutinesDic[ID]);
            buffCoroutinesDic.Remove(ID);   
        }
        buffCoroutinesDic[ID] = BuffLib.Instance.GetBuffByID(ID).ApplyBuff(this);
    }
   
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            movementSpeed.Value=DefautSpeed;
            fireRate.Value = DefautFireRate;
        }    
    }
}
