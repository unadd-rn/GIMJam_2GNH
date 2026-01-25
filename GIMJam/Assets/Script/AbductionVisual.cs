using UnityEngine;
using System.Collections;
using Cinemachine;

public class AbductionVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera playerVCam;
    [SerializeField] private Transform spriteChild;

    [Header("Visual Settings")]
    [SerializeField] private float duration = 3.0f;
    [SerializeField] private float targetZoom = 3.0f;
    [SerializeField] private float targetLensShiftY = 0.2f;

    public void PlayAbductionSequence()
    {
        StartCoroutine(AbductionRoutine());
    }

    private IEnumerator AbductionRoutine()
    {
        float elapsed = 0;
        float startZoom = playerVCam.m_Lens.OrthographicSize;
        float startShiftY = playerVCam.m_Lens.LensShift.y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float snappyT = 1 - Mathf.Pow(1 - t, 3); 

            playerVCam.m_Lens.OrthographicSize = Mathf.Lerp(startZoom, targetZoom, snappyT);
            
            Vector2 currentShift = playerVCam.m_Lens.LensShift;
            currentShift.y = Mathf.Lerp(startShiftY, targetLensShiftY, snappyT);
            playerVCam.m_Lens.LensShift = currentShift;

            if (spriteChild != null)
            {
                float zTilt = Mathf.Sin(Time.time * 15f) * 15f; 
                spriteChild.localRotation = Quaternion.Euler(0, 0, zTilt);
            }

            yield return null;
        }
    }
}