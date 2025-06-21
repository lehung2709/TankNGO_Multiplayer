using Unity.Netcode;
using UnityEngine;

public class SpawnPointGenerator : MonoBehaviour
{
    public static SpawnPointGenerator Instance { get; private set; }

    [SerializeField] private Vector2 spawnArea = new Vector2(90f, 90f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public Vector2 GetSpawnPoint(Vector2 checkBoxSize)
    {
        Vector2 spawnPos;
        do
        {
            spawnPos = new Vector3(
               Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f),
               Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f),
               0f
           );
        }
        while (Physics2D.OverlapBox(spawnPos, checkBoxSize, 0.0f) != null);

        return spawnPos;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, (Vector3)spawnArea);
    }
}
