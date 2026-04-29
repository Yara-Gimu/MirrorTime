using UnityEngine;
using UnityEngine.Localization.Settings; // مكتبة الترجمة
using TMPro; // عشان نتعامل مع القائمة

public class LanguageSwitcher : MonoBehaviour
{
    public TMP_Dropdown languageDropdown; // اسحبي القائمة هنا

    void Start()
    {
        // أول ما تبدأ اللعبة، نخلي القائمة تختار اللغة المحفوظة حالياً
        // مثلاً لو اللاعب كان مختار عربي، القائمة تكون واقفة على "العربية"
        int currentID = LocalizationSettings.SelectedLocale.SortOrder;
        
        // ملاحظة: هذا يعتمد على ترتيبك للغات في Localization Settings
        // الأسهل هو البحث عن اللغة الحالية في القائمة واختيارها
        UpdateDropdownToCurrentLanguage();
    }

    public void ChangeLanguage(int index)
    {
        // هذه الدالة تتنادى لما اللاعب يغير الخيار في القائمة
        // index هو رقم الخيار (0, 1, 2...)
        
        // نغير لغة اللعبة بناءً على الترتيب في قائمة اللغات المتوفرة
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
    
    void UpdateDropdownToCurrentLanguage()
    {
        // كود إضافي عشان يخلي القائمة تختار اللغة الصح عند البداية
        // (يمكنك تركه الآن إذا كان معقداً، لكنه مفيد)
        var currentLocale = LocalizationSettings.SelectedLocale;
        var options = LocalizationSettings.AvailableLocales.Locales;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] == currentLocale)
            {
                languageDropdown.value = i;
                break;
            }
        }
    }
}