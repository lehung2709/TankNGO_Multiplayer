using UnityEngine;
using UnityEngine.Pool;
using Unity.Netcode;
using System.Collections.Generic;

public class ProjectilePool : NetworkBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    [Header("Projectile Prefabs")]
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;

    [SerializeField] private int prewarmCount=6;

    private ObjectPool<GameObject> serverProjectilePool;
    private ObjectPool<GameObject> clientProjectilePool;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        
            if (IsServer)
            {
                serverProjectilePool = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(serverProjectilePrefab),
                    actionOnGet: (proj) => proj.SetActive(true),
                    actionOnRelease: (proj) => proj.SetActive(false),
                    actionOnDestroy: (proj) => Destroy(proj),
                    collectionCheck: false,
                    defaultCapacity: 10,
                    maxSize: 20
                );
                List<GameObject> tempList = new List<GameObject>();

                for (int i = 0; i < prewarmCount; i++)
                {
                   var proj = serverProjectilePool.Get();
                   tempList.Add(proj);
                }

            
                foreach (var proj in tempList)
                {
                   serverProjectilePool.Release(proj);
                }

            
            }

            if (IsClient)
            {
                clientProjectilePool = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(clientProjectilePrefab),
                    actionOnGet: (proj) => proj.SetActive(true),
                    actionOnRelease: (proj) => proj.SetActive(false),
                    actionOnDestroy: (proj) => Destroy(proj),
                    collectionCheck: false,
                    defaultCapacity: 10,
                    maxSize: 20
                );

               
               List<GameObject> tempList = new List<GameObject>();

                for (int i = 0; i < prewarmCount; i++)
                {
                   var proj = clientProjectilePool.Get();
                   tempList.Add(proj);
                }

            
                foreach (var proj in tempList)
                {
                   clientProjectilePool.Release(proj);
                }


            }

        
        
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;

        
    }

    public GameObject GetServerProjectile()
    {
        if (!IsServer) return null;
        return serverProjectilePool.Get();
    }

    public GameObject GetClientProjectile()
    {
        if (!IsClient) return null;
        return clientProjectilePool.Get();
    }

    public void DespawmServerProjectile(GameObject projectile)
    {
        if (!IsServer) return;
        serverProjectilePool.Release(projectile);
    }

    public void DespawnClientProjectile(GameObject projectile)
    {
        if (!IsClient) return;
        clientProjectilePool.Release(projectile);
    }
}
