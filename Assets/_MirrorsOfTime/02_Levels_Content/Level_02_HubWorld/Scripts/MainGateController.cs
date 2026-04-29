using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainGateController : MonoBehaviour
{
    [Header("--- المجسمات والإضاءة ---")]
    public GameObject portalGlowObject; 
    public GameObject centerSpotLightObject; 

    [Header("--- منطق اللعبة (متى تفتح البوابة النهائية) ---")]
    [Tooltip("رقم المرحلة المطلوبة لفتح البوابة بالكامل")]
    public int levelRequiredToOpen = 4;

    [Header("--- إعدادات الانتقال والتفاعل ---")]
    public string finalSceneName = "FinalEndingScene"; 
    public InputActionReference interactAction; 
    
    [Tooltip("لون الوميض اللي بيظهر وقت الانتقال للبوابة الرئيسية")]
    public Color fadeColor = Color.white;

    // 🔥 التعديل هنا: غيرناها من GameObject إلى InteractPromptController
    [Tooltip("اسحبي مجسم الأيقونة اللي عليه سكريبت InteractPromptController هنا")]
    public InteractPromptController interactPrompt; 
    
    public CanvasGroup whiteFade; 
    public float fadeSpeed = 1.5f;

    [Header("--- الأصوات ---")]
    public AudioSource ambientLoopSound; 
    public AudioSource teleportSound;    

    private bool isPlayerInRange = false;
    private bool isTransitioning = false;
    private bool isFullyOpen = false; 

    void Start()
    {
        // 🔥 التعديل هنا: استخدام دالة الإخفاء الإجباري
        if (interactPrompt != null) interactPrompt.ForceHide();
        if (whiteFade != null) whiteFade.alpha = 0f;
        
        CheckGateStatus();
    }

    private void OnEnable()
    {
        if (interactAction != null) interactAction.action.performed += OnInteractPressed;
        EventManager.StartListening("Level_Completed", OnLevelCompletedEvent);
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.performed -= OnInteractPressed;
        EventManager.StopListening("Level_Completed", OnLevelCompletedEvent);
    }

    private void OnLevelCompletedEvent(Dictionary<string, object> data)
    {
        Debug.Log("🏛️ البوابة العملاقة تلقت إشارة بانتهاء مرحلة! يتم التحقق من حالة الفتح...");
        CheckGateStatus(); 
    }

    public void CheckGateStatus()
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

        if (currentProgress >= levelRequiredToOpen)
        {
            if(portalGlowObject) portalGlowObject.SetActive(true);
            if(centerSpotLightObject) centerSpotLightObject.SetActive(true);

            isFullyOpen = true; 
            
            if (ambientLoopSound != null && !ambientLoopSound.isPlaying) ambientLoopSound.Play();
        }
        else
        {
            if(portalGlowObject) portalGlowObject.SetActive(false);
            if(centerSpotLightObject) centerSpotLightObject.SetActive(false);

            isFullyOpen = false; 
            
            if (ambientLoopSound != null) ambientLoopSound.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning && isFullyOpen)
        {
            isPlayerInRange = true;
            // 🔥 التعديل هنا: نأمر الأيقونة بالظهور
            if (interactPrompt != null) interactPrompt.ShowPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isPlayerInRange = false;
            // 🔥 التعديل هنا: نأمر الأيقونة بالاختفاء
            if (interactPrompt != null) interactPrompt.HidePrompt();
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && !isTransitioning && isFullyOpen)
        {
            StartCoroutine(TransitionRoutine());
        }
    }

    IEnumerator TransitionRoutine()
    {
        isTransitioning = true;
        
        // 🔥 التعديل هنا: إخفاء الأيقونة فوراً عند بدء الانتقال
        if (interactPrompt != null) interactPrompt.ForceHide();
        if (teleportSound != null) teleportSound.Play();

        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();        
        if (player != null) player.enabled = false;

        if (whiteFade != null)
        {
            Image fadeImage = whiteFade.GetComponent<Image>();
            if (fadeImage != null) fadeImage.color = fadeColor;

            whiteFade.alpha = 0f;
            while (whiteFade.alpha < 1f)
            {
                whiteFade.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
        
        EventManager.TriggerEvent("Telemetry_Final_Gate_Entered", null);
        SceneManager.LoadScene(finalSceneName);
    }
}