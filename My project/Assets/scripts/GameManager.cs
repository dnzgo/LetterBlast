using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Spawner spawner;
    public GridManager gridManager;

    public int score = 0;
    public int bestScore = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

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
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = score.ToString();
        if (bestScoreText != null) bestScoreText.text = bestScore.ToString();
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

        // bestScore check and save
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
            PlayerPrefs.Save();
        }

        // TODO: some game over ui etc.

    }


}
