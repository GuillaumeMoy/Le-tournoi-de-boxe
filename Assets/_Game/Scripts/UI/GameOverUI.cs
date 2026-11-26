
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMPro.TMP_Text subtitleText;

    // Set by the story (steroids vs the final boss); replaces the subtitle when you lose
    public static string NextSubtitle;
    private string defaultSubtitle;

    void Start()
    {
        gameOverPanel.SetActive(false);
        if (subtitleText != null)
            defaultSubtitle = subtitleText.text;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        if (subtitleText != null)
            subtitleText.text = string.IsNullOrEmpty(NextSubtitle) ? defaultSubtitle : NextSubtitle;

        // Pause the game
        Time.timeScale = 0f;
    }

    public void TryAgain()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Restart the current level
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void MainMenu()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Load the Main Menu scene
        SceneManager.LoadScene("MainMenu");
    }
}
