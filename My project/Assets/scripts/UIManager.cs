using System.Collections;
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

    [Header("Game HUD UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI comboText;
    public GameObject floatingScorePrefab;
    public Transform gameHUDTransform;

    [Header("GameOver UI")]
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI gameOverBestScore;

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

    public void UpdateGameOverScores(int score, int bestScore)
    {
        if (gameOverScore != null)
            gameOverScore.text = score.ToString();

        if (gameOverBestScore != null)
            gameOverBestScore.text = bestScore.ToString();
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
        AdManager.Instance.ShowBannerBottom();
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
        AdManager.Instance.ShowInterstitial();
    }
    private void HideAll()
    {
        mainMenuPanel.SetActive(false);
        gameHUDPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        AdManager.Instance.HideBanner();
    }
    public void ShowCombo(string message)
    {
        //StopAllCoroutines();
        StartCoroutine(ShowComboRoutine(message));
    }

    private IEnumerator ShowComboRoutine(string message)
    {
        comboText.text = message;
        comboText.gameObject.SetActive(true);

        float duration = 0.3f; //rainbow duration
        float elapsed = 0f;

        Color[] rainbowColors = new Color[]
        {
            Color.red,
            new Color(1f, 0.5f, 0f), // orange
            Color.yellow,
            Color.green,
            Color.cyan,
            Color.blue,
            new Color(0.5f, 0f, 1f) // purple
        };

        int colorIndex = 0;

        while (elapsed < duration)
        {
            float t = (elapsed / duration) * (rainbowColors.Length - 1);
            colorIndex = Mathf.FloorToInt(t);
            int nextIndex = Mathf.Clamp(colorIndex + 1, 0, rainbowColors.Length - 1);

            comboText.color = Color.Lerp(rainbowColors[colorIndex], rainbowColors[nextIndex], t - colorIndex);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // after rainbow set to white
        comboText.color = rainbowColors[rainbowColors.Length - 1];

        yield return new WaitForSeconds(duration);
        comboText.gameObject.SetActive(false);
    }

    public void SpawnFloatingScore(int amount, Vector3 worldPos, bool isBonus)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        GameObject obj = Instantiate(floatingScorePrefab, gameHUDTransform);
        obj.transform.position = screenPos;

        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = $"+{amount}";
        tmp.color = isBonus ? Color.yellow : Color.white;

        // animation: move to scoreText
        StartCoroutine(FloatingScoreRoutine(obj.transform, tmp, isBonus));
    }
    
    private IEnumerator FloatingScoreRoutine(Transform obj, TextMeshProUGUI tmp, bool isBonus)
    {
        Vector3 targetPos = scoreText.transform.position; // score text UI position

        float t = 0;
        float duration = isBonus ? 1f : 0.6f;
        Vector3 startPos = obj.position;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            obj.position = Vector3.Lerp(startPos, targetPos, normalized);
            tmp.alpha = 1f - normalized; // slowly disappear
            yield return null;
        }

        Destroy(obj.gameObject);
    }


}
