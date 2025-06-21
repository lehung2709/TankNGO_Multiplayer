using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BuffPickup : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> ID = new NetworkVariable<FixedString32Bytes>("",
       NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Server);

    [SerializeField] private SpriteRenderer Icon;

    public override void OnNetworkSpawn()
    {
       
        if (IsClient)
        {
            ID.OnValueChanged += OnBuffIDChange;
        }
    }

    private void OnBuffIDChange(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        StartCoroutine(WaitForBuffLibAndSetIcon(newValue.ToString()));
    }

    private IEnumerator WaitForBuffLibAndSetIcon(string ID)
    {
        yield return new WaitUntil(() => BuffLib.Instance != null);
        Icon.sprite = BuffLib.Instance.GetBuffByID(ID).icon;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<NetworkBehaviour>().IsOwner)
        {
            AudioManager.Instance.SpawnSoundEmitter(null,"Pickup",transform.position);
        }
        if(IsServer)
        {
            if(collision.TryGetComponent<PlayerStats>(out var playerStats))
            {
                playerStats.GetBuff(ID.Value.ToString());
                BuffSpawner.Instance.DespawnBuff(this.NetworkObject);
                
            } 
                
        }    
    }
    

    public void SetBuffID(string ID)
    {
        this.ID.Value = ID;

    }    


}
