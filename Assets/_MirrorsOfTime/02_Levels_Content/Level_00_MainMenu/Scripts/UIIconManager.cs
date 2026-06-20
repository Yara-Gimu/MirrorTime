using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIIconManager : MonoBehaviour
{
    [Header("--- صور الأيقونات في الشاشة ---")]
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

    private PlayerInput playerInput;

    void Start()
    {
        // 🌟 الإصلاح: مسحنا كود التركيز (Focus) من هنا لتجنب التضارب مع MainMenuManager
        
        // جلب أداة التحكم الحالية وتحديث الأيقونات فور تشغيل اللعبة
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            UpdateIcons(playerInput.currentControlScheme);
        }
    }

    public void OnControlsChanged(PlayerInput pi)
    {
        if (pi != null) UpdateIcons(pi.currentControlScheme);
    }

    private void UpdateIcons(string currentDevice)
    {
        if (selectIcon == null || backIcon == null) return;

        if (currentDevice == "Keyboard&Mouse" || currentDevice == "Keyboard")
        {
            selectIcon.sprite = kbSelect;
            backIcon.sprite = kbBack;
        }
        else if (currentDevice == "Gamepad")
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
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