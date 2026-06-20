using UnityEngine;
using UnityEngine.UI;

public class VSyncSettingsUI : MonoBehaviour
{
    [Header("--- زر تزامن الإطارات (VSync) ---")]
    public Toggle vsyncToggle;

    void Start()
    {
        // 1 يعني مفعل (وهو الأفضل افتراضياً لمنع تقطيع الشاشة)، 0 يعني مغلق
        int isVSyncOn = PlayerPrefs.GetInt("VSync", 1);
        
        if (vsyncToggle != null)
        {
            // تحديث شكل الزر في الواجهة
            vsyncToggle.SetIsOnWithoutNotify(isVSyncOn == 1);
            
            // ربط الزر بالدالة برمجياً
            vsyncToggle.onValueChanged.AddListener(UpdateVSync);
        }

        // تطبيق الإعداد الفعلي على اللعبة فوراً عند التشغيل
        ApplyVSync(isVSyncOn == 1);
    }

    public void UpdateVSync(bool isEnabled)
    {
        // حفظ الإعداد في جهاز اللاعب
        PlayerPrefs.SetInt("VSync", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
        
        // تطبيق الإعداد
        ApplyVSync(isEnabled);
        
        Debug.Log("📺 تم تغيير إعداد VSync. الحالة الآن: " + isEnabled);
    }

    private void ApplyVSync(bool isEnabled)
    {
        // السطر السحري: 1 تعني تفعيل التزامن مع سرعة شاشة اللاعب، 0 تعني إغلاقه
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;
    }
}