using Unity.Netcode;
using UnityEngine;

public class HealthStation : NetworkBehaviour
{
    [SerializeField] private GameObject availableSignGOj;
    [SerializeField] private int healAmount = 50;
    [SerializeField] private float cooldown = 10f;

    private NetworkVariable<bool> isAvailable = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if(IsClient && !IsServer)
        {
            Collider2D _collider = GetComponent<Collider2D>();
            _collider.enabled = false;

        }

        if (IsClient)
        {
            isAvailable.OnValueChanged += (oldValue, newValue) => {
                UpdateVisual(newValue);
            };
            UpdateVisual(true);
        } 
            
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer || !isAvailable.Value) return;

        var playerHealth = other.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.RestoreHealth(healAmount);
            StartCooldown();
        }
    }

    private void StartCooldown()
    {
        isAvailable.Value = false;
        Invoke(nameof(ResetAvailability), cooldown);
    }

    private void ResetAvailability()
    {
        isAvailable.Value = true;
    }

    private void UpdateVisual(bool available)
    {
        if (availableSignGOj != null)
            availableSignGOj.SetActive(available);
    }

    
}
