using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

public class DropdownLocalizer : MonoBehaviour
{
    [Header("قائمة الخيارات (Dropdown)")]
    public TMP_Dropdown dropdown;
    
    [Header("خيارات الترجمة")]
    public LocalizedString[] localizedOptions;

    void OnEnable()
    {
        // الاستماع لتغيير اللغة
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        UpdateTranslations();
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale locale)
    {
        UpdateTranslations();
    }

    public void UpdateTranslations()
    {
        if (dropdown == null || localizedOptions == null) return;

        // 🌟 1. نحفظ اختيار اللاعب الحالي (مثلاً كان حاط الجودة: عالية)
        int currentValue = dropdown.value;
        
        // تجهيز القائمة المترجمة
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();
        foreach (var locString in localizedOptions)
        {
            newOptions.Add(new TMP_Dropdown.OptionData(locString.GetLocalizedString()));
        }

        // 🌟 2. تحديث الكلمات
        dropdown.ClearOptions();
        dropdown.AddOptions(newOptions);
        
        // 🌟 3. الحل السحري: نرجع اختيار اللاعب بدون ما نطلق حدث OnValueChanged!
        dropdown.SetValueWithoutNotify(currentValue);
        dropdown.RefreshShownValue();
    }
}