using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ComingSoonGate : MonoBehaviour
{
    [Header("--- إعدادات واجهة قريباً ---")]
    public GameObject comingSoonCanvas;
    public Button returnButton; 

    [Header("--- إعدادات الإدخال (Cross-Platform) ---")]
    public InputActionReference interactAction; // زر E أو مربع/X
    public InputActionReference cancelAction;   // زر ESC أو دائرة/B
    public InteractPromptController interactPrompt; 

    private bool isPlayerInRange = false;

    void Start()
    {
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(CloseScreen);
        }
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractPressed;
        }
        
        if (cancelAction != null)
        {
            cancelAction.action.Enable();
            cancelAction.action.performed += OnCancelPressed;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.performed -= OnInteractPressed;
        if (cancelAction != null) cancelAction.action.performed -= OnCancelPressed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactPrompt != null && (comingSoonCanvas == null || !comingSoonCanvas.activeSelf)) 
                interactPrompt.ShowPrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactPrompt != null) interactPrompt.HidePrompt();
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (isPlayerInRange && comingSoonCanvas != null && !comingSoonCanvas.activeSelf)
        {
            ShowComingSoonScreen();
        }
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        if (comingSoonCanvas != null && comingSoonCanvas.activeSelf)
        {
            CloseScreen();
        }
    }

    private void ShowComingSoonScreen()
    {
        comingSoonCanvas.SetActive(true);
        if (interactPrompt != null) interactPrompt.ForceHide();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; 
    }

    public void CloseScreen()
    {
        if (comingSoonCanvas != null) comingSoonCanvas.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f; 
        
        if (isPlayerInRange && interactPrompt != null) interactPrompt.ShowPrompt();
    }
}