using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    public int collectedCount { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Add(string itemId)
    {
        collectedCount++;
        // TODO: track by itemId, update UI, play SFX, etc.
        Debug.Log($"Collected: {itemId} | Total: {collectedCount}");
    }
}
