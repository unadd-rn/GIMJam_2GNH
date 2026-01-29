using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;


    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;


    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    private Animator layoutAnimator;


    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;


    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSO[] audioInfos;
    [SerializeField] private bool makePredictable;
    private DialogueAudioInfoSO currentAudioInfo;
    private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;
    private AudioSource audioSource;

    [Header("Background/Image UI")]
// The UI Image component on your Canvas where the picture will show up
    [SerializeField] private UnityEngine.UI.Image displayImage; 

    // A list of all possible Sprites (images) you want to use in your game
    [SerializeField] private List<Sprite> sceneSprites; 

    [Header("Scene Management")]
    // Drag your Scene Asset here if using a specific reference, 
    // or just use a string for the Scene Name.
    [SerializeField] private string sceneToLoadAtEnd; 
    [SerializeField] private bool loadSceneOnExit = false;

    // The name of the tag we are looking for in Ink
    private const string IMAGE_TAG = "image";


    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }


    private bool canContinueToNextLine = false;


    private Coroutine displayLineCoroutine;


    private static DialogueManager instance;


    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";
    private const string AUDIO_TAG = "audio";


    private DialogueVariables dialogueVariables;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;


        dialogueVariables = new DialogueVariables(loadGlobalsJSON);


        audioSource = this.gameObject.AddComponent<AudioSource>();
        currentAudioInfo = defaultAudioInfo;
    }


    public static DialogueManager GetInstance()
    {
        return instance;
    }


    private void Start()
    {
        Debug.Log("MASUK");
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);


        // get the layout animator
        layoutAnimator = dialoguePanel.GetComponent<Animator>();


        // get all of the choices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }


        InitializeAudioInfoDictionary();
    }


    private void InitializeAudioInfoDictionary()
    {
        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
        audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
        foreach (DialogueAudioInfoSO audioInfo in audioInfos)
        {
            audioInfoDictionary.Add(audioInfo.id, audioInfo);
        }
    }


    private void SetCurrentAudioInfo(string id)
    {
        DialogueAudioInfoSO audioInfo = null;
        audioInfoDictionary.TryGetValue(id, out audioInfo);
        if (audioInfo != null)
        {
            this.currentAudioInfo = audioInfo;
        }
        else
        {
            Debug.LogWarning("Failed to find audio info for id: " + id);
        }
    }


    private void Update() 
    {
        if (!dialogueIsPlaying) return;

        // Use a variable to catch the input
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);

        // Only continue if the typing is finished AND space is pressed
        if (canContinueToNextLine 
            && currentStory.currentChoices.Count == 0 
            && spacePressed)
        {
            ContinueStory();
        }
    }


    public void EnterDialogueMode(TextAsset inkJSON) 
    {
        // 1. Reset everything to a "Clean State"
        StopAllCoroutines(); // Stop any leftover typing from previous scenes
        canContinueToNextLine = false; 
        
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);

        // 2. Clear the text box so it's not showing old text
        dialogueText.text = "";
        displayNameText.text = "";

        // 3. Force a "Fresh Start"
        StartCoroutine(ForceFirstLine());
    }

    private IEnumerator ForceFirstLine()
    {
        // Wait two frames to be absolutely sure Input is cleared 
        // and the Canvas has rebuilt itself.
        yield return null; 
        yield return null; 

        if (currentStory.canContinue)
        {
            ContinueStory();
        }
    }


    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);


        dialogueVariables.StopListening(currentStory);


        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";


        // go back to default audio
        SetCurrentAudioInfo(defaultAudioInfo.id);

        if (loadSceneOnExit && !string.IsNullOrEmpty(sceneToLoadAtEnd))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName == "Prologue")
            {
                PlayerPrefs.SetString("SavedLevel", "Level 1");
            }

            GameObject.Find("Scene Transition").GetComponent<Animator>().SetTrigger("End");

            yield return new WaitForSeconds(1.5f);

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoadAtEnd);
        }

        PauseManager.ToggleEntities(true);
    }


    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            
            // set text for the current dialogue line
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            string nextLine = currentStory.Continue();
            Debug.Log("INK SAYS: " + nextLine); 
            // handle tags
            HandleTags(currentStory.currentTags);
            displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }


    private IEnumerator DisplayLine(string line)
    {
        // Set the text to the full line, but set the visible characters to 0
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
       
        // Hide items while text is typing
        continueIcon.SetActive(false);
        HideChoices();


        canContinueToNextLine = false;


        // --- FIX: WAIT FOR ONE FRAME ---
        // This prevents the Spacebar press used to CONTINUE from being
        // registered as a SKIP for this new line.
        yield return null;


        bool isAddingRichTextTag = false;


        // Display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // If the spacebar is pressed during typing, skip to the end of the line
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }


            // Check for rich text tag
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else
            {
                PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters]);
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }


        // Actions to take after the entire line has finished displaying
        continueIcon.SetActive(true);
        DisplayChoices();


        // --- FIX: INPUT BUFFER ---
        // Wait one frame before allowing "Continue" so a single click
        // doesn't skip both the typing AND the line itself.
        yield return null;
        canContinueToNextLine = true;
    }


    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        // set variables for the below based on our config
        AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
        int frequencyLevel = currentAudioInfo.frequencyLevel;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;


        // play the sound based on the config
        if (currentDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }
            // AudioClip soundClip = null;
            // // create predictable audio from hashing
            // if (makePredictable)
            // {
            //     int hashCode = currentCharacter.GetHashCode();
            //     // sound clip
            //     int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
            //     soundClip = dialogueTypingSoundClips[predictableIndex];
            //     // pitch
            //     int minPitchInt = (int) (minPitch * 100);
            //     int maxPitchInt = (int) (maxPitch * 100);
            //     int pitchRangeInt = maxPitchInt - minPitchInt;
            //     // cannot divide by 0, so if there is no range then skip the selection
            //     if (pitchRangeInt != 0)
            //     {
            //         int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
            //         float predictablePitch = predictablePitchInt / 100f;
            //         audioSource.pitch = predictablePitch;
            //     }
            //     else
            //     {
            //         audioSource.pitch = minPitch;
            //     }
            // }
            // // otherwise, randomize the audio
            // else
            // {
            //     // sound clip
            //     int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
            //     soundClip = dialogueTypingSoundClips[randomIndex];
            //     // pitch
            //     audioSource.pitch = Random.Range(minPitch, maxPitch);
            // }
           
            // play sound
            // audioSource.PlayOneShot(soundClip);

            SoundManager.Instance.PlaySound2D("Dialogue Typing");
        }
    }


    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }


    private void HandleTags(List<string> currentTags)
    {
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();
           
            // handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                case AUDIO_TAG:
                    SetCurrentAudioInfo(tagValue);
                    break;
                case IMAGE_TAG: // If the tag is #image: something
                    ChangeSceneImage(tagValue); // Run our new function
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void ChangeSceneImage(string spriteName) 
    {
        // Search through our list of sprites for one that matches the name from Ink
        foreach (Sprite s in sceneSprites) 
        {
            if (s.name == spriteName) 
            {
                displayImage.sprite = s; // Change the image on screen!
                return;
            }
        }
        Debug.LogWarning("I couldn't find an image named: " + spriteName);
    }
    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;


        // defensive check to make sure our UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                + currentChoices.Count);
        }


        int index = 0;
        // enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        // go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }


        StartCoroutine(SelectFirstChoice());
    }


    private IEnumerator SelectFirstChoice()
    {
        // Event System requires we clear it first, then wait
        // for at least one frame before we set the current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        // check if exist so not out of bound
        if (choices.Length > 0)
        {
            EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
        }
    }


    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            // NOTE: The below two lines were added to fix a bug after the Youtube video was made
            ContinueStory();
        }
    }


    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }


    // This method will get called anytime the application exits.
    // Depending on your game, you may want to save variable state in other places.
    public void OnApplicationQuit()
    {
        dialogueVariables.SaveVariables();
    }


}




