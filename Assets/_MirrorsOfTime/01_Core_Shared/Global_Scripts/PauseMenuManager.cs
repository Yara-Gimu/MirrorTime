using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; 
using UnityEngine.SceneManagement; 
using UnityEngine.Rendering; 
using UnityEngine.InputSystem; // 🌟 ضروري لقراءة نوع يد التحكم

public class PauseMenuManager : MonoBehaviour
{
    // 🌟 نظام Singleton للوصول السريع من أي سكربت ثاني بدون GetComponent
    public static PauseMenuManager Instance;

    [Header("شاشات الواجهة (UI)")]
    public GameObject pauseMenuCanvas; 
    public GameObject settingsCanvas;  

    [Header("تأثيرات بصرية (AAA)")]
    public Volume blurVolume; 

    [Header("--- إعدادات التلميح الذكي (UI Panel) ---")]
    [Tooltip("اسحبي الـ CanvasGroup اللي يجمع النص والصورة معاً")]
    public CanvasGroup hintCanvasGroup; 
    
    [Tooltip("اسحبي الـ Image اللي بنغير صورتها حسب جهاز اللاعب")]
    public Image hintIconImage; 

    [Header("أيقونات الأجهزة")]
    public Sprite pcIcon;
    public Sprite psIcon;
    public Sprite xboxIcon;

    [Header("إعدادات اللاعب")]
    [Tooltip("اسحبي اللاعب (Nawar) هنا لمعرفة نوع التحكم")]
    public PlayerInput playerInput;

    [Header("وقت التلميح")]
    public float hintDuration = 4f;    

    private bool isPaused = false;
    private bool hasHintPlayed = false; // عشان ما يشتغل التلميح إلا مرة وحدة بس

    void Awake()
    {
        // إعداد الـ Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. إغلاق القوائم وتصفية الشاشة
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        if (blurVolume != null) blurVolume.weight = 0f; 

        // 2. إخفاء التلميح تماماً في البداية (الشفافية صفر)
        if (hintCanvasGroup != null)
        {
            hintCanvasGroup.alpha = 0f;
            hintCanvasGroup.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 3. مراقبة ضغطة الإيقاف (ESC أو زر Start في اليد)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (settingsCanvas != null && settingsCanvas.activeSelf)
        {
            CloseSettings();
        }
        else
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // --- وظائف التحكم ---

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f; 
        if (blurVolume != null) blurVolume.weight = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        Time.timeScale = 1f; 
        if (blurVolume != null) blurVolume.weight = 0f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        pauseMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ==========================================
    // --- نظام التلميح السينمائي الذكي (AAA) ---
    // ==========================================

    // 🌟 هذي الدالة اللي بنناديها من سكربت حركة نوار
    public void TriggerPauseHint()
    {
        if (hintCanvasGroup != null && !hasHintPlayed)
        {
            hasHintPlayed = true; // عشان ما يتكرر كل ما تركض
            StartCoroutine(PlayHintSequence());
        }
    }

    private void UpdateHintIcon()
    {
        if (playerInput == null || hintIconImage == null) return;

        string currentDevice = playerInput.currentControlScheme;

        if (currentDevice == "Keyboard&Mouse" || currentDevice == "Keyboard")
        {
            hintIconImage.sprite = pcIcon;
        }
        else if (currentDevice == "Gamepad")
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad || gamepad.name.Contains("DualSense"))
                {
                    hintIconImage.sprite = psIcon;
                }
                else
                {
                    hintIconImage.sprite = xboxIcon;
                }
            }
        }
    }

    IEnumerator PlayHintSequence()
    {
        hintCanvasGroup.gameObject.SetActive(true);
        UpdateHintIcon(); // نحدث الصورة قبل الظهور

        // 1. فترة "التنفس" (ثانية ونص)
        yield return new WaitForSeconds(1.5f);

        // 2. ظهور ناعم (Fade In) للمجموعة كاملة
        float fadeTime = 1f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            hintCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            yield return null;
        }

        // 3. الانتظار ليقرأ اللاعب
        yield return new WaitForSeconds(hintDuration);

        // 4. اختفاء ناعم (Fade Out)
        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            hintCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }

        hintCanvasGroup.gameObject.SetActive(false);
    }
}