using UnityEngine;
using TMPro; 
using UnityEngine.Localization.Settings; 

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance; 

    [Header("UI Elements")]
    public TextMeshProUGUI subtitleText;
    public GameObject subtitlePanel; 

    [Header("Localization Settings")]
    public string tableName = "SubtitlesTable"; 
    
    [Tooltip("ضعي هنا مفاتيح الترجمة الـ 6 بالترتيب (للتايم لاين)")]
    public string[] subtitleKeys; 

    private int currentIndex = 0; 

    // 🌟 التعديل السحري هنا: جعلنا هذا المتغير يقرأ حالة الإعدادات الحية مباشرة بدلاً من قراءتها مرة واحدة
    private bool isSubtitleEnabled 
    {
        get { return PlayerPrefs.GetInt("SubtitlesEnabled", 1) == 1; }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // مسحنا السطر القديم الذي كان يقرأ الإعداد هنا فقط
        HideSubtitle();
        currentIndex = 0; 
    }

    // 🌟 1. هذه الدالة مخصصة للـ Timeline (دبابيس التايم لاين تقرأ بالترتيب)
    public void ShowNextSubtitle()
    {
        if (!isSubtitleEnabled || currentIndex >= subtitleKeys.Length) return;

        string key = subtitleKeys[currentIndex];
        string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);

        if (!string.IsNullOrEmpty(localizedText))
        {
            subtitleText.text = localizedText;
            subtitlePanel.SetActive(true);
        }
        
        currentIndex++; 
    }

    // 🌟 2. هذه الدالة مخصصة للـ Triggers العادية (تقبل LocalizedString)
    public void ShowSubtitle(UnityEngine.Localization.LocalizedString localizedString)
    {
        if (!isSubtitleEnabled) return; 

        string text = localizedString.GetLocalizedString();
        
        if (!string.IsNullOrEmpty(text))
        {
            subtitleText.text = text;
            subtitlePanel.SetActive(true);
        }
    }

    // 🌟 3. إخفاء الترجمة
    public void HideSubtitle()
    {
        if (this != null && subtitlePanel != null) 
            subtitlePanel.SetActive(false);
    }

    // 🌟 4. الدالة الناقصة: مخصصة لسكربت فحص النقوش (تقبل String مباشر)
    public void ShowSubtitleByKey(string translationKey)
    {
        if (!isSubtitleEnabled) return; 

        string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, translationKey);

        if (!string.IsNullOrEmpty(localizedText))
        {
            subtitleText.text = localizedText;
            subtitlePanel.SetActive(true);
        }
    }
}