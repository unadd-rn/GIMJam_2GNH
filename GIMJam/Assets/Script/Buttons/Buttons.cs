using UnityEngine.SceneManagement;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public GameObject pauseUI;
    private bool isPaused = false;

    void Start()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        Debug.Log("[PauseMenu] Game paused");

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseUI != null)
            pauseUI.SetActive(true);
    }

    public void Resume()
    {
        Debug.Log("[PauseMenu] Game resumed");

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseUI != null)
            pauseUI.SetActive(false);
    }

    public void BackToMainMenu()
    {
        Debug.Log("[PauseMenu] Back to main menu");

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Respawan()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentScene.name);
    }
}
