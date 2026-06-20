using UnityEngine;
using UnityEngine.Localization;

public class SubtitleTrigger : MonoBehaviour
{
    [Header("اختر الجملة من جدول الترجمة")]
    public LocalizedString subtitleKey;

    // عندما يتم تفعيل هذا المجسم (عن طريق التايم لاين)
    private void OnEnable()
    {
        if (SubtitleManager.Instance != null)
        {
            SubtitleManager.Instance.ShowSubtitle(subtitleKey);
        }
    }

    // عندما يتم إطفاء هذا المجسم
    private void OnDisable()
    {
        if (SubtitleManager.Instance != null)
        {
            SubtitleManager.Instance.HideSubtitle();
        }
    }
}