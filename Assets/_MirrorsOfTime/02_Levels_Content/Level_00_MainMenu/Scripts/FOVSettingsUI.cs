using UnityEngine;
using UnityEngine.UI;

public class FOVSettingsUI : MonoBehaviour
{
    [Header("--- سلايدر مجال الرؤية ---")]
    public Slider fovSlider;

    void Start()
    {
        // تحميل الـ FOV المحفوظ (60 هو الافتراضي)
        float savedFOV = PlayerPrefs.GetFloat("CameraFOV", 60f);
        
        if(fovSlider != null)
        {
            fovSlider.value = savedFOV;
        }
    }

    // هذه الدالة سنربطها بالسلايدر
    public void UpdateFOV(float newValue)
    {
        PlayerPrefs.SetFloat("CameraFOV", newValue);
        PlayerPrefs.Save();
        
        Debug.Log("👁️ تم تغيير مجال الرؤية (FOV) إلى: " + newValue);
    }
}