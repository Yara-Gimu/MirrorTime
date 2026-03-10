using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GateController : MonoBehaviour
{
    [Header("--- إعدادات القفل ---")]
    public bool isAlwaysOpen = false;   // ضعي صح لبوابة العلا فقط
    public int levelRequiredToOpen = 1; // رقم المرحلة المطلوبة (ثاج=1, الفاو=2, تاروت=3)

    [Header("--- المجسمات والإضاءة ---")]
    public GameObject spotLightObject;  
    public GameObject portalGlowObject; 
    public GameObject darkPatternsObject; 
    public GameObject glowPatternsObject; 

    [Header("--- إعدادات الانتقال (الجديدة) ---")]
    public string sceneToLoad; 
    public InputActionReference interactAction; 
    
    [Tooltip("لون الوميض اللي بيظهر وقت الانتقال (اختاري لون يناسب المرحلة)")]
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
        if (interactAction != null) interactAction.action.performed += OnInteractPressed;
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.performed -= OnInteractPressed;
    }

    public void CheckPermission()
    {
        // 🏗️ الترقية المعمارية: القراءة من المدير المركزي
        int currentProgress = 0;
        
        if (SaveManager.Instance != null)
        {
            currentProgress = SaveManager.Instance.currentGateProgress;
        }
        else
        {
            // خطة طوارئ: في حال تم تشغيل المشهد للتجربة بدون الـ SaveManager
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
        if (darkPatternsObject != null) darkPatternsObject.SetActive(false);
        if (glowPatternsObject != null) glowPatternsObject.SetActive(true);

        if (ambientLoopSound != null && !ambientLoopSound.isPlaying) ambientLoopSound.Play();
    }

    public void LockGate()
    {
        isCurrentlyUnlocked = false;

        if (spotLightObject != null) spotLightObject.SetActive(false);
        if (portalGlowObject != null) portalGlowObject.SetActive(false);
        if (darkPatternsObject != null) darkPatternsObject.SetActive(true);
        if (glowPatternsObject != null) glowPatternsObject.SetActive(false);

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

        // 🏗️ الترقية المعمارية: إرسال إشارة لمدير البيانات عند الدخول للبوابة (مؤجلة حالياً كتعليق)
        // EventManager.Trigger("Telemetry_Gate_Entered", sceneToLoad);

        SceneManager.LoadScene(sceneToLoad);
    }
}