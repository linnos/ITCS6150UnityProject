using UnityEngine;

public class LevelUI : MonoBehaviour
{
    [SerializeField] GameObject levelCompletePanel;
    [SerializeField] GameObject pausePanel;

    static LevelUI instance;

    void Awake() { instance = this; HideAll(); }

    void HideAll()
    {
        if (levelCompletePanel) levelCompletePanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowLevelComplete()
    {
        if (levelCompletePanel) levelCompletePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public static void TogglePause()
    {
        if (!instance || !instance.pausePanel) return;
        bool show = !instance.pausePanel.activeSelf;
        instance.pausePanel.SetActive(show);
        Time.timeScale = show ? 0f : 1f;
    }

    // Button hooks
    public void OnRestart() { Time.timeScale = 1f; LevelState.ResetLevel(); }
    public void OnNextLevel() { Time.timeScale = 1f; LevelState.NextLevel(); }
    public void OnResetToStart() { LevelState.Respawn(); }
    public void OnMarkUnplayable() { LevelState.MarkUnplayable(); }
}
