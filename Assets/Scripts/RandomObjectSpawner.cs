using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] objectsToSpawn; // Prefabs to spawn
    public int spawnCount = 10; // How many objects to spawn
    public Vector3 spawnAreaCenter; // Center of spawn area
    public Vector3 spawnAreaSize; // Size of spawn area

    [Header("Optional")]
    public bool randomRotation = true;
    public bool randomScale = false;
    public Vector2 scaleRange = new Vector2(0.5f, 1.5f);

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // Random position within defined area
            Vector3 randomPos = spawnAreaCenter + new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            // Pick a random prefab
            GameObject prefab = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];

            // Create object
            GameObject obj = Instantiate(prefab, randomPos, Quaternion.identity);

            // Random rotation
            if (randomRotation)
            {
                obj.transform.rotation = Random.rotation;
            }

            // Random scale
            if (randomScale)
            {
                float scale = Random.Range(scaleRange.x, scaleRange.y);
                obj.transform.localScale = Vector3.one * scale;
            }
        }
    }

    // Draw spawn area in Scene View
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
