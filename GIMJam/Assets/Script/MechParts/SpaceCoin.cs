using UnityEngine;
using System.Collections;

public class SpaceCoin : MonoBehaviour
{
    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D other) // Use OnTriggerEnter for 3D
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            StartCoroutine(CollectSequence());
        }
    }

    IEnumerator CollectSequence()
    {
        LevelManager.instance.AddCoin();

        Vector3 startScale = transform.localScale;
        Vector3 giantScale = startScale * 1.5f; // Pop size
        float duration = 0.5f; // Total animation time
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float scaleStep = Mathf.Sin(t * Mathf.PI); 
            transform.localScale = Vector3.Lerp(startScale, giantScale, scaleStep);
            
            if (t > 0.5f) 
                transform.localScale = Vector3.Lerp(giantScale, Vector3.zero, (t - 0.5f) * 2);

            transform.Rotate(0, 0, 720f * Time.deltaTime); 

            yield return null;
        }

        Destroy(gameObject);
    }
}