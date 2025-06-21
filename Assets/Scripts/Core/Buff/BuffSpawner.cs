using Unity.Netcode;
using UnityEngine;

public class BuffSpawner : NetworkBehaviour
{
    public static BuffSpawner Instance { get; private set; }

    [SerializeField] private GameObject buffPrefabs; 
    [SerializeField] private int buffCount = 10; 
    [SerializeField] Vector2 checkBoxSize = new Vector2(1.42f, 1.42f);


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
   

    public void SpawnBuffs()
    {
        for (int i = 0; i < buffCount; i++)
        {
            SpawnBuff();
        }
    }

    private void SpawnBuff()
    {
        
        Vector2 spawnPos = SpawnPointGenerator.Instance.GetSpawnPoint(checkBoxSize);

        
        NetworkObject buffInstance = NetworkObjectPool.Singleton.GetNetworkObject(buffPrefabs, spawnPos, Quaternion.identity);


        if (buffInstance != null)
        {
               
            buffInstance.Spawn(true);
            buffInstance.GetComponent<BuffPickup>().SetBuffID(BuffLib.Instance.GetRandomID());              
        }
        

    }

    public void DespawnBuff(NetworkObject buffInstance)
    {
        buffInstance.Despawn();
        NetworkObjectPool.Singleton.ReturnNetworkObject(buffInstance, buffPrefabs);
        SpawnBuff();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, checkBoxSize);

    }
}
