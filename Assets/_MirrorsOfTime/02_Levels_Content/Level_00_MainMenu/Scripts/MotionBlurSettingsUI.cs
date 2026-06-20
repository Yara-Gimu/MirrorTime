using UnityEngine;
using UnityEngine.UI;

public class MotionBlurSettingsUI : MonoBehaviour
{
    [Header("--- زر ضبابية الحركة ---")]
    public Toggle motionBlurToggle;

    void Start()
    {
        // 1 تعني مفعل (الافتراضي)، 0 تعني مغلق
        int isBlurOn = PlayerPrefs.GetInt("MotionBlur", 1);
        
        if (motionBlurToggle != null)
        {
            motionBlurToggle.SetIsOnWithoutNotify(isBlurOn == 1);
            motionBlurToggle.onValueChanged.AddListener(UpdateMotionBlur);
        }
    }

    public void UpdateMotionBlur(bool isEnabled)
    {
        PlayerPrefs.SetInt("MotionBlur", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log("💨 تم تغيير إعداد ضبابية الحركة. الحالة الآن: " + isEnabled);
    }
}