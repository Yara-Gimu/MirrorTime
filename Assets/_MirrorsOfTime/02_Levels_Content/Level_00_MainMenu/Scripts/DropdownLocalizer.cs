using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;                // عشان الدروب داون
using UnityEngine.Localization; // مكتبة الترجمة
using UnityEngine.Localization.Settings; // إعدادات الترجمة

public class DropdownLocalizer : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public string[] optionKeys; // هنا بنكتب المفاتيح (Quality_Low, etc)

    void OnEnable()
    {
        // أول ما تشتغل اللعبة، نحدث القائمة
        UpdateDropdownOptions();
        // ونشترك في حدث تغيير اللغة (عشان لو غيرنا اللغة تتحدث القائمة فوراً)
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(Locale locale)
    {
        UpdateDropdownOptions();
    }

    void UpdateDropdownOptions()
    {
        // لازم ننتظر الترجمة تتحمل
        StartCoroutine(UpdateRoutine());
    }

    IEnumerator UpdateRoutine()
    {
        // ننتظر تحميل النظام
        yield return LocalizationSettings.InitializationOperation;

        var options = new List<string>();

        // نلف على المفاتيح اللي انتي حطيتيها ونجيب ترجمتها
        foreach (var key in optionKeys)
        {
            var localizedString = LocalizationSettings.StringDatabase.GetLocalizedString("MyGameText", key); 
            // تأكدي أن اسم الجدول هنا "MainMenu Table" يطابق اسم جدولك بالضبط
            options.Add(localizedString);
        }

        // نمسح القديم ونحط الجديد المترجم
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }
}