using UnityEngine;
using System.Collections;

public class SpaceCoin : MonoBehaviour
{
    [Header("Hover Settings")]
    public float floatAmplitude = 0.2f; //bob height
    public float floatFrequency = 1.5f; //bob speed
    
    private bool isCollected = false;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!isCollected)
        {
            float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = startPos + new Vector3(0, newY, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            SoundManager.Instance.PlaySound2D("Collect Coin");
            StartCoroutine(CollectSequence());
        }
    }

    IEnumerator CollectSequence()
    {
        LevelManager.instance.AddCoin();

        Vector3 currentScale = transform.localScale;
        Vector3 giantScale = currentScale * 1.5f; 
        float duration = 0.5f; 
        float elapsed = 0f;

        Vector3 collectionPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float scaleStep = Mathf.Sin(t * Mathf.PI); 
            transform.localScale = Vector3.Lerp(currentScale, giantScale, scaleStep);
            
            if (t > 0.5f) 
                transform.localScale = Vector3.Lerp(giantScale, Vector3.zero, (t - 0.5f) * 2);

            transform.Rotate(0, 0, 720f * Time.deltaTime); 

            yield return null;
        }

        Destroy(gameObject);
    }
}