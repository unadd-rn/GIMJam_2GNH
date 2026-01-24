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

    private CinemachineTransposer transposer;

    void Awake()
    {
        instance = this;
        totalCoinsInLevel = GameObject.FindGameObjectsWithTag("Coin").Length;
        
        if (vcam != null)
        {
            transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        }
        
        UpdateUI();
    }

    public void AddCoin()
    {
        coinsCollected++;
        UpdateUI();
        if (coinsCollected >= totalCoinsInLevel) StartCoroutine(LevelCompleteSequence());
    }

    void UpdateUI()
    {
        if (coinText != null) coinText.text = "PARTS: " + coinsCollected + "/" + totalCoinsInLevel;
    }

    IEnumerator LevelCompleteSequence()
    {
        Time.timeScale = 0.5f; 
        
        float startSize = vcam.m_Lens.OrthographicSize;
        
        Vector3 startOffset = Vector3.zero;
        if (transposer != null)
        {
            startOffset = transposer.m_FollowOffset;
        }

        Vector3 targetOffset = new Vector3(0, 0, startOffset.z);

        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / zoomDuration;

            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetOrthoSize, t);
            
            if (transposer != null)
            {
                transposer.m_FollowOffset = Vector3.Lerp(startOffset, targetOffset, t);
            }

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}