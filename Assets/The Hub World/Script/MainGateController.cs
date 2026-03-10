using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainGateController : MonoBehaviour
{
    [Header("--- المجسمات ---")]
    public Renderer mainGateRenderer;  
    public GameObject portalGlowObject; 
    public GameObject centerSpotLightObject; 

    [Header("--- الماتيريال ---")]
    public int rightMaterialIndex = 0; 
    public int leftMaterialIndex = 1;  
    public Material litMaterial;       
    public Material unlitMaterial;     

    [Header("--- منطق اللعبة (متى تفتح) ---")]
    public int levelToLightRight = 2;       
    public int levelToLightLeftAndOpen = 4; 

    [Header("--- إعدادات الانتقال والتفاعل (الجديدة) ---")]
    public string finalSceneName = "FinalEndingScene"; 
    public InputActionReference interactAction; 
    
    [Tooltip("لون الوميض اللي بيظهر وقت الانتقال للبوابة الرئيسية (يفضل أبيض ناصع)")]
    public Color fadeColor = Color.white;

    public GameObject interactPrompt; 
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
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (whiteFade != null) whiteFade.alpha = 0f;
        
        CheckGateStatus();
    }

    private void OnEnable()
    {
        if (interactAction != null) interactAction.action.performed += OnInteractPressed;
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.performed -= OnInteractPressed;
    }

    public void CheckGateStatus()
    {
        // 🏗️ الترقية المعمارية: القراءة من المدير المركزي
        int currentProgress = 0;
        if (SaveManager.Instance != null)
        {
            currentProgress = SaveManager.Instance.currentGateProgress;
        }
        else
        {
            // خطة طوارئ لو اختبرتي المشهد بدون مدير الحفظ
            currentProgress = PlayerPrefs.GetInt("GateProgress", 0);
        }

        Material[] mats = mainGateRenderer.materials;

        if (currentProgress >= levelToLightRight)
        {
            if (mats.Length > rightMaterialIndex) mats[rightMaterialIndex] = litMaterial;
        }
        else
        {
            if (mats.Length > rightMaterialIndex) mats[rightMaterialIndex] = unlitMaterial;
        }

        if (currentProgress >= levelToLightLeftAndOpen)
        {
            if (mats.Length > leftMaterialIndex) mats[leftMaterialIndex] = litMaterial;
            if(portalGlowObject) portalGlowObject.SetActive(true);
            if(centerSpotLightObject) centerSpotLightObject.SetActive(true);

            isFullyOpen = true; 
            
            if (ambientLoopSound != null && !ambientLoopSound.isPlaying) ambientLoopSound.Play();
        }
        else
        {
            if (mats.Length > leftMaterialIndex) mats[leftMaterialIndex] = unlitMaterial;
            if(portalGlowObject) portalGlowObject.SetActive(false);
            if(centerSpotLightObject) centerSpotLightObject.SetActive(false);

            isFullyOpen = false; 
            
            if (ambientLoopSound != null) ambientLoopSound.Stop();
        }

        mainGateRenderer.materials = mats;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning && isFullyOpen)
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
        if (isPlayerInRange && !isTransitioning && isFullyOpen)
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
            if (fadeImage != null) fadeImage.color = fadeColor;

            whiteFade.alpha = 0f;
            while (whiteFade.alpha < 1f)
            {
                whiteFade.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
        
        // 🏗️ الترقية المعمارية: إرسال إشارة لمدير البيانات لتسجيل إن اللاعب اتجه للنهاية
        // EventManager.Trigger("Telemetry_Final_Gate_Entered");

        SceneManager.LoadScene(finalSceneName);
    }
}