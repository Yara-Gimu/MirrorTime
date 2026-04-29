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

    // 🌟 المتغير السري: هل انتهى التأخير الأول؟
    private bool introDelayFinished = false;

    void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (iconSprite != null)
        {
            Color c = iconSprite.color;
            c.a = 0f;
            iconSprite.color = c;
        }
    }

    void Update()
    {
        if (iconSprite == null) return;

        UpdateIconBasedOnDevice();

        // التلاشي الناعم (الفيد)
        targetAlpha = isPlayerNear ? 1f : 0f;
        Color currentColor = iconSprite.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        iconSprite.color = currentColor;

        // Billboard: مواجهة الكاميرا دائماً
        iconSprite.transform.LookAt(iconSprite.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                    Camera.main.transform.rotation * Vector3.up);
    }

    private void UpdateIconBasedOnDevice()
    {
        if (playerInput == null) return;
        string currentDevice = playerInput.currentControlScheme;

        if (currentDevice == "Keyboard&Mouse") iconSprite.sprite = kbIcon;
        else if (currentDevice == "Gamepad")
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
            // 🌟 التحقق: إذا كان هذا اللقاء الأول، انتظر. إذا لا، أظهر فوراً.
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
        introDelayFinished = true; // 🌟 علامة أننا انتهينا من التأخير السينمائي للأبد
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // إذا خرج اللاعب والعملية لم تنتهِ، نوقفها.
            StopAllCoroutines();
            isPlayerNear = false;
        }
    }
}