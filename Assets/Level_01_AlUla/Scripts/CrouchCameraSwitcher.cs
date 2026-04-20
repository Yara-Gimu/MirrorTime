using UnityEngine;
using UnityEngine.InputSystem; // 🌟 أضفنا مكتبة الإدخال الجديدة
using Unity.Cinemachine; 

public class CrouchCameraSwitcher : MonoBehaviour
{
    [Header("إعدادات الكاميرات السينمائية")]
    public CinemachineCamera mainCamera; 
    public CinemachineCamera crouchCamera;

    [Header("نظام الإدخال الجديد")]
    [Tooltip("اسحبي الـ InputAction الخاص بالزحف (مثلاً زر C أو الدائرة في البلايستيشن) هنا")]
    public InputActionReference crouchAction;

    private bool isCrouching = false;

    void OnEnable()
    {
        if (crouchAction != null)
        {
            crouchAction.action.Enable();
            crouchAction.action.performed += ToggleCrouchCamera;
        }
    }

    void OnDisable()
    {
        if (crouchAction != null)
        {
            crouchAction.action.performed -= ToggleCrouchCamera;
        }
    }

    private void ToggleCrouchCamera(InputAction.CallbackContext context)
    {
        isCrouching = !isCrouching; 
        
        if (isCrouching)
        {
            crouchCamera.Priority = 50; 
            mainCamera.Priority = 10;
            Debug.Log("وضع الزحف: كاميرا الزحف مفعلة");
        }
        else
        {
            crouchCamera.Priority = 10;
            mainCamera.Priority = 50; 
            Debug.Log("وضع الوقوف: الكاميرا الأساسية مفعلة");
        }
    }
}