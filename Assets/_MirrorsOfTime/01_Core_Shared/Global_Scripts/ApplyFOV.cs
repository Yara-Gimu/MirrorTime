using UnityEngine;
using Unity.Cinemachine; // أو using Cinemachine; حسب نسختك

public class ApplyFOV : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;

    void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();

        // جلب الـ FOV الذي اختاره اللاعب (60 هو الافتراضي)
        float fovMultiplier = PlayerPrefs.GetFloat("CameraFOV", 60f);

        if (freeLookCamera != null)
        {
            // تطبيق الـ FOV على الكاميرا
            freeLookCamera.m_Lens.FieldOfView = fovMultiplier;
        }
    }
}