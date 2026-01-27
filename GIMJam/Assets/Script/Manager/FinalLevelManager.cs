using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Cinemachine;

public class FinalLevelManager : MonoBehaviour
{
    public static FinalLevelManager instance;

    [Header("UI & Tracking")]
    public TextMeshProUGUI coinText;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera vcam;
    public float targetOrthoSize = 3f; 
    public float zoomDuration = 1.2f;
    public Vector2 finalCameraOffset = new Vector2(0f, 0.5f); 

    private CinemachineTransposer transposer;
    private CinemachineConfiner2D confiner;
    public GameObject spriteplayer;
    public GameObject robot;

    void Awake()
    {
        instance = this;
                
        if (vcam != null)
        {
            transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
            confiner = vcam.GetComponent<CinemachineConfiner2D>();
        }
        
    }

    public void OnLaunch(bool launch)
    {
        if (!launch) 
        {
            if (coinText != null) coinText.text ="Go To The Launcher!";
        }

        if (launch) 
        {
            if (coinText != null) coinText.text ="LAUNCH!!   >.<";
            Animator anim = spriteplayer.GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("Jump");
            StartCoroutine(LevelCompleteSequence());
        
        ControlPlate[] allPlates = Object.FindObjectsByType<ControlPlate>(FindObjectsSortMode.None);

        foreach (ControlPlate script in allPlates)
        {
            script.enabled = false; // Matikan scriptnya
        }}
    }

    IEnumerator LevelCompleteSequence()
    {
        // 1. Simpan Progres (PlayerPrefs)
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Level 1") PlayerPrefs.SetString("SavedLevel", "Level 2");
        else if (currentSceneName == "Level 2") PlayerPrefs.SetString("SavedLevel", "Level 3");
        else if (currentSceneName == "Level 3") PlayerPrefs.SetString("SavedLevel", "Level 3");//nnti ganti cutscene akhir
        PlayerPrefs.Save();

        // 2. Setup Awal: Cari Player & Stop Camera Follow
        Rigidbody2D rb = robot.GetComponent<Rigidbody2D>();
        vcam.Follow = null; 
        
        FreezeObjectsByTag("Enemy"); 
        FreezeObjectsByTag("FinalBoss"); 
        // Ganti nama parameter sesuai Animator-mu

        // --- SETUP PLAYER (PENTING!) ---
        if (rb != null) 
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // Jadi penurut sama script
        }
    
        float flyDuration = 3.0f; 
        float startZoomAt = 1f; 
        float elapsedFly = 0f;
        float elapsedZoom = 0f;

        Vector3 playerStartPos = robot.transform.position;
        Vector3 playerTargetPos = playerStartPos + Vector3.up * 10f;
        float startSize = vcam.m_Lens.OrthographicSize;

        while (elapsedFly < flyDuration)
        {
            elapsedFly += Time.unscaledDeltaTime;
            float tFly = elapsedFly / flyDuration;

            // Gerakin player murni pakai posisi (aman karena sudah Kinematic)
            robot.transform.position = Vector3.Lerp(playerStartPos, playerTargetPos, tFly);

            // Logic Zoom Paralel
            if (elapsedFly >= startZoomAt)
            {
                elapsedZoom += Time.unscaledDeltaTime;
                float tZoom = Mathf.Clamp01(elapsedZoom / zoomDuration);
                float smoothT = tZoom * tZoom * (3f - 2f * tZoom); 
                vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetOrthoSize, smoothT);
            }

            yield return null;
        }
        
        yield return new WaitForSecondsRealtime(1f);

        // 5. Reset & Pindah Scene
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
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