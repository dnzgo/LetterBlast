using UnityEngine;
using TMPro;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public float startTime = 180f;
    public float maxTime = 240f;

    private float currentTime;
    private bool isRunning = false;

    public TextMeshProUGUI timerText;

    private bool isGameOverTriggered = false;

    Coroutine colorRoutine;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;

            if (!isGameOverTriggered)
            {
                isGameOverTriggered = true;
                GameManager.Instance.TriggerGameOver(GameManager.GameOverReason.TimeUp);
            }

            isRunning = false;
        }

        UpdateTimerUI();
    }

    public void StartTimer()
    {
        currentTime = startTime;
        isRunning = true;
        isGameOverTriggered = false;
        UpdateTimerUI();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void AddTime(float amount)
    {
        if (!isRunning) return;

        currentTime += amount;
        currentTime = Mathf.Min(currentTime, maxTime);

        if (colorRoutine != null)
            StopCoroutine(colorRoutine);

        colorRoutine = StartCoroutine(FlashGreen());

        UpdateTimerUI();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SpawnFloatingTime(amount);
        }
    }
    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
        isGameOverTriggered = false;
    }

    public void SetTime(float time)
    {
        currentTime = time;
        currentTime = Mathf.Min(currentTime, maxTime);

        isRunning = true;
        isGameOverTriggered = false;

        UpdateTimerUI();
    }

    void ResetColor()
    {
        timerText.color = Color.white; // default color
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:0}:{seconds:00}";
    }

    IEnumerator FlashGreen()
    {
        timerText.color = Color.green;
        yield return new WaitForSeconds(0.2f);
        ResetColor();
    }
}