using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player")) 
        {
            StartCoroutine(WaitToStart());
        }
    }

    private IEnumerator WaitToStart() 
    {
        // Wait until the very end of the first frame
        yield return new WaitForEndOfFrame();
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
    }
}
