using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerVR : MonoBehaviour
{
    public static GameManagerVR Instance;

    [Header("Gameplay")]
    public int totalObjectsToCollect;   // set this when spawning objects
    private int collectedCount = 0;

    [Header("Timer")]
    public float timeLimit = 60f;  // seconds
    private float timer;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text statusText;

    private bool gameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        timer = timeLimit;
        UpdateUI();
    }

    private void Update()
    {
        if (gameOver) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            LoseGame();
        }
        UpdateUI();
    }

    public void ObjectCollected()
    {
        collectedCount++;
        if (collectedCount >= totalObjectsToCollect)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        gameOver = true;
        statusText.text = "🎉 You Win!";
        Debug.Log("Player wins!");
    }

    private void LoseGame()
    {
        gameOver = true;
        statusText.text = "⏳ Time’s Up! You Lose!";
        Debug.Log("Player loses!");
    }

    private void UpdateUI()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString();

        if (statusText != null && !gameOver)
            statusText.text = "Collected: " + collectedCount + "/" + totalObjectsToCollect;
    }
}
