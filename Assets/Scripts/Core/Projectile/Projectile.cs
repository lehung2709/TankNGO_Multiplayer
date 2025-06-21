using Unity.Netcode;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;
    protected ulong ownerClientId;
    private float spawnTime;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }
    private void OnEnable()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - spawnTime >= lifeTime)
        {
            Return2Pool();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!col.gameObject.CompareTag("Player"))
        {
            Return2Pool();
            return;
        }
        if (col.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
            HandleContact(col);
        }
    }

    protected virtual void HandleContact(Collider2D collider)
    {

    }

    protected virtual void Return2Pool()
    {

    }    


}
