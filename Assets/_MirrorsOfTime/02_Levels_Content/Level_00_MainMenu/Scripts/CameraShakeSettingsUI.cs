using UnityEngine;
using UnityEngine.UI;

public class CameraShakeSettingsUI : MonoBehaviour
{
    [Header("--- زر اهتزاز الكاميرا ---")]
    public Toggle cameraShakeToggle;

    void Start()
    {
        // 1 تعني مفعل (الافتراضي)، 0 تعني مغلق
        int isShakeOn = PlayerPrefs.GetInt("CameraShake", 1);
        
        if (cameraShakeToggle != null)
        {
            cameraShakeToggle.SetIsOnWithoutNotify(isShakeOn == 1);
            cameraShakeToggle.onValueChanged.AddListener(UpdateCameraShake);
        }
    }

    public void UpdateCameraShake(bool isEnabled)
    {
        PlayerPrefs.SetInt("CameraShake", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}