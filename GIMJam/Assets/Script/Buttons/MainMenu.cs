using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public void ContinueGame()
    {
        Debug.Log("Continue");
        if (!PlayerPrefs.HasKey("SavedLevel"))
        {
            Debug.Log("[MainMenu] no save so new game");
            NewGame();
            return;
        }

        string levelName = PlayerPrefs.GetString("SavedLevel");
        Debug.Log($"[MainMenu] continye {levelName}");

        // wait for 1.5f second before load scene
        StartCoroutine(LoadLevelAfterDelay(levelName, 1.5f));
    }

    public void NewGame()
    {
        Debug.Log("NewGame");
        Debug.Log("[MainMenu]new game");

        PlayerPrefs.DeleteAll();
        //ini aku ganti sama yang bawah soalnya dia bakal reset setting sound juga
        // PlayerPrefs.DeleteKey("SavedLevel");

        StartCoroutine(LoadLevelAfterDelay("Prologue", 1.5f));
    }

    IEnumerator LoadLevelAfterDelay(string levelName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenu] I QUIT");
        Application.Quit();
    }
}
