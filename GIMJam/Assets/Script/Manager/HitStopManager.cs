using System.Collections;
using UnityEngine;
using Cinemachine;
public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;
    
    [Header("Camera Shake Settings")]
    [SerializeField] private CinemachineVirtualCamera _vCam;
    private CinemachineBasicMultiChannelPerlin _noise;

    private void Awake()
    {
        Instance = this;
        if (_vCam != null)
            _noise = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Stop(float duration, float shakeIntensity = 2f)
    {
        StartCoroutine(Wait(duration, shakeIntensity));
    }

    private IEnumerator Wait(float duration, float intensity)
    {
        if (_noise != null) _noise.m_AmplitudeGain = intensity;

        yield return null; 

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;

        if (_noise != null) _noise.m_AmplitudeGain = 0f;
    }
}