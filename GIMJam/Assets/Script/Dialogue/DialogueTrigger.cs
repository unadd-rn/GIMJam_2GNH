using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private void Start()
    {
        // Kita jalankan Coroutine-nya di sini
        StartCoroutine(StartDialogueWithDelay());
    }

    private IEnumerator StartDialogueWithDelay()
    {
        Debug.Log("Menunggu satu frame agar tidak tabrakan...");
        
        // Opsi 1: Tunggu 1 frame (biasanya cukup untuk mengatasi konflik Start)
        yield return null; 

        // Opsi 2: Kalau 1 frame masih kurang, pakai jeda waktu (misal 0.1 detik)
        // yield return new WaitForSeconds(0.1f);

        Debug.Log("Sekarang masuk ke Dialogue Mode");
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
    }
}
