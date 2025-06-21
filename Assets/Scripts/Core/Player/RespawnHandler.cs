using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private GameObject model;

    [SerializeField] private GameObject deadModel;

    [SerializeField] private Health health;

    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private DeadUI deadUI;

    [SerializeField] private float duration = 5f;

    [SerializeField] Vector2 checkBoxSize = new Vector2(3f, 3f);


    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            health.OnDie += OnDie; 
        }
        if (IsOwner)
        {
            deadUI = FindFirstObjectByType<DeadUI>();
            transform.position = SpawnPointGenerator.Instance.GetSpawnPoint(checkBoxSize);

        }
    }

    private void OnDie()
    {
        if (IsServer)
        {
            DieClientRpc();
            ShowRespawnCountdownClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { OwnerClientId }
                }
            });
            StartCoroutine(CountDownCoroutine());
        } 
        


    }    
    [ClientRpc]
    private void DieClientRpc()
    {
        model.SetActive(false);
        deadModel.SetActive(true);
        foreach (var script in scriptsToDisable)
        {
            script.enabled = false;
        }
    }
    [ClientRpc]
    private void ReviceClientRpc()
    {
        deadModel.SetActive(false);
        foreach (var script in scriptsToDisable)
        {
            script.enabled = true;
        }
        if (IsOwner)
        {
            transform.position = SpawnPointGenerator.Instance.GetSpawnPoint(checkBoxSize);
        }
        model.SetActive(true);


    }

    [ClientRpc]
    private void ShowRespawnCountdownClientRpc(ClientRpcParams rpcParams = default)
    {
        if (IsOwner && deadUI != null)
        {
            deadUI.StartCountDown(duration);
        }
    }
    private IEnumerator CountDownCoroutine()
    {
        yield return new WaitForSeconds(duration);
        ReviceClientRpc();
        health.Revive();
    }

    public override void OnNetworkDespawn()
    {
        health.OnDie -= OnDie;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, checkBoxSize);

    }
}
