using UnityEngine;
using UnityEngine.InputSystem;

public class InteractPromptController : MonoBehaviour
{
    [Header("--- إعدادات الأيقونة ---")]
    [Tooltip("مجسم الـ Sprite اللي يحتوي على الأيقونة")]
    public SpriteRenderer promptRenderer; 
    
    [Tooltip("سرعة الظهور والاختفاء")]
    public float fadeSpeed = 8f; 

    [Header("--- حركة الطفو السحرية ---")]
    public float floatSpeed = 2f;    
    public float floatAmplitude = 0.1f; 

    [Header("--- أيقونات الأجهزة المتعددة ---")]
    public Sprite keyboardSprite;   
    public Sprite xboxSprite; 
    public Sprite playstationSprite;   

    // --- متغيرات الحالة (State) ---
    private bool isVisible = false;
    private float currentAlpha = 0f;
    private Vector3 initialLocalPos;
    private string lastDeviceUsed = ""; 
    private PlayerInput playerInput;
    private Transform mainCameraTransform;

    void Start()
    {
        // 1. التجهيز والربط
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (Camera.main != null) mainCameraTransform = Camera.main.transform;
        
        if (promptRenderer != null)
        {
            initialLocalPos = promptRenderer.transform.localPosition;
            SetAlpha(0f);
        }
    }

    void Update()
    {
        if (promptRenderer == null) return;

        // 2. تحديث الأيقونة فقط إذا تغير الجهاز (توفير أداء)
        CheckAndUpdateDeviceIcon();

        // 3. معالجة التلاشي (Fade)
        float targetAlpha = isVisible ? 1f : 0f;
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        SetAlpha(currentAlpha);

        // إذا كانت الأيقونة مخفية تماماً، لا داعي لحساب الطفو والدوران (توفير أداء)
        if (currentAlpha <= 0.01f) return;

        // 4. معالجة الطفو (Floating)
        float newY = initialLocalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        promptRenderer.transform.localPosition = new Vector3(initialLocalPos.x, newY, initialLocalPos.z);

        // 5. معالجة مواجهة الكاميرا (Billboard)
        if (mainCameraTransform != null)
        {
            promptRenderer.transform.LookAt(promptRenderer.transform.position + mainCameraTransform.rotation * Vector3.forward,
                                            mainCameraTransform.rotation * Vector3.up);
        }
    }

    // --- دوال التحكم العامة (Public API) للمدراء (مثل البوابة) ---
    public void ShowPrompt() => isVisible = true;
    public void HidePrompt() => isVisible = false;
    public void ForceHide() 
    {
        isVisible = false;
        SetAlpha(0f);
    }

    // --- دوال المساعدة الداخلية ---
    private void SetAlpha(float alpha)
    {
        Color c = promptRenderer.color;
        c.a = alpha;
        promptRenderer.color = c;
    }

    private void CheckAndUpdateDeviceIcon()
    {
        if (playerInput == null) return;

        string currentScheme = playerInput.currentControlScheme;
        
        if (currentScheme == lastDeviceUsed) return;
        
        lastDeviceUsed = currentScheme;

        if (currentScheme == "Keyboard&Mouse") 
        {
            promptRenderer.sprite = keyboardSprite;
        }
        else if (currentScheme == "Gamepad")
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad || gamepad.name.Contains("DualSense"))
                    promptRenderer.sprite = playstationSprite;
                else
                    promptRenderer.sprite = xboxSprite;
            }
        }
    }
}