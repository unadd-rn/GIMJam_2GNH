using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ContinueGame()
    {
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
        Debug.Log("[MainMenu]new game");

        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Scene");
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenu] I QUIT");
        Application.Quit();
    }
}
