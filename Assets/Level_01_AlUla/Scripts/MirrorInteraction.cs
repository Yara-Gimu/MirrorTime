using UnityEngine;
using UnityEngine.InputSystem;

public class AAA_SinglePressMirror : MonoBehaviour
{
    [Header("--- عناصر البيئة والـ 3D UI ---")]
    [Tooltip("اسحبي مجسم الـ Sprite Renderer (صورة الزر) هنا")]
    public SpriteRenderer iconSprite;
    [Tooltip("مجسم الكوميكس اللي تبينه يظهر في النهاية")]
    public GameObject comicCanvas;

    [Header("--- أيقونات الأجهزة ---")]
    public Sprite kbIcon;   // حرف E
    public Sprite xboxIcon; // X
    public Sprite psIcon;   // مربع

    [Header("--- إعدادات الإدخال والتفاعل ---")]
    [Tooltip("اسحبي الـInputAction حق التفاعل (مثلاً E) هنا")]
    public InputActionReference interactAction;

    [Header("--- إعدادات حركة الطفو (سينمائية) ---")]
    public float fadeSpeed = 5f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.1f;

    private bool isPlayerNear = false;
    private bool isMirrorBroken = false;
    private float targetAlpha = 0f;
    private Vector3 startPos;
    private Camera mainCamera;
    private PlayerInput playerInput;

    void Start()
    {
        mainCamera = Camera.main;
        playerInput = FindFirstObjectByType<PlayerInput>();

        // إخفاء الأيقونة في البداية
        if (iconSprite != null)
        {
            startPos = iconSprite.transform.localPosition;
            Color c = iconSprite.color;
            c.a = 0f;
            iconSprite.color = c;
        }
    }

    void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractPressed;
        }
    }

    void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteractPressed;
        }
    }

    void Update()
    {
        if (isMirrorBroken || iconSprite == null || mainCamera == null) return;

        // 1. تحديث شكل الأيقونة حسب الجهاز (كيبورد/سوني/اكس بوكس)
        UpdateIconBasedOnDevice();

        // 2. الظهور والاختفاء الناعم بناءً على قرب نوار
        targetAlpha = isPlayerNear ? 1f : 0f;
        Color currentColor = iconSprite.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        iconSprite.color = currentColor;

        // 3. حركة الطفو ومواجهة الكاميرا
        if (currentColor.a > 0.01f)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            iconSprite.transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
            iconSprite.transform.LookAt(iconSprite.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
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

    // الدالة تشتغل بضغطة واحدة فقط (بدون انتظار)
    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (isPlayerNear && !isMirrorBroken)
        {
            BreakTheMirror();
        }
    }

    void BreakTheMirror()
    {
        isMirrorBroken = true;
        
        // إخفاء الأيقونة فوراً
        if (iconSprite != null) iconSprite.enabled = false;

        // تشغيل الكوميكس باستخدام الـ FadeManager
        if (FadeManager.instance != null)
        {
            FadeManager.instance.ShowUIWithFade(comicCanvas, null);
        }
        else
        {
            if (comicCanvas != null) comicCanvas.SetActive(true);
        }

        // إيقاف السكربت عشان ما ينضغط الزر مرتين
        this.enabled = false;
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
                if (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad || gamepad.name.Contains("DualSense")) iconSprite.sprite = psIcon;
                else iconSprite.sprite = xboxIcon;
            }
        }
    }
}