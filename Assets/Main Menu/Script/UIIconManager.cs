using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; // ضروري للتحكم بالأزرار

public class UIIconManager : MonoBehaviour
{
    [Header("--- Main Menu Buttons (إعدادات أزرار القائمة) ---")]
    public GameObject continueButton; // زر إكمال الرحلة
    public GameObject newGameButton;  // زر لعبة جديدة

    [Header("--- UI Images (صور الأزرار في الشاشة) ---")]
    public Image selectIcon; 
    public Image backIcon;   

    [Header("--- Keyboard Icons ---")]
    public Sprite kbSelect; 
    public Sprite kbBack;   

    [Header("--- Xbox Icons ---")]
    public Sprite xboxSelect; 
    public Sprite xboxBack;   

    [Header("--- PlayStation Icons ---")]
    public Sprite psSelect; 
    public Sprite psBack;   

    void Start()
    {
        // 1. فحص ملف التخزين (هذا مجرد مثال، 1 = فيه تخزين، 0 = لاعب جديد)
        bool hasSaveFile = PlayerPrefs.GetInt("HasSaveFile", 0) == 1; 

        // 2. تصفير التحديد الحالي
        EventSystem.current.SetSelectedGameObject(null);

        // 3. تحديد الزر الأول بناءً على حالة اللاعب
        if (hasSaveFile)
        {
            continueButton.SetActive(true); // إظهار زر الإكمال
            EventSystem.current.SetSelectedGameObject(continueButton); // تحديده كأول زر
        }
        else
        {
            continueButton.SetActive(false); // إخفاء زر الإكمال لأنه لاعب جديد
            EventSystem.current.SetSelectedGameObject(newGameButton); // تحديد "لعبة جديدة" كأول زر
        }
    }

    // دالة تغيير أيقونات التحكم تلقائياً
    public void OnControlsChanged(PlayerInput playerInput)
    {
        string currentDevice = playerInput.currentControlScheme;

        if (currentDevice == "Keyboard&Mouse")
        {
            selectIcon.sprite = kbSelect;
            backIcon.sprite = kbBack;
        }
        else if (currentDevice == "Gamepad")
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                // طريقة آمنة للتعرف على أيدي بلايستيشن 
                if (gamepad is UnityEngine.InputSystem.DualShock.DualShockGamepad || gamepad.name.Contains("DualSense"))
                {
                    selectIcon.sprite = psSelect;
                    backIcon.sprite = psBack;
                }
                else
                {
                    selectIcon.sprite = xboxSelect;
                    backIcon.sprite = xboxBack;
                }
            }
        }
    }
}