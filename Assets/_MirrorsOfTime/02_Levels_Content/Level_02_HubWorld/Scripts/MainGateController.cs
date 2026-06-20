using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainGateController : MonoBehaviour
{
    [Header("--- المجسمات والإضاءة ---")]
    public GameObject portalGlowObject; 
    public GameObject centerSpotLightObject; 

    [Header("--- منطق اللعبة ---")]
    public int levelRequiredToOpen = 4;
    public string finalSceneName = "FinalEndingScene"; 

    [Header("--- إعدادات التفاعل والتحميل ---")]
    public InputActionReference interactAction; 
    [Tooltip("اسحبي أيقونة الدائرة المفرغة (LoadingCircle) هنا")]
    public Image loadingCircle; 
    public float holdDuration = 1.5f; // المدة المطلوبة للضغط المطول
    
    [Header("--- إعدادات الواجهة والانتقال ---")]
    public InteractPromptController interactPrompt; 
    public CanvasGroup whiteFade; 
    public float fadeSpeed = 1.5f;
    public Color fadeColor = Color.white;

    [Header("--- الأصوات ---")]
    public AudioSource ambientLoopSound; 
    public AudioSource teleportSound;    

    private bool isPlayerInRange = false;
    private bool isTransitioning = false;
    private bool isFullyOpen = false; 
    private Coroutine fillCoroutine;

    void Start()
    {
        if (interactPrompt != null) interactPrompt.ForceHide();
        if (whiteFade != null) whiteFade.alpha = 0f;
        if (loadingCircle != null) loadingCircle.fillAmount = 0f;
        
        CheckGateStatus();
    }

    private void OnEnable()
    {
        if (interactAction != null) 
        {
            interactAction.action.Enable();
            interactAction.action.started += OnInteractStarted;
            interactAction.action.canceled += OnInteractCanceled;
        }
        EventManager.StartListening("Level_Completed", OnLevelCompletedEvent);
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.started -= OnInteractStarted;
            interactAction.action.canceled -= OnInteractCanceled;
        }
        EventManager.StopListening("Level_Completed", OnLevelCompletedEvent);
    }

    private void OnLevelCompletedEvent(Dictionary<string, object> data)
    {
        CheckGateStatus(); 
    }

    public void CheckGateStatus()
    {
        int currentProgress = SaveManager.Instance != null ? SaveManager.Instance.currentGateProgress : PlayerPrefs.GetInt("GateProgress", 0);

        isFullyOpen = (currentProgress >= levelRequiredToOpen);

        if(portalGlowObject) portalGlowObject.SetActive(isFullyOpen);
        if(centerSpotLightObject) centerSpotLightObject.SetActive(isFullyOpen);
        
        if (isFullyOpen && ambientLoopSound != null && !ambientLoopSound.isPlaying) ambientLoopSound.Play();
        else if (!isFullyOpen && ambientLoopSound != null) ambientLoopSound.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning && isFullyOpen)
        {
            isPlayerInRange = true;
            if (interactPrompt != null) interactPrompt.ShowPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactPrompt != null) interactPrompt.HidePrompt();
            
            if (fillCoroutine != null) StopCoroutine(fillCoroutine);
            if (loadingCircle != null) loadingCircle.fillAmount = 0f;
        }
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && !isTransitioning && isFullyOpen)
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
        float timer = 0f;
        while (timer < holdDuration)
        {
            timer += Time.deltaTime;
            if (loadingCircle != null) loadingCircle.fillAmount = timer / holdDuration;
            yield return null;
        }
        
        if (loadingCircle != null) loadingCircle.fillAmount = 1f;
        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        isTransitioning = true;
        if (interactPrompt != null) interactPrompt.ForceHide();
        if (loadingCircle != null) loadingCircle.gameObject.SetActive(false);
        if (teleportSound != null) teleportSound.Play();

        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
        if (player != null) player.enabled = false;

        if (whiteFade != null)
        {
            Image fadeImage = whiteFade.GetComponent<Image>();
            if (fadeImage != null) fadeImage.color = fadeColor;

            while (whiteFade.alpha < 1f)
            {
                whiteFade.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
        
        Dictionary<string, object> finalGateData = new Dictionary<string, object>
        {
            { "Scene", finalSceneName },
            { "PlayerLevel", SaveManager.Instance != null ? SaveManager.Instance.currentGateProgress : 0 },
            { "TimePlayed", Time.timeSinceLevelLoad }
        };
        EventManager.TriggerEvent("Telemetry_Final_Gate_Entered", finalGateData);

        SceneManager.LoadScene(finalSceneName);
    }
}