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
    // 🔥 التعديل هنا: غيرناها إلى السكريبت الجديد
    public InteractPromptController interactPrompt; 
    
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
        // 🔥 التعديل هنا
        if (interactPrompt != null) interactPrompt.ForceHide();
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
            // 🔥 التعديل هنا
            if (interactPrompt != null) interactPrompt.ShowPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isPlayerInRange = false;
            // 🔥 التعديل هنا
            if (interactPrompt != null) interactPrompt.HidePrompt();
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
        
        // 🔥 التعديل هنا
        if (interactPrompt != null) interactPrompt.ForceHide();
        if (teleportSound != null) teleportSound.Play();

        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
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

        Dictionary<string, object> gateData = new Dictionary<string, object>
        {
            { "DestinationScene", sceneToLoad },
            { "TimeEntered", Time.time }
        };
        EventManager.TriggerEvent("Telemetry_Gate_Entered", gateData);

        SceneManager.LoadScene(sceneToLoad);
    }
}