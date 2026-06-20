using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class IntroControlsDisplay : MonoBehaviour
{
    [Header("--- إعدادات شريط التعليمات (Zelda Style) ---")]
    [Tooltip("اسحبي مجسم الـ Hint_Bar بالكامل هنا (عشان السكربت يصحيه)")]
    public GameObject hintBarPanel; 

    [Tooltip("اسحبي الكانفاس جروب حق شريط التعليمات هنا")]
    public CanvasGroup hintsCanvasGroup;
    
    public float delayBeforeShow = 0.2f;
    public float displayDuration = 3.0f;
    public float fadeSpeed = 2.0f;

    [Header("--- 1️⃣ أيقونات تحريك الكاميرا (Look) ---")]
    public Image lookIconImage;
    public Sprite lookKeyboard; // صورة الفأرة ↔️
    public Sprite lookXbox;     // صورة الأنالوج الأيمن RS
    public Sprite lookPS;       // صورة الأنالوج الأيمن R

    [Header("--- 2️⃣ أيقونات المشي (Walk) ---")]
    public Image walkIconImage;
    public Sprite walkKeyboard; // صورة WASD
    public Sprite walkXbox;     // صورة الأنالوج الأيسر LS
    public Sprite walkPS;       // صورة الأنالوج الأيسر L

    [Header("--- 3️⃣ أيقونات الركض (Run) ---")]
    public Image runIconImage;
    public Sprite runKeyboard;  // صورة Shift
    public Sprite runXbox;
    public Sprite runPS;

    private PlayerInput playerInput;

    void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        
        if (hintsCanvasGroup != null) hintsCanvasGroup.alpha = 0f;
        UpdateIconsBasedOnDevice();
    }

    public void StartShowingHints()
    {
        if (hintBarPanel != null) hintBarPanel.SetActive(true); 

        if (hintsCanvasGroup != null)
        {
            StartCoroutine(SequenceRoutine());
        }
    }

    private IEnumerator SequenceRoutine()
    {
        yield return new WaitForSeconds(delayBeforeShow);

        while (hintsCanvasGroup.alpha < 1f)
        {
            hintsCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        hintsCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        while (hintsCanvasGroup.alpha > 0f)
        {
            hintsCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        hintsCanvasGroup.alpha = 0f;
        
        if (hintBarPanel != null) hintBarPanel.SetActive(false); 
    }

    private void UpdateIconsBasedOnDevice()
    {
        if (playerInput == null) return;
        string currentDevice = playerInput.currentControlScheme;
        
        if (currentDevice == "Keyboard&Mouse" || currentDevice == "Keyboard")
        {
            if (lookIconImage != null) lookIconImage.sprite = lookKeyboard;
            if (walkIconImage != null) walkIconImage.sprite = walkKeyboard;
            if (runIconImage != null) runIconImage.sprite = runKeyboard;
        }
        else if (currentDevice == "Gamepad")
        {
            Gamepad gamepad = Gamepad.current;
            bool isPS = (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad || (gamepad != null && gamepad.name.Contains("DualSense")));
            
            if (lookIconImage != null) lookIconImage.sprite = isPS ? lookPS : lookXbox;
            if (walkIconImage != null) walkIconImage.sprite = isPS ? walkPS : walkXbox;
            if (runIconImage != null) runIconImage.sprite = isPS ? runPS : runXbox;
        }
    }
}