using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

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
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     if (isPaused)
        //         Resume();
        //     else
        //         Pause();
        // }
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
        GameObject.Find("Scene Transition").GetComponent<Animator>().SetTrigger("End");

        StartCoroutine(LoadAfterDelay("MainMenu", 1.5f));
    }

    public void Respawan()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
        GameObject.Find("Scene Transition").GetComponent<Animator>().SetTrigger("End");
        StartCoroutine(LoadAfterDelay(currentScene, 1.5f));

    }

    IEnumerator LoadAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
