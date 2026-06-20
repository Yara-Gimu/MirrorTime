using UnityEngine;
using UnityEngine.UI; // ضروري للتعامل مع السلايدر

public class CameraSensitivityUI : MonoBehaviour
{
    [Header("--- سلايدر الكاميرا ---")]
    public Slider sensitivitySlider;

    void Start()
    {
        // تحميل الحساسية المحفوظة سابقاً (الرقم 1 هو القيمة الافتراضية لو كانت اللعبة تفتح لأول مرة)
        float savedSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 1f);
        
        if(sensitivitySlider != null)
        {
            sensitivitySlider.value = savedSensitivity;
        }
    }

    // هذه الدالة سنربطها بالسلايدر
    public void UpdateSensitivity(float newValue)
    {
        // حفظ الرقم الجديد في جهاز اللاعب
        PlayerPrefs.SetFloat("CameraSensitivity", newValue);
        PlayerPrefs.Save();
        
        Debug.Log("🎥 تم تغيير حساسية الكاميرا. المضاعف الجديد: " + newValue);
    }
}