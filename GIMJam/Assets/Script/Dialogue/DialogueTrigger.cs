using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private void Start()
    {
        Debug.Log("masuk");
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
    }
}
