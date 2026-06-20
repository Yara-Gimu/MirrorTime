using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CreditsUIController : MonoBehaviour
{
    [Header("--- إعدادات التمرير (Scrolling) ---")]
    public ScrollRect creditsScrollRect; 
    public float scrollSpeed = 0.5f;
    [Tooltip("اسحبي حدث Navigate أو Move الخاص بالـ UI هنا")]
    public InputActionReference navigateAction; 

    [Header("--- إعدادات الخروج (Cancel) ---")]
    [Tooltip("اسحبي حدث Cancel الخاص بالـ UI هنا (غالباً زر الدائرة/B)")]
    public InputActionReference cancelAction; 
    [Tooltip("اسحبي زر العودة الخاص بشاشة المطورين هنا")]
    public Button backButton;

    void OnEnable()
    {
        if (cancelAction != null)
        {
            cancelAction.action.Enable();
            cancelAction.action.performed += OnCancelPressed;
        }
        
        if (navigateAction != null)
        {
            navigateAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (cancelAction != null)
        {
            cancelAction.action.performed -= OnCancelPressed;
        }
    }

    void Update()
    {
        // 🌟 قراءة حركة الأنالوج/الأسهم للتمرير للأسفل والأعلى بسلاسة
        if (navigateAction != null && creditsScrollRect != null)
        {
            Vector2 input = navigateAction.action.ReadValue<Vector2>();
            if (input.y != 0)
            {
                // تحريك النص وتثبيته بين 0 (الأسفل) و 1 (الأعلى)
                creditsScrollRect.verticalNormalizedPosition += input.y * scrollSpeed * Time.deltaTime;
                creditsScrollRect.verticalNormalizedPosition = Mathf.Clamp01(creditsScrollRect.verticalNormalizedPosition);
            }
        }
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        // 🌟 إذا ضغط اللاعب دائرة/B، نضغط زر العودة برمجياً
        if (backButton != null)
        {
            backButton.onClick.Invoke();
        }
    }
}