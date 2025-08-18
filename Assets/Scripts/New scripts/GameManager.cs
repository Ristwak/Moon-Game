using UnityEngine;
using TMPro; // TextMeshPro for UI
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Rules")]
    [Tooltip("If 0 or less, will auto-detect how many CollectAfterGrab items exist in the scene at Start.")]
    public int totalToCollect = 0;
    [Tooltip("Seconds the player has to collect all items.")]
    public float timeLimit = 90f;

    [Header("UI")]
    public TextMeshProUGUI collectedText;  // e.g. "3 / 10"
    public TextMeshProUGUI timeText;       // e.g. "01:23"
    public TextMeshProUGUI resultText;     // e.g. "You Win!" / "Time Up!" (optional)

    [Header("Events")]
    public UnityEvent onVictory;
    public UnityEvent onDefeat;

    public int CollectedCount { get; private set; }
    public bool GameRunning { get; private set; }
    float _timeRemaining;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Auto-detect total if not set
        if (totalToCollect <= 0)
        {
            var items = FindObjectsOfType<CollectAfterGrab>(true); // include inactive
            totalToCollect = items.Length;
        }

        CollectedCount = 0;
        _timeRemaining = Mathf.Max(0f, timeLimit);
        GameRunning = true;

        UpdateCollectedUI();
        UpdateTimeUI();
        if (resultText) resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!GameRunning) return;

        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining <= 0f)
        {
            _timeRemaining = 0f;
            UpdateTimeUI();
            // If time ran out and not all collected -> defeat
            if (CollectedCount < totalToCollect) EndDefeat();
            // (If player collected last item exactly as time hit zero,
            // AddCollected() will already have triggered victory.)
            return;
        }

        UpdateTimeUI();
    }

    public void AddCollected(string itemId)
    {
        if (!GameRunning) return;

        CollectedCount++;
        // Debug.Log($"Collected: {itemId} | Total: {CollectedCount}/{totalToCollect}");
        UpdateCollectedUI();

        if (CollectedCount >= totalToCollect)
        {
            EndVictory();
        }
    }

    void EndVictory()
    {
        if (!GameRunning) return;
        GameRunning = false;
        if (resultText)
        {
            resultText.text = "You Win!";
            resultText.gameObject.SetActive(true);
        }
        onVictory?.Invoke();
        // TODO: load next scene, show summary, etc.
    }

    void EndDefeat()
    {
        if (!GameRunning) return;
        GameRunning = false;
        if (resultText)
        {
            resultText.text = "Time Up!";
            resultText.gameObject.SetActive(true);
        }
        onDefeat?.Invoke();
        // TODO: restart, show retry UI, etc.
    }

    void UpdateCollectedUI()
    {
        if (collectedText)
            collectedText.text = $"{CollectedCount} / {totalToCollect}";
    }

    void UpdateTimeUI()
    {
        if (!timeText) return;
        int secs = Mathf.CeilToInt(_timeRemaining);
        int m = secs / 60;
        int s = secs % 60;
        timeText.text = $"{m:00}:{s:00}";
    }
}
