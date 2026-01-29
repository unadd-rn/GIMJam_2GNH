using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // stop music
        MusicManager.Instance.PlayMusic("Credit BG");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainMenu()
    {
        GameObject.Find("Scene Transition").GetComponent<Animator>().SetTrigger("End");

        StartCoroutine(LoadAfterDelay("MainMenu", 1.5f));
    }

    private IEnumerator LoadAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}