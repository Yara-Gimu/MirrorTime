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
        // التحقق من ضغط زر C 
        // (إذا تستخدمين نظام الإدخال الجديد، تقدرين تربطينها به بدل GetKeyDown)
        if (Input.GetKeyDown(KeyCode.C))
        {
            // عكس الحالة: إذا كانت واقفة تصير تزحف، والعكس
            isCrouching = !isCrouching; 

            if (isCrouching)
            {
                // وضع الزحف: نرفع أولوية كاميرا الزحف عشان تظهر للمشاهد
                crouchCamera.Priority = 20;
                mainCamera.Priority = 10;
            }
            else
            {
                // وضع الوقوف: نرجع الأولوية للكاميرا الأساسية
                crouchCamera.Priority = 10;
                mainCamera.Priority = 20;
            }
        }
    }
}