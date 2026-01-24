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

    void Awake()
    {
        instance = this;
        totalCoinsInLevel = GameObject.FindGameObjectsWithTag("Coin").Length;
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
        Time.timeScale = 0.5f; // Slow motion "Matrix" effect
        
        float startSize = vcam.m_Lens.OrthographicSize;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetOrthoSize, elapsed / zoomDuration);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}