using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // ⚠️ إضافة مهمة جداً لمدير الأحداث

public class GateController : MonoBehaviour
{
    [Header("--- إعدادات القفل ---")]
    public bool isAlwaysOpen = false;   
    public int levelRequiredToOpen = 1; 

    [Header("--- المجسمات والإضاءة ---")]
    public GameObject spotLightObject;  
    public GameObject portalGlowObject; 

    [Header("--- فكرة يوري: تبديل ماتيريال النقش فقط ---")]
    [Tooltip("مجسم البوابة اللي عليه النقش")]
    public MeshRenderer gateRenderer; 
    
    [Tooltip("رقم الماتيريال حق النقش في قائمة الإنسبكتور (غالباً 1 إذا الخشب 0)")]
    public int patternMaterialIndex = 1; 
    
    [Tooltip("ماتيريال النقش وهو طافي (عادي)")]
    public Material darkPatternMaterial; 
    
    [Tooltip("ماتيريال النقش وهو شغال (مضيء)")]
    public Material glowingPatternMaterial; 

    [Header("--- إعدادات الانتقال ---")]
    public string sceneToLoad; 
    public InputActionReference interactAction; 
    public Color fadeColor = Color.white; 

    [Header("--- واجهة المستخدم (UI) ---")]
    public GameObject interactPrompt; 
    public CanvasGroup whiteFade; 
    public float fadeSpeed = 1.5f;

    [Header("--- الصوت (Audio) ---")]
    public AudioSource ambientLoopSound; 
    public AudioSource teleportSound;    

    private bool isPlayerInRange = false;
    private bool isTransitioning = false;
    private bool isCurrentlyUnlocked = false; 

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (whiteFade != null) whiteFade.alpha = 0f;

        CheckPermission(); 
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

    public void CheckPermission()
    {
        int currentProgress = 0;
        
        if (SaveManager.Instance != null)
        {
            currentProgress = SaveManager.Instance.currentGateProgress;
        }
        else
        {
            currentProgress = PlayerPrefs.GetInt("GateProgress", 0);
        }

        if (isAlwaysOpen || currentProgress >= levelRequiredToOpen)
        {
            UnlockGate(); 
        }
        else
        {
            LockGate();   
        }
    }

    public void UnlockGate()
    {
        isCurrentlyUnlocked = true;

        if (spotLightObject != null) spotLightObject.SetActive(true);
        if (portalGlowObject != null) portalGlowObject.SetActive(true);

        // 🌟 فكرتك: تغيير ماتيريال النقش إلى المضيء
        if (gateRenderer != null && glowingPatternMaterial != null)
        {
            Material[] mats = gateRenderer.materials; // نسحب قائمة الماتيريالز
            if (mats.Length > patternMaterialIndex)
            {
                mats[patternMaterialIndex] = glowingPatternMaterial; // نغير النقش بس
                gateRenderer.materials = mats; // نرجع القائمة للمجسم
            }
        }

        if (ambientLoopSound != null && !ambientLoopSound.isPlaying) ambientLoopSound.Play();
    }

    public void LockGate()
    {
        isCurrentlyUnlocked = false;

        if (spotLightObject != null) spotLightObject.SetActive(false);
        if (portalGlowObject != null) portalGlowObject.SetActive(false);

        // 🌟 فكرتك: تغيير ماتيريال النقش إلى الطافي
        if (gateRenderer != null && darkPatternMaterial != null)
        {
            Material[] mats = gateRenderer.materials; 
            if (mats.Length > patternMaterialIndex)
            {
                mats[patternMaterialIndex] = darkPatternMaterial; 
                gateRenderer.materials = mats; 
            }
        }

        if (ambientLoopSound != null) ambientLoopSound.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning && isCurrentlyUnlocked)
        {
            isPlayerInRange = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isPlayerInRange = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && !isTransitioning && isCurrentlyUnlocked)
        {
            StartCoroutine(TransitionRoutine());
        }
    }

    IEnumerator TransitionRoutine()
    {
        isTransitioning = true;
        
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (teleportSound != null) teleportSound.Play();

        PlayerStateMachine player = FindObjectOfType<PlayerStateMachine>();
        if (player != null) player.enabled = false;

        if (whiteFade != null)
        {
            Image fadeImage = whiteFade.GetComponent<Image>();
            if (fadeImage != null)
            {
                fadeImage.color = fadeColor;
            }

            whiteFade.alpha = 0f;
            while (whiteFade.alpha < 1f)
            {
                whiteFade.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f); 
        }

        // 📡 الترقية المعمارية: إرسال بيانات التتبع (Telemetry) لمدير الأحداث
        Dictionary<string, object> gateData = new Dictionary<string, object>
        {
            { "DestinationScene", sceneToLoad },
            { "TimeEntered", Time.time }
        };
        EventManager.TriggerEvent("Telemetry_Gate_Entered", gateData);

        SceneManager.LoadScene(sceneToLoad);
    }
}