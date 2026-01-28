using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private string triggerID;

    void Start() 
    {
        // If this trigger is meant to start as soon as the level loads
        if (PlayerPrefs.GetInt(triggerID, 0) == 0) 
        {
            StartCoroutine(WaitToStart());
        };
    }

    private IEnumerator WaitToStart() 
    {
        PlayerPrefs.SetInt(triggerID, 1);
        PlayerPrefs.Save();
        // Wait until the very end of the first frame
        yield return new WaitForEndOfFrame();
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
    }
}
