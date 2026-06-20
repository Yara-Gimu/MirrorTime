using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GateController : MonoBehaviour
{
    [Header("--- إعدادات القفل ---")]
    public bool isAlwaysOpen = false;   
    public int levelRequiredToOpen = 1; 

    [Header("--- المجسمات والإضاءة ---")]
    public GameObject spotLightObject;  
    public GameObject portalGlowObject; 

    [Header("--- إعدادات الانتقال ---")]
    public string sceneToLoad; 
    public InputActionReference interactAction; 
    public Color fadeColor = Color.white; 

    [Header("--- واجهة المستخدم (UI) ---")]
    public InteractPromptController interactPrompt; 
    public CanvasGroup whiteFade; 
    public float fadeSpeed = 1.5f;
    
    [Header("--- دائرة التحميل (Hold UI) ---")]
    [Tooltip("اسحبي صورة الدائرة (LoadingCircle) التي تحتوي على خاصية Image Type: Filled")]
    public Image loadingCircle; 
    [Tooltip("كم ثانية يحتاج اللاعب للضغط؟")]
    public float holdDuration = 1f;

    [Header("--- الصوت (Audio) ---")]
    public AudioSource ambientLoopSound; 
    public AudioSource teleportSound;   

    private bool isPlayerInRange = false;
    private bool isTransitioning = false;
    private bool isCurrentlyUnlocked = false; 
    
    private PlayerStateMachine cachedPlayer;
    private Coroutine fillCoroutine; 

    void Start()
    {
        if (interactPrompt != null) interactPrompt.ForceHide();
        if (whiteFade != null) whiteFade.alpha = 0f;
        if (loadingCircle != null) loadingCircle.fillAmount = 0f; 
        
        cachedPlayer = FindFirstObjectByType<PlayerStateMachine>();

        CheckPermission(); 
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable(); 
            // نستخدم فقط Started (للبدء) و Canceled (للإلغاء)
            interactAction.action.started += OnInteractStarted; 
            interactAction.action.canceled += OnInteractCanceled;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.started -= OnInteractStarted;
            interactAction.action.canceled -= OnInteractCanceled;
        }
    }

    public void CheckPermission()
    {
        int currentProgress = SaveManager.Instance != null ? SaveManager.Instance.currentGateProgress : 0;

        if (isAlwaysOpen || currentProgress >= levelRequiredToOpen)
            UnlockGate(); 
        else
            LockGate();   
    }

    public void UnlockGate()
    {
        isCurrentlyUnlocked = true;
        if (spotLightObject != null) spotLightObject.SetActive(true);
        if (portalGlowObject != null) portalGlowObject.SetActive(true);
        if (ambientLoopSound != null && !ambientLoopSound.isPlaying) ambientLoopSound.Play();
    }

    public void LockGate()
    {
        isCurrentlyUnlocked = false;
        if (spotLightObject != null) spotLightObject.SetActive(false);
        if (portalGlowObject != null) portalGlowObject.SetActive(false);
        if (ambientLoopSound != null) ambientLoopSound.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning && isCurrentlyUnlocked)
        {
            isPlayerInRange = true;
            if (interactPrompt != null) interactPrompt.ShowPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isPlayerInRange = false;
            if (interactPrompt != null) interactPrompt.HidePrompt();
            
            // إلغاء التعبئة فوراً إذا خرج اللاعب
            if (fillCoroutine != null) StopCoroutine(fillCoroutine);
            if (loadingCircle != null) loadingCircle.fillAmount = 0f;
        }
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && !isTransitioning && isCurrentlyUnlocked)
        {
            if (fillCoroutine != null) StopCoroutine(fillCoroutine);
            fillCoroutine = StartCoroutine(FillCircleRoutine());
        }
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        if (fillCoroutine != null) StopCoroutine(fillCoroutine);
        if (loadingCircle != null) loadingCircle.fillAmount = 0f;
    }

    IEnumerator FillCircleRoutine()
    {
        if (loadingCircle == null) yield break;
        
        float timer = 0f;
        loadingCircle.fillAmount = 0f;
        
        while (timer < holdDuration)
        {
            timer += Time.deltaTime;
            loadingCircle.fillAmount = timer / holdDuration;
            yield return null;
        }

        // 🌟 الانتقال يتم هنا فقط بعد اكتمال التعبئة 100%
        loadingCircle.fillAmount = 1f;
        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        isTransitioning = true;
        
        if (interactPrompt != null) interactPrompt.ForceHide();
        if (loadingCircle != null) loadingCircle.gameObject.SetActive(false);
        
        if (teleportSound != null) teleportSound.Play();
        if (cachedPlayer != null) cachedPlayer.enabled = false;

        if (whiteFade != null)
        {
            whiteFade.alpha = 0f;
            while (whiteFade.alpha < 1f)
            {
                whiteFade.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f); 
        }

        Dictionary<string, object> gateData = new Dictionary<string, object>
        {
            { "DestinationScene", sceneToLoad },
            { "TimeEntered", Time.time }
        };
        EventManager.TriggerEvent("Telemetry_Gate_Entered", gateData);

        SceneManager.LoadScene(sceneToLoad);
    }
}