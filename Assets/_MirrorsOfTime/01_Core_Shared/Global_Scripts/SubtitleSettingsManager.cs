using UnityEngine;
using UnityEngine.UI;

public class SubtitleSettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Toggle subtitlesToggle; 

    private void Start()
    {
        // قراءة الإعداد المحفوظ
        int isSubtitlesOn = PlayerPrefs.GetInt("SubtitlesEnabled", 1);
        
        // تطبيق الإعداد برمجياً على الزر
        subtitlesToggle.SetIsOnWithoutNotify(isSubtitlesOn == 1);

        // ربط الدالة بالزر
        subtitlesToggle.onValueChanged.AddListener(OnSubtitleToggleChanged);
    }

    public void OnSubtitleToggleChanged(bool isEnabled)
    {
        // حفظ الإعداد الجديد
        PlayerPrefs.SetInt("SubtitlesEnabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();

        // 🌟 السطر السحري الجديد:
        // إذا قامت اللاعبة بإطفاء الترجمة الآن، قم بإخفاء أي نص موجود على الشاشة فوراً!
        if (!isEnabled && SubtitleManager.Instance != null)
        {
            SubtitleManager.Instance.HideSubtitle();
        }
    }
}