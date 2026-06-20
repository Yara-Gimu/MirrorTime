using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.EventSystems; 
using UnityEngine.InputSystem; // 🌟 تم إضافة مكتبة الإدخال
using System.Collections; 

public class TabManager : MonoBehaviour
{
    [Header("مدير الصفحات")]
    public GameObject[] pages; 
    public TMP_Text titleText; 
    
    [Header("أزرار التبويبات (ضروري لربط اليد)")]
    public Button[] tabButtons; 

    [Header("عناوين الصفحات المترجمة")]
    public LocalizedString[] localizedTitles; 

    [Header("نصوص التبويبات (TMP)")]
    public TMP_Text[] tabTexts; 

    [Header("إعدادات الإدخال الحديث للتبديل (L1/R1 أو الأسهم)")]
    public InputActionReference nextTabAction; // 🌟 اربطي هنا زر الانتقال للتالي (مثل R1)
    public InputActionReference prevTabAction; // 🌟 اربطي هنا زر الانتقال السابق (مثل L1)

    [Header("إعدادات الألوان والتأثير")]
    public Color activeColor = new Color32(232, 224, 213, 255); 
    public Color inactiveColor = new Color32(150, 150, 150, 200); 
    public Color activeGlowColor = new Color32(232, 224, 213, 150); 

    private int currentTab = 0;

    void OnEnable()
    {
        if (nextTabAction != null) { nextTabAction.action.Enable(); nextTabAction.action.performed += OnNextTab; }
        if (prevTabAction != null) { prevTabAction.action.Enable(); prevTabAction.action.performed += OnPrevTab; }
    }

    void OnDisable()
    {
        if (nextTabAction != null) nextTabAction.action.performed -= OnNextTab;
        if (prevTabAction != null) prevTabAction.action.performed -= OnPrevTab;
    }

    private void OnNextTab(InputAction.CallbackContext context)
    {
        currentTab = (currentTab + 1) % pages.Length;
        OpenTab(currentTab);
    }

    private void OnPrevTab(InputAction.CallbackContext context)
    {
        currentTab--;
        if (currentTab < 0) currentTab = pages.Length - 1;
        OpenTab(currentTab);
    }

    public void OpenTab(int tabIndex)
    {
        currentTab = tabIndex;
        StartCoroutine(OpenTabRoutine(tabIndex));
    }

    private IEnumerator OpenTabRoutine(int tabIndex)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null) pages[i].SetActive(i == tabIndex);
        }

        for (int i = 0; i < tabTexts.Length; i++)
        {
            if (tabTexts[i] != null)
            {
                Material textMat = tabTexts[i].fontMaterial;
                bool isActive = (i == tabIndex);
                
                tabTexts[i].color = isActive ? activeColor : inactiveColor;
                
                if (isActive) 
                {
                    textMat.EnableKeyword("GLOW_ON");
                    tabTexts[i].transform.localScale = Vector3.one * 1.05f; 
                } 
                else 
                {
                    textMat.DisableKeyword("GLOW_ON");
                    tabTexts[i].transform.localScale = Vector3.one; 
                }
            }
        }

        yield return new WaitForEndOfFrame();

        if (tabButtons.Length > tabIndex && tabButtons[tabIndex] != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(tabButtons[tabIndex].gameObject);
        }

        if (titleText != null && localizedTitles.Length > tabIndex)
        {
            titleText.text = localizedTitles[tabIndex].GetLocalizedString();
        }
    }
}