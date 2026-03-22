using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class TabManager : MonoBehaviour
{
    [Header("مدير الصفحات")]
    public GameObject[] pages; 
    public TMP_Text titleText; 
    public string[] titleLocalizationKeys; 

    [Header("نصوص التبويبات (TMP)")]
    public TMP_Text[] tabTexts; // اسحبي هنا نصوص الأزرار (Text TMP اللي داخل Btn_Audio وغيرها)

    [Header("ألوان التبويبات")]
    public Color activeColor = new Color32(232, 224, 213, 255); // لون ذهبي مشرق
    public Color inactiveColor = new Color32(150, 150, 150, 200); // رمادي باهت
    public Color activeGlowColor = new Color32(232, 224, 213, 150); // توهج ذهبي

    void Start()
    {
        OpenTab(0); // يفتح أول صفحة ويخلي زرها مضيء ديفولت
    }

    public void OpenTab(int tabIndex)
    {
        // 1. فتح وإغلاق الصفحات
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null) pages[i].SetActive(i == tabIndex);
        }

        // 2. ✨ التحكم في إضاءة الأزرار الثابتة (التأثير البصري)
        for (int i = 0; i < tabTexts.Length; i++)
        {
            if (tabTexts[i] != null)
            {
                Material textMat = tabTexts[i].fontMaterial;
                
                if (i == tabIndex) // الزر المختار (نشط)
                {
                    tabTexts[i].color = activeColor;
                    textMat.EnableKeyword("GLOW_ON");
                    textMat.SetColor("_GlowColor", activeGlowColor);
                    textMat.SetFloat("_GlowPower", 0.5f);
                    tabTexts[i].transform.localScale = Vector3.one * 1.05f; // تكبير بسيط
                }
                else // الأزرار غير النشطة
                {
                    tabTexts[i].color = inactiveColor;
                    textMat.DisableKeyword("GLOW_ON");
                    textMat.SetFloat("_GlowPower", 0f);
                    tabTexts[i].transform.localScale = Vector3.one; // حجم طبيعي
                }
            }
        }

        // 3. تغيير العنوان الرئيسي
        if (titleText != null && titleLocalizationKeys.Length > tabIndex)
        {
            LocalizationSettings.InitializationOperation.Completed += (op) => {
                string localizedTitle = LocalizationSettings.StringDatabase.GetLocalizedString("MyGameText", titleLocalizationKeys[tabIndex]);
                titleText.text = localizedTitle;
            };
        }
    }
}