using UnityEngine;
using UnityEngine.InputSystem; // ضروري لمعرفة يد التحكم

public class DynamicInteractPrompt : MonoBehaviour
{
    [Header("--- إعدادات الأيقونة (Sprite 3D) ---")]
    public SpriteRenderer iconSprite; 

    [Header("--- أيقونات الأجهزة ---")]
    public Sprite kbIcon;   // حطي صورة حرف E أو الكيبورد هنا
    public Sprite xboxIcon; // حطي صورة X للاكس بوكس
    public Sprite psIcon;   // حطي صورة مربع للبلايستيشن

    [Header("--- إعدادات الحركة والظهور ---")]
    public float fadeSpeed = 5f; 
    public float floatSpeed = 2f; 
    public float floatHeight = 0.1f; 

    private bool isPlayerNear = false;
    private float targetAlpha = 0f;
    private Vector3 startPos;
    private Camera mainCamera;
    
    // مرجع للاعب عشان نقرأ منه نوع التحكم
    private PlayerInput playerInput;

    void Start()
    {
        mainCamera = Camera.main;
        
        // نبحث عن اللاعب في المشهد وناخذ الـ PlayerInput حقه
        playerInput = FindFirstObjectByType<PlayerInput>();

        if (iconSprite != null)
        {
            startPos = iconSprite.transform.localPosition;
            Color c = iconSprite.color;
            c.a = 0f;
            iconSprite.color = c;
        }
    }

    void Update()
    {
        if (iconSprite == null || mainCamera == null) return;

        // 1. تحديث الأيقونة حسب يد التحكم (الفكرة حقتك)
        UpdateIconBasedOnDevice();

        // 2. الظهور والاختفاء الناعم
        targetAlpha = isPlayerNear ? 1f : 0f;
        Color currentColor = iconSprite.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        iconSprite.color = currentColor;

        // 3. حركة الطفو ومواجهة الكاميرا (Billboard)
        if (currentColor.a > 0.01f)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            iconSprite.transform.localPosition = new Vector3(startPos.x, newY, startPos.z);

            iconSprite.transform.LookAt(iconSprite.transform.position + mainCamera.transform.rotation * Vector3.forward,
                                        mainCamera.transform.rotation * Vector3.up);
        }
    }

    // دالة تحديث الأيقونة الذكية
    private void UpdateIconBasedOnDevice()
    {
        if (playerInput == null) return;

        string currentDevice = playerInput.currentControlScheme;

        if (currentDevice == "Keyboard&Mouse")
        {
            iconSprite.sprite = kbIcon;
        }
        else if (currentDevice == "Gamepad")
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad || gamepad.name.Contains("DualSense"))
                {
                    iconSprite.sprite = psIcon;
                }
                else
                {
                    iconSprite.sprite = xboxIcon;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNear = false;
    }
}