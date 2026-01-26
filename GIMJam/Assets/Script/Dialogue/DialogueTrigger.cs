using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    // Inside your DialogueTrigger.cs
    void Start() 
    {
        // If this trigger is meant to start as soon as the level loads
        StartCoroutine(WaitToStart());
    }

    private IEnumerator WaitToStart() 
    {
        // Wait until the very end of the first frame
        yield return new WaitForEndOfFrame();
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
    }
}
