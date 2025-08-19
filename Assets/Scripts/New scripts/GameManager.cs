using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;  // For scene reloading

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Controls")]
    public GameObject gestureDetection; // Drag your GestureDetection script here

    [Header("Game Rules")]
    public int totalToCollect = 0;
    public float timeLimit = 90f;

    [Header("UI")]
    public TextMeshProUGUI collectedText;  // e.g. "3 / 10"
    public TextMeshProUGUI timeText;       // e.g. "01:23"
    public TextMeshProUGUI resultText;     // e.g. "You Win!" / "Time Up!" (optional)
    public GameObject gameOverPanel;       // Reference to the Game Over Panel
    public GameObject aboutPanel;
    public GameObject startPanel;          // Reference to the Game Start Panel
    public GameObject gameUI;              // Reference to the gameplay UI (collectedText, timeText, etc.)

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
        // Initially deactivate gameplay elements
        if (gameUI) gameUI.SetActive(false);
        if (aboutPanel) aboutPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (startPanel) startPanel.SetActive(true); // Show start panel initially

        // Disable player controls while in menus
        if (gestureDetection) gestureDetection.SetActive(false);

        // Auto-detect total if not set
        if (totalToCollect <= 0)
        {
            var items = FindObjectsOfType<CollectAfterGrab>(true);
            totalToCollect = items.Length;
        }
    }

    public void StartGame()
    {
        if (startPanel) startPanel.SetActive(false);
        if (aboutPanel) aboutPanel.SetActive(false);
        if (gameUI) gameUI.SetActive(true);

        // Enable player controls
        if (gestureDetection) gestureDetection.SetActive(true);

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
            if (CollectedCount < totalToCollect) EndDefeat();
            return;
        }

        UpdateTimeUI();
    }

    public void AddCollected(string itemId)
    {
        if (!GameRunning) return;

        CollectedCount++;
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

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
        }

        onVictory?.Invoke();
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

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
        }

        onDefeat?.Invoke();
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

    public void AboutGame()
    {
        aboutPanel.SetActive(true);
        startPanel.SetActive(false);
        gameUI.SetActive(false);
        gameOverPanel.SetActive(false);

        // Disable player controls
        if (gestureDetection) gestureDetection.SetActive(false);
    }

    public void OnBack()
    {
        aboutPanel.SetActive(false);
        startPanel.SetActive(true);
        gameUI.SetActive(false);
        gameOverPanel.SetActive(false);

        // Disable player controls until StartGame is pressed
        if (gestureDetection) gestureDetection.SetActive(false);
    }

    // Restart Game or Reload Scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
