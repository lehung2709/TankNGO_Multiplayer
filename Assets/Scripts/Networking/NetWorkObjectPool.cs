using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


    public class NetworkObjectPool : NetworkBehaviour
    {
        public static NetworkObjectPool Singleton { get; private set; }

        [SerializeField] private List<PoolConfigObject> PooledPrefabsList;

        private readonly Dictionary<GameObject, Queue<NetworkObject>> pool = new();
        private bool initialized = false;

        private void Awake()
        {
            if (Singleton && Singleton != this) Destroy(gameObject);
            else Singleton = this;
        }

        public override void OnNetworkSpawn()
        {
            InitializePool();

            
        }

        public override void OnNetworkDespawn() => ClearPool();

        public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position = default, Quaternion rotation = default)
        {
        if (!pool.TryGetValue(prefab, out var queue)) return null;

        NetworkObject netObject = null;
        int initialCount = queue.Count;

        for (int i = 0; i < initialCount; i++)
        {
            var candidate = queue.Dequeue();

            if (!candidate.IsSpawned)
            {
                netObject = candidate;
                break;
            }

            queue.Enqueue(candidate);
        }

        if (netObject == null)
        {
            netObject = Instantiate(prefab).GetComponent<NetworkObject>();
        }

        netObject.transform.SetPositionAndRotation(position, rotation);
        netObject.gameObject.SetActive(true);
        return netObject;
    }

        public void ReturnNetworkObject(NetworkObject netObject, GameObject prefab)
        {
            netObject.gameObject.SetActive(false);
            if (pool.TryGetValue(prefab, out var queue)) queue.Enqueue(netObject);

        }

        public void AddPrefab(GameObject prefab, int prewarmCount = 0)
        {
            if (!prefab.TryGetComponent(out NetworkObject _)) return;
            if (pool.ContainsKey(prefab)) return;

            pool[prefab] = new Queue<NetworkObject>();
            for (int i = 0; i < prewarmCount; i++)
            {
                var netObject = Instantiate(prefab).GetComponent<NetworkObject>();
                ReturnNetworkObject(netObject, prefab);
            }

            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));
        }

        private void InitializePool()
        {
            if (initialized) return;
            foreach (var config in PooledPrefabsList) AddPrefab(config.Prefab, config.PrewarmCount);
            initialized = true;
        }

        private void ClearPool()
        {
            foreach (var prefab in pool.Keys)
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
            pool.Clear();
        }
    }

    [Serializable]
    struct PoolConfigObject
    {
        public GameObject Prefab;
        public int PrewarmCount;
    }

    class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        private readonly GameObject prefab;
        private readonly NetworkObjectPool pool;

        public PooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
        {
            this.prefab = prefab;
            this.pool = pool;
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
            => pool.GetNetworkObject(prefab, position, rotation);

        public void Destroy(NetworkObject networkObject)
            => pool.ReturnNetworkObject(networkObject, prefab);
    }

