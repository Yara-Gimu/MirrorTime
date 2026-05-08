using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ComingSoonGate : MonoBehaviour
{
    [Header("--- إعدادات واجهة قريباً ---")]
    public GameObject comingSoonCanvas;
    public Button returnButton; // زر الماوس

    [Header("--- إعدادات التفاعل ---")]
    public InputActionReference interactAction; // زر E
    public InteractPromptController interactPrompt; 

    private bool isPlayerInRange = false;

    void Start()
    {
        // ربط زر الواجهة (الماوس) لتقفيل الشاشة
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(CloseScreen);
        }
    }

    private void OnEnable()
    {
        if (interactAction != null) interactAction.action.Enable(); 
        if (interactAction != null) interactAction.action.performed += OnInteractPressed;
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.performed -= OnInteractPressed;
    }

    void Update()
    {
        // 🌟 مراقبة زر ESC: إذا الشاشة مفتوحة واللاعب ضغط ESC تقفل فوراً
        if (comingSoonCanvas != null && comingSoonCanvas.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseScreen();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // نظهر زر التفاعل (E) بس إذا الشاشة مو مفتوحة
            if (interactPrompt != null && (comingSoonCanvas == null || !comingSoonCanvas.activeSelf)) 
                interactPrompt.ShowPrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactPrompt != null) interactPrompt.HidePrompt();
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        // إذا اللاعب حول البوابة، وضغط E، والشاشة مقفلة -> نفتحها
        if (isPlayerInRange && comingSoonCanvas != null && !comingSoonCanvas.activeSelf)
        {
            ShowComingSoonScreen();
        }
    }

    private void ShowComingSoonScreen()
    {
        comingSoonCanvas.SetActive(true);
        
        if (interactPrompt != null) interactPrompt.ForceHide();
        
        // فك الماوس
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // إيقاف اللعبة
        Time.timeScale = 0f; 
    }

    public void CloseScreen()
    {
        if (comingSoonCanvas != null) comingSoonCanvas.SetActive(false);
        
        // إخفاء الماوس
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // استكمال اللعبة
        Time.timeScale = 1f; 
        
        // إرجاع حرف (E) إذا نوار لسه عند البوابة
        if (isPlayerInRange && interactPrompt != null) interactPrompt.ShowPrompt();
    }
}