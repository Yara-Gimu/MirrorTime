using UnityEngine;
using UnityEngine.InputSystem; // ضروري لنظام التحكم الجديد

public class MirrorInteraction : MonoBehaviour
{
    [Header("عناصر واجهة المستخدم")]
    public GameObject interactUI; 
    public GameObject comicCanvas; 

    [Header("نظام الإدخال (الجديد)")]
    public InputActionReference interactAction; // اسحبي زر E / أو مربع البلايستيشن هنا

    private bool isPlayerNear = false;

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
    }

    // تفعيل زر التفاعل
    void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.performed += OnInteractPressed;
    }

    // إغلاق زر التفاعل
    void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.performed -= OnInteractPressed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    // هذه الدالة تشتغل تلقائياً متى ما ضغط اللاعب الزر (سواء كيبورد أو يد تحكم)
    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (isPlayerNear)
        {
            BreakTheMirror();
        }
    }

    void BreakTheMirror()
    {
        // 🏗️ الترقية: إرسال إشارة لمدير البيانات أن الكوميكس بدأ (لتتبع سلوك اللاعب)
        // EventManager.Trigger("Telemetry_Cinematic_Started");

        if (FadeManager.instance != null)
        {
            FadeManager.instance.ShowUIWithFade(comicCanvas, interactUI);
        }
        else
        {
            if (comicCanvas != null) comicCanvas.SetActive(true);
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}