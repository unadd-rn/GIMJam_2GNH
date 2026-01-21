using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void goToSceneName(string name)
    {

        SceneManager.LoadSceneAsync(name);
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class SceneController : MonoBehaviour
{
    public int levelIndex;
    public static SceneController Instance;

    public Animator TransitionAnim;

    string TransitionMessage = "NextSceneTransition";

    float delay = 1.25f;
    int sceneIndex;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }
    public void nextScene()
    {
        ManageLoadScene(1.25f, SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMap()
    {
        UnlockNewLevel();

        TransitionMessage = "NextSceneTransitionDelay";
        // build index map = 1
        ManageLoadScene(3f, 1);
    }
    void UnlockNewLevel()
    {
        int currentIndex = levelIndex;
        int maxLevelIndex = 7; // INII karena levelnya masih 3
        
        if (currentIndex > PlayerPrefs.GetInt("ReachedIndex", 0) && currentIndex < maxLevelIndex)
        {
            PlayerPrefs.SetInt("ReachedIndex", currentIndex);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();

            Debug.Log($"Progress Unlocked! New UnlockedLevel: {PlayerPrefs.GetInt("UnlockedLevel")}");
        }
    }
    //dah


    public void loadScene(int _sceneIndex)
    {
        ManageLoadScene(1.25f, _sceneIndex);
    }

    public void resetScene()
    {
        SoundManager.Instance.PlaySound2D("Button Click");
        TransitionMessage = "NextSceneTransition";
        ManageLoadScene(1.25f, SceneManager.GetActiveScene().buildIndex);
    }

    public void returnToMenu()
    {
        SoundManager.Instance.PlaySound2D("Button Click");
        TransitionMessage = "NextSceneTransition";
        StartCoroutine(WaitForDelayMenu());
    }   

    IEnumerator WaitForDelayMenu()
    {
        yield return new WaitForSeconds(0f);
        ManageLoadScene(1.25f, 0);
    }
    public void ManageLoadScene(float _delay, int _sceneIndex)
    {
        delay = _delay;
        sceneIndex = _sceneIndex;
        StartCoroutine(WaitForDelay());
    }
    IEnumerator WaitForDelay()
    {
        TransitionAnim.SetTrigger(TransitionMessage);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneIndex);
    }
}
*/