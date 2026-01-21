using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CutsceneEnd : MonoBehaviour
{
    public static CutsceneEnd Instance;
    public GameObject LevelFinalGroup;
    public GameObject endcutsceneSprite;
    public GameObject worldcanvas;
    public Animator artifactAnim;
    public Animator GlobalVolumeAnim;
    public GameObject lineParticle;
    public GameObject shockExpression;
    public GameObject UIEndingText;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayEndCutscene()
    {
        StartCoroutine(EndCutsceneSequence());
    }

    IEnumerator EndCutsceneSequence()
    {
        // GameManager.Instance.pauseButton.gameObject.SetActive(false);
        // GameManager.Instance.timerText.gameObject.SetActive(false);

        UIEndingText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);

        // start sound here
        SoundManager.Instance.PlaySound2D("ending cutscene");
        //DialogueUI.Instance.portraitGameplayImage.gameObject.SetActive(false);
        shockExpression.SetActive(true);
        
        artifactAnim.SetTrigger("RiseArtifact");
        lineParticle.SetActive(true);
        yield return new WaitForSeconds(3f);
        lineParticle.SetActive(false);
        yield return new WaitForSeconds(2f);

        UIEndingText.gameObject.SetActive(false);
    
        //DialogueUI.Instance.portraitGameplayImage.transform.parent.gameObject.SetActive(false);
        worldcanvas.SetActive(false);

        LevelFinalGroup.SetActive(false);

        endcutsceneSprite.SetActive(true);


        GlobalVolumeAnim.SetTrigger("IncreaseVolume");
        yield return new WaitForSeconds(5);
        // SceneController.Instance.nextScene();
    }
}
