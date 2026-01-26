using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private void Start()
    {
        StartCoroutine(StartDialogueWithDelay());
    }

    private IEnumerator StartDialogueWithDelay()
    {
        yield return null; 

        //yieldnya buat nunggu 1 frame biar gak bentrok ama si manager yg bikin false

        Debug.Log("Sekarang masuk ke Dialogue Mode");
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
    }
}
