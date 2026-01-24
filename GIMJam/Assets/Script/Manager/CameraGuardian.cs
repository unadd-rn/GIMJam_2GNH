using UnityEngine;
using Cinemachine;

public class CameraLock : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    private CinemachineTransposer transposer;
    
    // Set these to your preferred values in the inspector
    public float lockedZ = -10f;
    public float lockedYOffset = 0.4f;

    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    void LateUpdate()
    {
        if (transposer != null)
        {
            // Force the camera to stay at these offsets every single frame
            Vector3 offset = transposer.m_FollowOffset;
            offset.y = lockedYOffset;
            offset.z = lockedZ;
            transposer.m_FollowOffset = offset;
        }
    }
}