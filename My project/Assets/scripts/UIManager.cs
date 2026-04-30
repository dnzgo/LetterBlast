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
    public GameObject rewardedAdOfferPanel;
    public GameObject noAdsOfferPanel;

    [Header("Game HUD UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI comboText;
    public GameObject floatingScorePrefab;
    public GameObject floatingTimePrefab;
    public int floatingTimePoolSize = 3;
    public float floatingTimeUnderOffset = -60f;
    public float floatingTimeRandomX = 36f;
    public float floatingTimeRandomY = 16f;
    public float floatingTimeMaxStartDelay = 0.08f;
    public float floatingTimeCurveAmount = 20f;
    public float floatingTimeTargetRandomX = 8f;
    public Transform gameHUDTransform;
    private GameObject[] floatingTimePool;
    private Coroutine[] floatingTimeRoutines;
    private int floatingTimeNextIndex = 0;

    [Header("GameOver UI")]
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI gameOverBestScore;

    [Header("IAP UI")]
    public TextMeshProUGUI priceOfNoAds;

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
        DragDrop.dragEnabled = true;
        AdManager.Instance.ShowAdaptiveBanner();
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
    public void ShowRewardedPanel()
    {
        HideAll();
        rewardedAdOfferPanel.SetActive(true);
    }

    public void ShowNoAdsOfferPanel()
    {
        HideAll();
        noAdsOfferPanel.SetActive(true);
    }
    private void HideAll()
    {
        mainMenuPanel.SetActive(false);
        gameHUDPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        rewardedAdOfferPanel.SetActive(false);
        noAdsOfferPanel.SetActive(false);
        DragDrop.dragEnabled = false;
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

    public void SpawnFloatingTime(float seconds)
    {
        if (floatingTimePrefab == null || gameHUDTransform == null) return;
        if (TimeManager.Instance == null || TimeManager.Instance.timerText == null) return;

        EnsureFloatingTimePool();

        int index = floatingTimeNextIndex;
        floatingTimeNextIndex = (floatingTimeNextIndex + 1) % floatingTimePool.Length;

        if (floatingTimeRoutines[index] != null)
            StopCoroutine(floatingTimeRoutines[index]);

        GameObject obj = floatingTimePool[index];
        RectTransform objRect = obj.GetComponent<RectTransform>();
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        RectTransform timerRect = TimeManager.Instance.timerText.rectTransform;

        if (objRect == null || tmp == null || timerRect == null) return;

        obj.SetActive(true);

        tmp.text = $"+{Mathf.RoundToInt(seconds)}s";
        tmp.color = new Color(0.4f, 1f, 0.4f, 1f);

        float randomX = Random.Range(-floatingTimeRandomX, floatingTimeRandomX);
        float randomY = Random.Range(-floatingTimeRandomY, floatingTimeRandomY);
        Vector3 startPos = timerRect.position + new Vector3(randomX, floatingTimeUnderOffset + randomY, 0f);
        float targetRandomX = Random.Range(-floatingTimeTargetRandomX, floatingTimeTargetRandomX);
        Vector3 targetPos = timerRect.position + new Vector3(targetRandomX, 0f, 0f);
        float startDelay = Random.Range(0f, floatingTimeMaxStartDelay);
        float curveDirection = Mathf.Sign(Random.Range(-1f, 1f));
        if (Mathf.Approximately(curveDirection, 0f))
            curveDirection = 1f;
        float curveAmount = Random.Range(floatingTimeCurveAmount * 0.6f, floatingTimeCurveAmount) * curveDirection;

        objRect.position = startPos;
        objRect.localScale = Vector3.one;

        floatingTimeRoutines[index] = StartCoroutine(FloatingTimeRoutine(index, objRect, tmp, startPos, targetPos, startDelay, curveAmount));
    }

    private void EnsureFloatingTimePool()
    {
        if (floatingTimePool != null && floatingTimePool.Length == floatingTimePoolSize && floatingTimePoolSize > 0)
            return;

        if (floatingTimePoolSize < 1)
            floatingTimePoolSize = 1;

        floatingTimePool = new GameObject[floatingTimePoolSize];
        floatingTimeRoutines = new Coroutine[floatingTimePoolSize];
        floatingTimeNextIndex = 0;

        for (int i = 0; i < floatingTimePoolSize; i++)
        {
            GameObject obj = Instantiate(floatingTimePrefab, gameHUDTransform);
            obj.SetActive(false);
            floatingTimePool[i] = obj;
        }
    }

    private IEnumerator FloatingScoreRoutine(Transform obj, TextMeshProUGUI tmp, bool isBonus)
    {
        Vector3 targetPos = scoreText.transform.position; // score text UI position

        float t = 0;
        float duration = isBonus ? 1f : 0.6f;
        Vector3 startPos = obj.position;

        while (t < duration)
        {
            if (obj == null) yield break; // obj destroyed → stop coroutine

            t += Time.deltaTime;
            float normalized = t / duration;

            obj.position = Vector3.Lerp(startPos, targetPos, normalized);
            tmp.alpha = 1f - normalized; // slowly disappear
            yield return null;
        }

        Destroy(obj.gameObject);
    }

    private IEnumerator FloatingTimeRoutine(int index, RectTransform obj, TextMeshProUGUI tmp, Vector3 startPos, Vector3 targetPos, float startDelay, float curveAmount)
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        float duration = 0.45f;
        float t = 0f;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.one * 0.85f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            float eased = 1f - Mathf.Pow(1f - normalized, 3f);

            Vector3 straightPos = Vector3.Lerp(startPos, targetPos, eased);
            float arc = Mathf.Sin(normalized * Mathf.PI) * curveAmount;
            obj.position = straightPos + new Vector3(arc, 0f, 0f);
            obj.localScale = Vector3.Lerp(startScale, endScale, eased);

            Color c = tmp.color;
            c.a = 1f - normalized;
            tmp.color = c;

            yield return null;
        }

        obj.gameObject.SetActive(false);
        floatingTimeRoutines[index] = null;
    }


}
