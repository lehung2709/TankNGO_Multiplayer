using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerProjectile : Projectile
{
    [SerializeField] private int damage;
    protected override void HandleContact(Collider2D collision)
    {
        
        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
            if(health.IsDead)
            {
                Leaderboard.Instance.AddScores(ownerClientId, 1);
            }    
            Return2Pool();
        }

    }

    protected override void Return2Pool()
    {
        ProjectilePool.Instance.DespawmServerProjectile(this.gameObject);
    }
}
