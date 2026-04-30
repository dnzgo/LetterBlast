using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum GameOverReason
    {
        None,
        TimeUp,
        Blocked
    }

    public static GameManager Instance;
    public Spawner spawner;
    public GridManager gridManager;
    public int score = 0;
    public int bestScore = 0;

    [Header("Combo Settings")]
    public int multiClearBonusPerStructure = 100;
    public int streakBonusPerStep = 200;
    public int comboStreak = 0;
    public float timeBonus = 2.0f;

    public int maxRewardOffersPerGame = 3;
    private int currentRewardOffers = 0;
    private bool isPaused = false;
    private bool isGameStarted = false;
    private GameOverReason lastGameOverReason = GameOverReason.None;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        UIManager.Instance.UpdateBestScoreUI(bestScore);
        UIManager.Instance.UpdateScoreUI(score);
        UIManager.Instance.ShowMainMenu();
    }

    public void AddScore(int amount, Vector3 worldPos, bool isBonus)
    {
        score += amount;
        if (UIManager.Instance != null)
            UIManager.Instance.SpawnFloatingScore(amount, worldPos, isBonus);
        if (score > bestScore)
        {
            bestScore = score;
            UIManager.Instance.UpdateBestScoreUI(bestScore);
        }
        UIManager.Instance.UpdateScoreUI(score);
    }

    public void CheckGameOver(List<GameObject> currentLetters)
    {

        bool canPlay = false;

        foreach (var letterObj in currentLetters)
        {
            if (letterObj == null) continue;

            DragDrop letter = letterObj.GetComponent<DragDrop>();
            if (letter != null && gridManager.CanPlaceLetter(letter))
            {
                canPlay = true;
                break;
            }
        }

        if (!canPlay)
        {
            TriggerGameOver(GameOverReason.Blocked);
        }
    }

    public void TriggerGameOver(GameOverReason reason)
    {
        lastGameOverReason = reason;

        if (CanOfferRewarded())
        {
            // Freeze game and timer while the rewarded offer panel is shown.
            Time.timeScale = 0f;
            TimeManager.Instance.PauseTimer();
            UIManager.Instance.ShowRewardedPanel();
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("---Game Over---");
        // stop timer
        TimeManager.Instance.StopTimer();

        isGameStarted = false;
        Time.timeScale = 0f;

        // Best score save
        PlayerPrefs.SetInt("BestScore", bestScore);
        PlayerPrefs.Save();

        foreach (Transform child in UIManager.Instance.gameHUDTransform)
        {
            if (child.CompareTag("FloatingScore"))
                Destroy(child.gameObject);
        }

        UIManager.Instance.ShowGameOver();
        UIManager.Instance.UpdateGameOverScores(score, bestScore);

    }

    public void StartGame()
    {
        Debug.Log("Game Started!");
        isGameStarted = true;
        isPaused = false;
        lastGameOverReason = GameOverReason.None;
        Time.timeScale = 1f;
        // reset score and rewardedAd count
        score = 0;
        currentRewardOffers = 0;
        spawner.SpawnBatch();
        UIManager.Instance.UpdateScoreUI(score);
        // start timer
        TimeManager.Instance.StartTimer();
    }
    public void PauseGame()
    {
        if (!isGameStarted) return;
        Debug.Log("Game Paused!");
        isPaused = true;
        Time.timeScale = 0f;

        TimeManager.Instance.PauseTimer();
    }
    public void ResumeGame()
    {
        if (!isGameStarted) return;
        Debug.Log("Game Resumed!");
        isPaused = false;
        Time.timeScale = 1f;
        TimeManager.Instance.ResumeTimer();
    }
    public void RestartGame()
    {
        isPaused = false;
        isGameStarted = true;
        lastGameOverReason = GameOverReason.None;
        Time.timeScale = 1f;
        // reset score and rewardedAd count
        score = 0;
        currentRewardOffers = 0;
        UIManager.Instance.UpdateScoreUI(score);
        // start timer
        TimeManager.Instance.StartTimer();

        // grid reset
        if (gridManager != null)
        {
            gridManager.ClearGrid();
        }

        // spawner reset
        if (spawner != null)
        {
            spawner.ClearLetters();
            spawner.SpawnBatch();
        }
    }
    public void ReturnToMainMenu()
    {
        isGameStarted = false;
        isPaused = false;
        lastGameOverReason = GameOverReason.None;

        gridManager.ClearGrid();
        spawner.ClearLetters();
        score = 0;
    }
    // Reward Limit control
    public bool CanOfferRewarded()
    {
        return currentRewardOffers < maxRewardOffersPerGame;
    }

    public void ConsumeRewardOffer()
    {
        currentRewardOffers++;
    }

    public float GetRewardedTimeForLastGameOver()
    {
        switch (lastGameOverReason)
        {
            case GameOverReason.TimeUp:
                return 45f;
            case GameOverReason.Blocked:
                return 15f;
            default:
                return 0f;
        }
    }

}
