using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.InputSystem; 

public class RuneUIManager : MonoBehaviour
{
    public static RuneUIManager Instance;

    [Header("عناصر الواجهة")]
    public GameObject readerPanel;
    public TextMeshProUGUI runeTextDisplay;
    public Image runeDisplayImage; 
    public Button closeButton; 

    [Header("أيقونات الإغلاق (حسب الجهاز)")]
    public Image closeIconImage; // 🌟 اسحبي الصورة التي ستتغير هنا
    public Sprite kbCloseIcon;   // 🌟 صورة زر ESC أو Backspace
    public Sprite xboxCloseIcon; // 🌟 صورة زر B
    public Sprite psCloseIcon;   // 🌟 صورة زر الدائرة

    [Header("إعدادات الترجمة")]
    public string tableName = "SubtitlesTable"; 

    [Header("إعدادات الكنترولر")]
    public InputActionReference cancelAction; 

    private PlayerInput playerInput;

    private void Awake()
    {
        Instance = this;
        if (readerPanel != null) readerPanel.SetActive(false);
        if (runeTextDisplay != null) runeTextDisplay.isRightToLeftText = true;

        if (closeButton != null)
            closeButton.onClick.AddListener(HideRune);

        playerInput = FindFirstObjectByType<PlayerInput>();
    }

    private void OnEnable()
    {
        if (cancelAction != null)
        {
            cancelAction.action.Enable();
            cancelAction.action.performed += OnCancelPressed;
        }
    }

    private void OnDisable()
    {
        if (cancelAction != null) cancelAction.action.performed -= OnCancelPressed;
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        if (readerPanel != null && readerPanel.activeSelf) HideRune();
    }

    public void ShowRune(string key, Sprite runeSprite) 
    {
        if (readerPanel != null) readerPanel.SetActive(true);

        UpdateCloseIcon(); // 🌟 تحديث صورة الزر فور فتح اللوحة

        Time.timeScale = 0f; 

        if (runeTextDisplay != null)
        {
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, key).Completed += (h) => {
                runeTextDisplay.text = h.IsDone ? h.Result : "Error";
            };
        }

        if (runeDisplayImage != null)
        {
            if (runeSprite != null)
            {
                runeDisplayImage.sprite = runeSprite;
                runeDisplayImage.gameObject.SetActive(true);
            }
            else runeDisplayImage.gameObject.SetActive(false);
        }
    }

    public void HideRune()
    {
        if (readerPanel != null) readerPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    // 🌟 الدالة الجديدة لمعرفة الجهاز وتغيير أيقونة زر الخروج
    private void UpdateCloseIcon()
    {
        if (closeIconImage == null) return;

        if (playerInput != null && playerInput.currentControlScheme == "Gamepad")
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad || gamepad.name.Contains("DualSense"))
                    closeIconImage.sprite = psCloseIcon;
                else
                    closeIconImage.sprite = xboxCloseIcon;
            }
        }
        else
        {
            closeIconImage.sprite = kbCloseIcon;
        }
    }
}