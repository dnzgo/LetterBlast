using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Spawner spawner;
    public GridManager gridManager;

    private bool isPaused = false;
    private bool isGameStarted = false;

    public int score = 0;
    public int bestScore = 0;

    [Header("Combo Settings")]
    public int multiClearBonusPerStructure = 50;
    public int streakBonusPerStep = 20;
    public int comboStreak = 0;

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

    public void AddScore(int amount)
    {
        score += amount;
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
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("---Game Over---");

        isGameStarted = false;
        Time.timeScale = 0f;

        // Best score save
        PlayerPrefs.SetInt("BestScore", bestScore);
        PlayerPrefs.Save();

        UIManager.Instance.ShowGameOver();

    }

    public void StartGame()
    {
        Debug.Log("Game Started!");
        isGameStarted = true;
        isPaused = false;
        Time.timeScale = 1f;
        score = 0;
        UIManager.Instance.UpdateScoreUI(score);
    }
    public void PauseGame()
    {
        if (!isGameStarted) return;
        Debug.Log("Game Paused!");
        isPaused = true;
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        if (!isGameStarted) return;
        Debug.Log("Game Resumed!");
        isPaused = false;
        Time.timeScale = 1f;
    }
    public void RestartGame()
    {
        isPaused = false;
        isGameStarted = true;
        Time.timeScale = 1f;
        // score reset
        score = 0;
        UIManager.Instance.UpdateScoreUI(score);

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

        // incativate gameOver panel and activate GameHUD
        UIManager.Instance.ShowGameHUD();
    }

}
