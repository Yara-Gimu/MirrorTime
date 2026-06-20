using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.InputSystem; 

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;

    [Header("--- الواجهات (UI) ---")]
    public GameObject pauseMenuCanvas; 
    public GameObject settingsCanvas;  

    [Header("--- التأثيرات البصرية ---")]
    public Volume blurVolume; 

    [Header("--- نظام الإدخال الجديد (Command Pattern) ---")]
    [Tooltip("اسحبي حدث الإيقاف هنا (مثلاً زر Options في اليد أو ESC)")]
    public InputActionReference pauseAction;

    [Header("--- التلميح الذكي ---")]
    public CanvasGroup hintCanvasGroup; 
    public Image hintIconImage; 
    public Sprite pcIcon, psIcon, xboxIcon;
    public PlayerInput playerInput;
    public float hintDuration = 4f;     

    private bool isPaused = false;
    private bool hasHintPlayed = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        if (blurVolume != null) blurVolume.weight = 0f; 

        if (hintCanvasGroup != null)
        {
            hintCanvasGroup.alpha = 0f;
            hintCanvasGroup.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPausePerformed;
        }
    }

    void OnDisable()
    {
        if (pauseAction != null)
            pauseAction.action.performed -= OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
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
        if (FadeManager.instance != null)
            FadeManager.instance.LoadSceneSmoothly("MainMenu"); 
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ==========================================
    // --- نظام التلميح السينمائي الذكي (AAA) ---
    // ==========================================

    public void TriggerPauseHint()
    {
        if (hintCanvasGroup != null && !hasHintPlayed)
        {
            hasHintPlayed = true; 
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
        UpdateHintIcon(); 

        yield return new WaitForSeconds(1.5f);

        float fadeTime = 1f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            hintCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            yield return null;
        }

        yield return new WaitForSeconds(hintDuration);

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