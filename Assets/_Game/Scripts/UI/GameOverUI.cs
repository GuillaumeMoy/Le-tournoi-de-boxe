
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

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
