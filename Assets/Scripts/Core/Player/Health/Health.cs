using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; } = 30;

    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public bool IsDead { get; private set; }

    public event Action OnDie;

    [SerializeField] private PlayerStats stats;

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        CurrentHealth.OnValueChanged += OnHealthChange;

        if (!IsServer) { return; }

        CurrentHealth.Value = MaxHealth;

    }

    public void TakeDamage(int damageValue)
    {
        if (IsDead) return;
        if (stats.shield.Value == true)
        {
            stats.shield.Value = false;
            return;
        }
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    private void ModifyHealth(int value)
    {
         
        int newHealth = CurrentHealth.Value + value;
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        if (CurrentHealth.Value <= 0)
        {
            OnDie?.Invoke();
            IsDead = true;
        }
    }
    public void Revive()
    {
        IsDead = false;
        CurrentHealth.Value=MaxHealth;
    }

    private void OnHealthChange(int oldValue, int newValue)
    {
        if (newValue <= 0)
        {
            AudioManager.Instance.SpawnSoundEmitter(null, "Explode", transform.position);

        }

        if (newValue<oldValue)
        {
            AudioManager.Instance.SpawnSoundEmitter(null, "RetroExplode", transform.position);

        }
        if (newValue > oldValue)
        {
           if(IsOwner)
            {
                AudioManager.Instance.SpawnSoundEmitter(null, "Heal", transform.position);

            }


        }
        
    }    
    


}
