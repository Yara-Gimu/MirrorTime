using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class WaitAngleCamera : MonoBehaviour
{
    [Header("--- كاميرا الزاوية ---")]
    [Tooltip("اسحبي كاميرا VCam_WaitAngle هنا")]
    public CinemachineCamera waitCamera;

    [Header("--- زر الحركة ---")]
    [Tooltip("اسحبي أكشن الحركة (Player/Move) هنا")]
    public InputActionReference moveAction;

    void Start()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    void Update()
    {
        if (moveAction != null)
        {
            // نقرأ قيمة عصا التحكم أو أزرار الكيبورد
            Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
            
            // إذا اللاعب لمس العصا بقوة بسيطة (أكبر من 0.1)
            if (moveInput.magnitude > 0.1f)
            {
                // نرجع أولوية كاميرا الزاوية للصفر! 
                // (السينماشين راح يسوي دمج ناعم ويرجع لكاميرا ظهر اللاعب الأساسية تلقائياً)
                if (waitCamera != null)
                {
                    waitCamera.Priority = 0;
                }
                
                // نقفل السكربت عشان ما عاد يشتغل ويزعجنا
                this.enabled = false;
            }
        }
    }
}