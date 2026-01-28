using UnityEngine;

public class TutorGone : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the Canvas with a Canvas Group here")]
    public CanvasGroup canvasToFade;

    [Header("Settings")]
    public float fadeSpeed = 10.0f;
    public string playerTag = "Player";

    private bool lastDialogueState = false;
    private bool isFadingIn = false;

    void Start()
    {
        if (canvasToFade != null) 
            canvasToFade.alpha = 0f;
    }

    void Update()
    {
        DialogueManager dm = DialogueManager.GetInstance();
        if (dm == null) return;

        if (lastDialogueState == true && dm.dialogueIsPlaying == false)
        {
            isFadingIn = true;
        }

        lastDialogueState = dm.dialogueIsPlaying;

        if (isFadingIn && canvasToFade != null)
        {
            if (canvasToFade.alpha < 1f)
            {
                canvasToFade.alpha += fadeSpeed * Time.deltaTime;
            }
            else
            {
                isFadingIn = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered the trigger: " + other.name); // ADD THIS LINE
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player detected! Hiding canvas."); // ADD THIS LINE
            isFadingIn = false;
            if (canvasToFade != null)
            {
                canvasToFade.alpha = 0f;
            }
        }
    }
}