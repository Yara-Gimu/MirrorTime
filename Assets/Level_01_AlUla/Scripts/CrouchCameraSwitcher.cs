using UnityEngine;
using Unity.Cinemachine; // مهم جداً لمكتبة الكاميرات الجديدة في يونيتي 6

public class CrouchCameraSwitcher : MonoBehaviour
{
    [Header("إعدادات الكاميرات السينمائية")]
    [Tooltip("اسحبي كاميرا الوقوف الأساسية هنا")]
    public CinemachineCamera mainCamera; 
    
    [Tooltip("اسحبي كاميرا الزحف اللي نسخناها هنا")]
    public CinemachineCamera crouchCamera;

    // متغير لحفظ حالة نوار (هل هي تزحف أو واقفة؟)
    private bool isCrouching = false;

void Update()
{
    if (Input.GetKeyDown(KeyCode.C))
    {
        isCrouching = !isCrouching; 
        
        if (isCrouching)
        {
            crouchCamera.Priority = 50; // ارفعي الرقم لضمان الغلبة
            mainCamera.Priority = 10;
            Debug.Log("وضع الزحف: كاميرا الزحف مفعلة");
        }
        else
        {
            crouchCamera.Priority = 10;
            mainCamera.Priority = 50; // نرفع الأساسية عشان تسحب الكاميرا لها
            Debug.Log("وضع الوقوف: الكاميرا الأساسية مفعلة");
        }
    }
}
}