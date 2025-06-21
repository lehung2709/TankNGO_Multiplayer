using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private Collider2D playerCollider;

    private PlayerStats playerStats;


    private float startShootTime = 0f;
    private float muzzleDuration = 0.1f;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerStats = GetComponent<PlayerStats>();
    }

    private float preFireTime = 0;

    private void Update()
    {
        if(muzzleFlash != null && muzzleFlash.activeSelf)
        {
            if(startShootTime+muzzleDuration < Time.time) muzzleFlash.SetActive(false);
        }
        if(!IsOwner) return;
        if (!inputReader.PrimaryFireInput) return;      
        if (Time.time < (1 / playerStats.fireRate.Value) + preFireTime) return;

        ShootProjectileServerRpc(shootPoint.position,shootPoint.up);
        ShootDummyProjectile(shootPoint.position,shootPoint.up);

        preFireTime = Time.time;

    }

    private void ShootDummyProjectile(Vector2 pos, Vector2 direction)
    {
        startShootTime=Time.time;
        muzzleFlash.SetActive(true);
        AudioManager.Instance.SpawnSoundEmitter(null, "Shoot", transform.position);
        GameObject projectileInstance = ProjectilePool.Instance.GetClientProjectile();
        projectileInstance.transform.position = pos;
        projectileInstance.transform.up = direction;

        if (projectileInstance.TryGetComponent<Projectile>(out Projectile proj))
        {
            proj.SetOwner(OwnerClientId);
        }


        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = direction.normalized*playerStats.projectileSpeed;
        }

    }

    [ServerRpc]
    private void ShootProjectileServerRpc(Vector2 pos, Vector2 direction)
    {
        GameObject projectileInstance = ProjectilePool.Instance.GetServerProjectile();
        projectileInstance.transform.position = pos;
        projectileInstance.transform.up = direction;

        if (projectileInstance.TryGetComponent<Projectile>(out Projectile proj))
        {
            proj.SetOwner(OwnerClientId);
        }

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = direction.normalized * playerStats.projectileSpeed;
        }

        ShootDummyProjectileClientRpc(pos,direction);
    }


    [ClientRpc]
    private void ShootDummyProjectileClientRpc(Vector2 pos, Vector2 direction)
    {
        if (IsOwner) return;

        ShootDummyProjectile(pos,direction);
    }

}
