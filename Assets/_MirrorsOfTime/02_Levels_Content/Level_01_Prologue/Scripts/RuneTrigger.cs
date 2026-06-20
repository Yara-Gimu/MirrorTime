using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.InputSystem;

public class RuneTrigger : MonoBehaviour
{
    [Header("--- بيانات النقش ---")]
    public string translationKey; 
    public Sprite runeSprite; // 🌟 هنا تحطين الصورة المقصوصة الجاهزة

    [Header("--- واجهة التفاعل (زر E) ---")]
    public GameObject interactPromptUI; 
    public Image promptIconImage;   
    public float promptHeightOffset = 1.5f;

    [Header("--- أيقونات الأجهزة ---")]
    public Sprite kbIcon;   
    public Sprite xboxIcon; 
    public Sprite psIcon; 

    [Header("--- إعدادات الإدخال ---")]
    public InputActionReference interactAction; 

    private bool isPlayerNear = false;
    private bool isReading = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        if (interactPromptUI != null) interactPromptUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && !isReading)
        {
            UpdateIconBasedOnDevice();

            if (interactPromptUI != null && mainCam != null)
            {
                Vector3 targetPos = transform.position + new Vector3(0, promptHeightOffset, 0);
                Vector3 screenPos = mainCam.WorldToScreenPoint(targetPos);
                if (screenPos.z > 0) interactPromptUI.transform.position = screenPos;
            }
        }
    }

    private void UpdateIconBasedOnDevice()
    {
        if (promptIconImage == null) return;
        if (Gamepad.current != null)
        {
            if (Gamepad.current is UnityEngine.InputSystem.DualShock.DualShockGamepad || Gamepad.current.name.Contains("DualSense"))
                promptIconImage.sprite = psIcon;
            else
                promptIconImage.sprite = xboxIcon;
        }
        else promptIconImage.sprite = kbIcon;
    }

    private void OnEnable() { if (interactAction != null) { interactAction.action.Enable(); interactAction.action.started += OnInteract; } }
    private void OnDisable() { if (interactAction != null) interactAction.action.started -= OnInteract; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            UpdateIconBasedOnDevice(); 
            if (!isReading && interactPromptUI != null) interactPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            isReading = false;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
            if (RuneUIManager.Instance != null) RuneUIManager.Instance.HideRune();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isPlayerNear)
        {
            if (!isReading)
            {
                isReading = true;
                if (interactPromptUI != null) interactPromptUI.SetActive(false);
                if (RuneUIManager.Instance != null) RuneUIManager.Instance.ShowRune(translationKey, runeSprite); // 🌟 نرسل الصورة
            }
            else
            {
                isReading = false;
                if (RuneUIManager.Instance != null) RuneUIManager.Instance.HideRune();
                UpdateIconBasedOnDevice();
                if (interactPromptUI != null) interactPromptUI.SetActive(true);
            }
        }
    }
}