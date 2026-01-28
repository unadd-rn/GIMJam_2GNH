using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("UI & Tracking")]
    public TextMeshProUGUI coinText;
    private int totalCoinsInLevel;
    private int coinsCollected = 0;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera vcam;
    public float targetOrthoSize = 3f; 
    public float zoomDuration = 1.2f;
    public Vector2 finalCameraOffset = new Vector2(0f, 0.5f); 

    private CinemachineTransposer transposer;
    private CinemachineConfiner2D confiner;

    [SerializeField] Animator transitionAnim;


    void Awake()
    {
        instance = this;
        totalCoinsInLevel = GameObject.FindGameObjectsWithTag("Coin").Length;
        
        if (vcam != null)
        {
            transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
            confiner = vcam.GetComponent<CinemachineConfiner2D>();
        }
        
        UpdateUI();
    }

    public void AddCoin()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        coinsCollected++;
        UpdateUI();
        if ((coinsCollected >= totalCoinsInLevel) && (currentSceneName != "Prologue")) StartCoroutine(LevelCompleteSequence());
    }

    void UpdateUI()
    {
        if (coinText != null) coinText.text = "PARTS: " + coinsCollected + "/" + totalCoinsInLevel;
    }

    IEnumerator LevelCompleteSequence()
    {
        //ini buat PlayerPref
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Prologue")
        {
            PlayerPrefs.SetString("SavedLevel", "Level 1");
        }else if (currentSceneName == "Level 1")
        {
            PlayerPrefs.SetString("SavedLevel", "Level 2");
        }
       else if (currentSceneName == "Level 2")
        {
            PlayerPrefs.SetString("SavedLevel", "Level 3");
        }

        PlayerPrefs.Save();
        
        Debug.Log("Progres disimpan: " + PlayerPrefs.GetString("SavedLevel"));
            //Player Pref end, masuk ke animasi
        FreezeObjectsByTag("Player");
        FreezeObjectsByTag("Enemy");

        if (confiner != null) confiner.enabled = false; 

        Time.timeScale = 0.5f; 
        
        float startSize = vcam.m_Lens.OrthographicSize;
        Vector3 startOffset = transposer != null ? transposer.m_FollowOffset : Vector3.zero;
        Vector3 targetOffset = new Vector3(finalCameraOffset.x, finalCameraOffset.y, startOffset.z);

        float originalXDamp = transposer.m_XDamping;
        float originalYDamp = transposer.m_YDamping;
        transposer.m_XDamping = 2f; 
        transposer.m_YDamping = 2f;

        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / zoomDuration;
            float smoothT = t * t * (3f - 2f * t); 

            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetOrthoSize, smoothT);
            
            if (transposer != null)
                transposer.m_FollowOffset = Vector3.Lerp(startOffset, targetOffset, smoothT);

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);
        
        transposer.m_XDamping = originalXDamp;
        transposer.m_YDamping = originalYDamp;
        
        Time.timeScale = 1f;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings){
            transitionAnim.SetTrigger("End");
            yield return new WaitForSeconds(0.45f);
            SceneManager.LoadScene(nextSceneIndex);
            transitionAnim.SetTrigger("Start");
        }
        else
            SceneManager.LoadScene(0);
    }

    void FreezeObjectsByTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            }

            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null && script != this) script.enabled = false;
            }
        }
    }
}