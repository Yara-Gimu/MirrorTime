using UnityEngine;
using UnityEngine.InputSystem;

public class RadialMenuController : MonoBehaviour
{
    [Tooltip("اسحبي مجسم الـ Canvas الخاص بالعجلة هنا")]
    public GameObject radialMenuCanvas;
    
    [Tooltip("اسحبي الأكشن (OpenRadialMenu) من مجلداتك هنا")]
    public InputActionReference openMenuAction;

    void Start()
    {
        radialMenuCanvas.SetActive(false);
    }

    void Update()
    {
        // إذا اللاعب ضغط الزر (بغض النظر عن نوع اليد أو الكيبورد)
        if (openMenuAction.action.WasPressedThisFrame())
        {
            radialMenuCanvas.SetActive(true);
            Time.timeScale = 0.2f; 
        }
        
        // إذا اللاعب فك الزر
        if (openMenuAction.action.WasReleasedThisFrame())
        {
            radialMenuCanvas.SetActive(false);
            Time.timeScale = 1f; 
        }
    }
}