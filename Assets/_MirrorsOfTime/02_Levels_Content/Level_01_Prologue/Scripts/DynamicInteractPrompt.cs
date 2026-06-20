using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DynamicInteractPrompt : MonoBehaviour
{
    [Header("--- إعدادات الأيقونة (Sprite 3D) ---")]
    public SpriteRenderer iconSprite; 

    [Header("--- أيقونات الأجهزة ---")]
    public Sprite kbIcon;   
    public Sprite xboxIcon; 
    public Sprite psIcon;   

    [Header("--- إعدادات الظهور ---")]
    public float fadeSpeed = 5f; 
    public float showDelay = 4.0f; 

    private bool isPlayerNear = false;
    private float targetAlpha = 0f;
    private PlayerInput playerInput;

    private bool introDelayFinished = false;
    
    // 🌟 المتغير المنقذ للأداء: لتتبع الجهاز الحالي فقط
    private string currentControlScheme = "";

    void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (iconSprite != null)
        {
            Color c = iconSprite.color;
            c.a = 0f;
            iconSprite.color = c;
        }
        
        // تحديث الصورة مرة واحدة في البداية
        UpdateIconBasedOnDevice();
    }

    void Update()
    {
        if (iconSprite == null) return;

        // 🌟 فحص ذكي: لا نحدث الصورة إلا إذا تغير الجهاز فعلاً!
        if (playerInput != null && playerInput.currentControlScheme != currentControlScheme)
        {
            UpdateIconBasedOnDevice();
        }

        // التلاشي الناعم
        targetAlpha = isPlayerNear ? 1f : 0f;
        Color currentColor = iconSprite.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        iconSprite.color = currentColor;

        // مواجهة الكاميرا (Billboard)
        iconSprite.transform.LookAt(iconSprite.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                    Camera.main.transform.rotation * Vector3.up);
    }

    private void UpdateIconBasedOnDevice()
    {
        if (playerInput == null) return;
        
        // تحديث المتغير لحفظ الجهاز الجديد
        currentControlScheme = playerInput.currentControlScheme;

        if (currentControlScheme == "Keyboard&Mouse" || currentControlScheme == "Keyboard") 
            iconSprite.sprite = kbIcon;
        else if (currentControlScheme == "Gamepad")
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad || gamepad.name.Contains("DualSense"))
                    iconSprite.sprite = psIcon;
                else
                    iconSprite.sprite = xboxIcon;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!introDelayFinished)
            {
                StartCoroutine(WaitToShowFirstTime());
            }
            else
            {
                isPlayerNear = true; 
            }
        }
    }

    IEnumerator WaitToShowFirstTime()
    {
        yield return new WaitForSeconds(showDelay);
        isPlayerNear = true;
        introDelayFinished = true; 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            isPlayerNear = false;
        }
    }
}