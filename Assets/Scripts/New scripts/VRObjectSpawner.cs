using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class VRObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectPrefab;      // Prefab of grabbable object
    public int totalObjects = 10;        // Total number of objects to spawn
    public Vector3 spawnArea = new Vector3(5, 2, 5); // Random area size

    [Header("Game Settings")]
    public float gameDuration = 30f;     // Timer in seconds
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI counterText;

    private int pickedCount = 0;
    private float timeRemaining;
    private bool gameRunning = false;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        timeRemaining = gameDuration;
        pickedCount = 0;
        gameRunning = true;

        SpawnObjects();
        UpdateUI();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < totalObjects; i++)
        {
            Vector3 randomPos = transform.position +
                new Vector3(Random.Range(-spawnArea.x, spawnArea.x),
                            Random.Range(0, spawnArea.y),
                            Random.Range(-spawnArea.z, spawnArea.z));

            GameObject obj = Instantiate(objectPrefab, randomPos, Quaternion.identity);
            obj.AddComponent<XRGrabInteractable>(); // makes it pinch-grabbable
            obj.AddComponent<VRPickable>();         // custom script to track pick
        }
    }

    void Update()
    {
        if (!gameRunning) return;

        timeRemaining -= Time.deltaTime;
        UpdateUI();

        if (timeRemaining <= 0f)
        {
            EndGame();
        }
    }

    public void ObjectPicked()
    {
        pickedCount++;
        UpdateUI();

        if (pickedCount >= totalObjects)
        {
            EndGame();
        }
    }

    void UpdateUI()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();

        if (counterText != null)
            counterText.text = "Picked: " + pickedCount + "/" + totalObjects;
    }

    void EndGame()
    {
        gameRunning = false;
        Debug.Log("Game Over! Picked " + pickedCount + " objects.");
    }
}
