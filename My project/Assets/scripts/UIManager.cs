using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameHUDPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Texts")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }
    public void UpdateBestScoreUI(int bestScore)
    {
        if (bestScoreText != null) bestScoreText.text = bestScore.ToString();
    }

    public void ShowMainMenu()
    {
        HideAll();
        mainMenuPanel.SetActive(true);
    }
    public void ShowGameHUD()
    {
        HideAll();
        gameHUDPanel.SetActive(true);
    }
    public void ShowPause()
    {
        HideAll();
        pausePanel.SetActive(true);
    }
    public void ShowGameOver()
    {
        HideAll();
        gameOverPanel.SetActive(true);
    }

    private void HideAll()
    {
        mainMenuPanel.SetActive(false);
        gameHUDPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }
}
