using UnityEngine;

public class ClientProjectile : Projectile
{
    [SerializeField] private GameObject dustCloud;
    protected override void HandleContact(Collider2D collision)
    {
        Return2Pool();
    }

    protected override void Return2Pool()
    {
        Instantiate(dustCloud, transform.position, Quaternion.identity);
        ProjectilePool.Instance.DespawnClientProjectile(this.gameObject);
    }
}
