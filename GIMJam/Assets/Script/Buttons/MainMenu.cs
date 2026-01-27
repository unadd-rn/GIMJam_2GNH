using UnityEngine;
using UnityEngine.SceneManagement;

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

        SceneManager.LoadScene(levelName);
    }

    public void NewGame()
    {
        Debug.Log("NewGame");
        Debug.Log("[MainMenu]new game");

        //PlayerPrefs.DeleteAll();
        //ini aku ganti sama yang bawah soalnya dia bakal reset setting sound juga
        PlayerPrefs.DeleteKey("SavedLevel");
        SceneManager.LoadScene("Prologue");
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenu] I QUIT");
        Application.Quit();
    }
}
