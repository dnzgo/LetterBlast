using UnityEngine;

public class UIButtons : MonoBehaviour
{
    public void OnPlayButton()
    {
        UIManager.Instance.ShowGameHUD();
        GameManager.Instance.StartGame();
    }
    public void OnPauseButton()
    {
        UIManager.Instance.ShowPause();
        GameManager.Instance.PauseGame();
    }
    public void OnResumeGame()
    {
        UIManager.Instance.ShowGameHUD();
        GameManager.Instance.ResumeGame();
    }
    public void OnRestartButton()
    {
        GameManager.Instance.RestartGame();
        UIManager.Instance.ShowGameHUD();
    }
    public void OnMainMenuButton()
    {
        UIManager.Instance.ShowMainMenu();
        GameManager.Instance.ReturnToMainMenu();

    }
    public void OnWatchAdButton()
    {

    AdManager.Instance.ShowRewarded(
            onReward: () =>
            {
                // Oyuna devam etmek için 3 tek hücrelik letter spawn et
                UIManager.Instance.ShowGameHUD();
                Spawner spawner = FindFirstObjectByType<Spawner>();
                if (spawner != null)
                    spawner.SpawnRewardLetters();
            });
    }

    public void OnNoThanksButton()
    {
        GameManager.Instance.GameOver();
    }

}
