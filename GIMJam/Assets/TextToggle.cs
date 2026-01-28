using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class TextToggle : MonoBehaviour
{
    private TMP_Text textMesh; 
    public float pulseSpeed = 2.0f;
    public float delayBeforeStart = 1.0f;
    
    private float timer = 0f;
    private bool isDisappeared = false;

    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
        
        Color c = textMesh.color;
        c.a = 0;
        textMesh.color = c;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDisappeared = true;
            this.gameObject.SetActive(false);
        }

        if (isDisappeared) return;

        timer += Time.deltaTime;
        
        if (timer < delayBeforeStart)
        {
            return; 
        }

        float alphaValue = Mathf.PingPong((Time.time - delayBeforeStart) * pulseSpeed, 1f);
        
        Color newColor = textMesh.color;
        newColor.a = alphaValue;
        textMesh.color = newColor;
    }
}