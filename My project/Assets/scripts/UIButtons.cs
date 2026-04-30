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
                // Continue with 3 reward letters + time bonus based on game-over reason.
                GameManager.Instance.ConsumeRewardOffer();
                UIManager.Instance.ShowGameHUD();
                GameManager.Instance.ResumeGame();

                float rewardedTime = GameManager.Instance.GetRewardedTimeForLastGameOver();
                if (rewardedTime > 0f)
                {
                    TimeManager.Instance.AddTime(rewardedTime);
                }

                Spawner spawner = FindFirstObjectByType<Spawner>();
                if (spawner != null)
                    spawner.SpawnRewardLetters();
            });
    }

    public void OnNoThanksButton()
    {
        GameManager.Instance.GameOver();
    }

    public void OnNoAdsButton()
    {
        UIManager.Instance.ShowNoAdsOfferPanel();
        GameManager.Instance.PauseGame();
    }

}
