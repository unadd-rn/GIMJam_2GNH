using UnityEngine;
using System.Collections;
using Cinemachine;

public class AbductionVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera playerVCam;
    [SerializeField] private Transform spriteChild; // Drag the Sprite child here

    [Header("Visual Settings")]
    [SerializeField] private float duration = 3.0f;
    [SerializeField] private float targetZoom = 3.0f; // Smaller number = closer zoom
    [SerializeField] private float targetLensShiftY = 0.2f; // Similar to your pressure plate offset

    public void PlayAbductionSequence()
    {
        StartCoroutine(AbductionRoutine());
    }

    private IEnumerator AbductionRoutine()
    {
        Debug.Log("Abduction Sequence Started!");
        float elapsed = 0;
        
        float startZoom = playerVCam.m_Lens.OrthographicSize;
        float startShiftY = playerVCam.m_Lens.LensShift.y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // ZOOM: From 3 down to your target (e.g., 1.5)
            playerVCam.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, targetZoom, smoothT);

            // OFFSET: Shift the lens vertically
            Vector2 currentShift = playerVCam.m_Lens.LensShift;
            currentShift.y = Mathf.Lerp(startShiftY, targetLensShiftY, smoothT);
            playerVCam.m_Lens.LensShift = currentShift;

            // TILT: Wobble the child sprite
            if (spriteChild != null)
            {
                float zTilt = Mathf.Sin(Time.time * 15f) * 15f; 
                spriteChild.localRotation = Quaternion.Euler(0, 0, zTilt);
            }

            yield return null;
        }
        Debug.Log("Abduction Sequence Finished!");
    }
}