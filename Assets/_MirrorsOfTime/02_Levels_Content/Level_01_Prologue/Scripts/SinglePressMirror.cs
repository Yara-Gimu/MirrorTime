using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; 

public class SinglePressMirror : MonoBehaviour
{
    [Header("--- عناصر البيئة والـ 3D UI ---")]
    public SpriteRenderer iconSprite;
    public GameObject cinematicManager; 

    [Header("--- أيقونات الأجهزة ---")]
    public Sprite kbIcon;   
    public Sprite xboxIcon; 
    public Sprite psIcon;   

    [Header("--- إعدادات الإدخال والتفاعل ---")]
    public InputActionReference interactAction;
    public float fadeSpeed = 5f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.1f;

    [Header("--- 🎥 الكاميرات (المخرج) ---")]
    [Tooltip("الكاميرا الجانبية اللي تشتغل أول ما تضغطين E")]
    public CinemachineCamera sideCamera;
    
    [Tooltip("كاميرا الاستعراض اللي تشتغل بعد نزول المرآة")]
    public CinemachineCamera reflectionCamera;

    [Header("--- 🌟 إعدادات وقوف الممثل (Director's Mark) ---")]
    public Transform playerStandingMark;
    public float playerPositioningDuration = 0.8f;

    [Header("--- إعدادات نزول المرآة الحقيقية ---")]
    public Transform mirrorTransform; 
    public Transform targetLandingPosition;
    public float descentDuration = 2.5f;
    
    public MonoBehaviour mirrorFloatScript; 

    private bool isPlayerNear = false;
    private bool isMirrorBroken = false;
    private float targetAlpha = 0f;
    private Vector3 startPos;
    private Camera mainCamera;
    private PlayerInput playerInput;
    private GameObject playerRef; 

    void Start()
    {
        mainCamera = Camera.main;
        playerInput = FindFirstObjectByType<PlayerInput>();

        if (iconSprite != null)
        {
            startPos = iconSprite.transform.localPosition;
            Color c = iconSprite.color;
            c.a = 0f;
            iconSprite.color = c;
        }
        
        if (interactAction != null) interactAction.action.Enable();
    }

    void Update()
    {
        if (isMirrorBroken || iconSprite == null || mainCamera == null) return;

        UpdateIconBasedOnDevice();

        targetAlpha = isPlayerNear ? 1f : 0f;
        Color currentColor = iconSprite.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        iconSprite.color = currentColor;

        if (currentColor.a > 0.01f)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            iconSprite.transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
            iconSprite.transform.LookAt(iconSprite.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            playerRef = other.gameObject; 
            if (interactAction != null) interactAction.action.started += OnInteractPressed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            playerRef = null;
            if (interactAction != null) interactAction.action.started -= OnInteractPressed;
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (isPlayerNear && !isMirrorBroken) BreakTheMirror();
    }

    void BreakTheMirror()
    {
        isMirrorBroken = true;
        
        if (interactAction != null) interactAction.action.started -= OnInteractPressed;
        if (iconSprite != null) iconSprite.enabled = false;

        StartCoroutine(FullSceneSetupSequence());
    }

    private IEnumerator FullSceneSetupSequence()
    {
        // 1. الكاميرا الجانبية
        if (sideCamera != null) sideCamera.Priority = 200;
        if (mirrorFloatScript != null) mirrorFloatScript.enabled = false;

        // 2. توجيه اللاعب
        if (playerRef != null && playerStandingMark != null)
        {
            CharacterController cc = playerRef.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            MonoBehaviour stateMachine = playerRef.GetComponent("PlayerStateMachine") as MonoBehaviour;
            if (stateMachine != null) stateMachine.enabled = false;

            Vector3 startPlayerPos = playerRef.transform.position;
            Quaternion startPlayerRot = playerRef.transform.rotation;
            float elapsedPlayer = 0f;

            while (elapsedPlayer < playerPositioningDuration)
            {
                elapsedPlayer += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsedPlayer / playerPositioningDuration);
                playerRef.transform.position = Vector3.Lerp(startPlayerPos, playerStandingMark.position, t);
                playerRef.transform.rotation = Quaternion.Slerp(startPlayerRot, playerStandingMark.rotation, t);
                yield return null;
            }
            playerRef.transform.position = playerStandingMark.position;
            playerRef.transform.rotation = playerStandingMark.rotation;
        }

        // 3. نزول المرآة
        if (mirrorTransform != null && targetLandingPosition != null)
        {
            Vector3 startMirrorPos = mirrorTransform.position;
            Vector3 endMirrorPos = targetLandingPosition.position;
            float elapsedMirror = 0f;

            while (elapsedMirror < descentDuration)
            {
                elapsedMirror += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsedMirror / descentDuration);
                mirrorTransform.position = Vector3.Lerp(startMirrorPos, endMirrorPos, t);
                yield return null;
            }
            mirrorTransform.position = endMirrorPos;
        }

        // 🌟 4. تشغيل كاميرا الاستعراض الثانية من هنا! (بناءً على فكرتك)
        if (reflectionCamera != null)
        {
            // نشغل أبوها إذا كان مطفي
            if (reflectionCamera.transform.parent != null)
            {
                reflectionCamera.transform.parent.gameObject.SetActive(true);
            }
            reflectionCamera.gameObject.SetActive(true);
            reflectionCamera.Priority = 9999; // تقطع فوراً على الكاميرا الجانبية
        }

        // 🌟 5. تسليم الراية لفني المؤثرات
        if (cinematicManager != null)
        {
            cinematicManager.SetActive(true);
            MirrorCinematicSequence cinematicScript = cinematicManager.GetComponent<MirrorCinematicSequence>();
            if (cinematicScript != null) cinematicScript.StartSequenceFromMirror(); 
        }
        
        this.enabled = false;
    }

    private void UpdateIconBasedOnDevice()
    {
        if (playerInput == null) return;
        string currentDevice = playerInput.currentControlScheme;
        if (currentDevice == "Keyboard&Mouse" || currentDevice == "Keyboard") iconSprite.sprite = kbIcon;
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