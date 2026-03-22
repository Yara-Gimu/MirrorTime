using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UIIconManager : MonoBehaviour
{
    [Header("--- Main Menu Buttons (إعدادات أزرار القائمة) ---")]
    public GameObject continueButton; 
    public GameObject newGameButton;  

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
       
        bool hasSaveFile = SaveManager.Instance.HasSaveData();
        EventSystem.current.SetSelectedGameObject(null);

        if (hasSaveFile)
        {
            continueButton.SetActive(true); 
            EventSystem.current.SetSelectedGameObject(continueButton); 
        }
        else
        {
            continueButton.SetActive(false); 
            EventSystem.current.SetSelectedGameObject(newGameButton); 
        }
    }

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