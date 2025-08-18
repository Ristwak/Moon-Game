using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class RandomObjectSpawnerVR : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] objectsToSpawn;
    public int spawnCount = 10;

    [Header("Map Borders")]
    public Transform border1; // minX
    public Transform border2; // maxX
    public Transform border3; // minZ
    public Transform border4; // maxZ

    [Header("Options")]
    public bool randomRotation = true;
    public bool randomScale = false;
    public Vector2 scaleRange = new Vector2(0.5f, 1.5f);
    public float minDistanceBetweenObjects = 2f; // Prevent clumping

    [Header("Gameplay")]
    public float timeLimit = 60f; // seconds
    public TMP_Text timerText;
    public TMP_Text counterText;
    public TMP_Text statusText;

    private float timer;
    private int collectedCount = 0;
    private List<Vector3> spawnedPositions = new List<Vector3>();
    private bool gameRunning = true;

    void Start()
    {
        timer = timeLimit;
        SpawnObjects();
        UpdateUI();
    }

    void Update()
    {
        if (!gameRunning) return;

        // Countdown timer
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            LoseGame();
        }

        UpdateUI();
    }

    void SpawnObjects()
    {
        if (objectsToSpawn.Length == 0)
        {
            Debug.LogError("[Spawner] No objects assigned to spawn!");
            return;
        }

        int spawned = 0;
        int attempts = 0;

        while (spawned < spawnCount && attempts < spawnCount * 10)
        {
            attempts++;

            float randomX = Mathf.Lerp(border1.position.x, border2.position.x,
                Mathf.PerlinNoise(Random.value * 10f, Time.time));
            float randomZ = Mathf.Lerp(border3.position.z, border4.position.z,
                Mathf.PerlinNoise(Time.time, Random.value * 10f));

            randomX += Random.Range(-3f, 3f);
            randomZ += Random.Range(-3f, 3f);

            Vector3 spawnPos = new Vector3(randomX, Random.Range(40f, 60f), randomZ);

            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 100f))
            {
                bool tooClose = false;
                foreach (var pos in spawnedPositions)
                {
                    if (Vector3.Distance(pos, hit.point) < minDistanceBetweenObjects)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose) continue;

                GameObject prefab = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
                GameObject obj = Instantiate(prefab, hit.point, Quaternion.identity);

                if (randomRotation)
                    obj.transform.rotation = Random.rotation;

                if (randomScale)
                {
                    float scale = Random.Range(scaleRange.x, scaleRange.y);
                    obj.transform.localScale = Vector3.one * scale;
                }

                // Make sure it can be grabbed in VR
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb == null) rb = obj.AddComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;

                XRGrabInteractable grab = obj.GetComponent<XRGrabInteractable>();
                if (grab == null) grab = obj.AddComponent<XRGrabInteractable>();

                // Attach pickable script
                VRPickable pickable = obj.AddComponent<VRPickable>();
                pickable.spawner = this;

                spawnedPositions.Add(hit.point);
                spawned++;
            }
        }

        Debug.Log($"[Spawner] Spawned {spawned}/{spawnCount} objects.");
    }

    public void ObjectCollected(GameObject obj)
    {
        collectedCount++;
        Destroy(obj);

        Debug.Log($"✅ Collected {collectedCount}/{spawnCount}");

        if (collectedCount >= spawnCount)
        {
            WinGame();
        }
    }

    void UpdateUI()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString();

        if (counterText != null)
            counterText.text = "Collected: " + collectedCount + "/" + spawnCount;
    }

    void WinGame()
    {
        gameRunning = false;
        if (statusText != null) statusText.text = "🎉 You Win!";
        Debug.Log("Player wins!");
    }

    void LoseGame()
    {
        gameRunning = false;
        if (statusText != null) statusText.text = "⏳ Time’s Up! You Lose!";
        Debug.Log("Player loses!");
    }
}
