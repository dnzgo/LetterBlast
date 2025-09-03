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
        UIManager.Instance.ShowGameHUD();
        GameManager.Instance.RestartGame();
    }
}
