using UnityEngine;
using UnityEngine.UI; // مهم عشان نقدر نتعامل مع الـ Toggle

public class SubtitleSettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject subtitleTextObject; // هنا بنسحب الـ Subtitle_Text تبع الترجمة
    public Toggle subtitlesToggle; // هنا بنسحب زر الـ Toggle

    private void Start()
    {
        // 1. نقرأ الإعداد المحفوظ من قبل (إذا ما كان في حفظ سابق، نعتبره 1 يعني شغال)
        int isSubtitlesOn = PlayerPrefs.GetInt("SubtitlesEnabled", 1);
        
        // 2. نحدث شكل زر الـ Toggle في بداية اللعبة
        subtitlesToggle.isOn = isSubtitlesOn == 1;

        // 3. نطبق الإعداد على النص
        subtitleTextObject.SetActive(subtitlesToggle.isOn);

        // 4. نربط الدالة بالزر عشان تشتغل كل ما اللاعب يغير الخيار
        subtitlesToggle.onValueChanged.AddListener(OnSubtitleToggleChanged);
    }

    // هذه الدالة تشتغل كل ما اللاعب ضغط على زر التفعيل
    public void OnSubtitleToggleChanged(bool isEnabled)
    {
        // إظهار أو إخفاء نص الترجمة بناءً على اختيار اللاعب
        subtitleTextObject.SetActive(isEnabled);

        // حفظ الخيار عشان ما يضطر اللاعب يعدله كل مرة يفتح اللعبة
        PlayerPrefs.SetInt("SubtitlesEnabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}