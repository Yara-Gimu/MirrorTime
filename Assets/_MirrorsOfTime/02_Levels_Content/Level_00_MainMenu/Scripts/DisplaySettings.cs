using System.Collections.Generic;
using UnityEngine;
using TMPro; // ضروري للتعامل مع نصوص وقوائم TextMeshPro

public class DisplaySettings : MonoBehaviour
{
    [Header("--- عناصر الواجهة ---")]
    public TMP_Dropdown resolutionDropdown;

    // مصفوفة لحفظ الدقات التي تدعمها شاشة اللاعب
    private Resolution[] resolutions;

    void Start()
    {
        // 1. جلب كل الدقات المتوافقة مع شاشة اللاعب الفعلية
        resolutions = Screen.resolutions;

        // 2. تنظيف القائمة المنسدلة من أي خيارات سابقة
        resolutionDropdown.ClearOptions();

        // 3. تحويل الدقات إلى نصوص (مثال: 1920 x 1080)
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // التأكد من تحديد الدقة الحالية كخيار افتراضي عند فتح القائمة
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // 4. تعبئة القائمة بالخيارات الجديدة
        resolutionDropdown.AddOptions(options);

        // 5. تحميل الإعداد المحفوظ سابقاً أو استخدام الدقة الحالية
        int savedResolution = PlayerPrefs.GetInt("ResolutionPreference", currentResolutionIndex);
        resolutionDropdown.value = savedResolution;
        resolutionDropdown.RefreshShownValue();
    }

    // --- هذه الدالة سنربطها بالقائمة المنسدلة ---
    public void SetResolution(int resolutionIndex)
    {
        // جلب الدقة التي اختارها اللاعب
        Resolution resolution = resolutions[resolutionIndex];

        // تطبيق الدقة على الشاشة (مع الحفاظ على حالة ملء الشاشة الحالية)
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // حفظ الخيار في جهاز اللاعب باستخدام PlayerPrefs (أفضل للإعدادات التقنية)
        PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
        PlayerPrefs.Save();
        
        Debug.Log("🖥️ تم تغيير دقة الشاشة إلى: " + resolution.width + "x" + resolution.height);
    }
}