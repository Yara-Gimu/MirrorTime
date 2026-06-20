using UnityEngine;
using Unity.Cinemachine; // إذا ظهر خطأ هنا، غيريها إلى: using Cinemachine;

public class ApplyCameraSensitivity : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;
    
    [Header("السرعات الأساسية للكاميرا")]
    [Tooltip("اكتبي هنا السرعات الأصلية التي تعجبك والموجودة في إعدادات الكاميرا حالياً")]
    public float baseSpeedX = 300f; // سرعة الالتفاف يمين ويسار
    public float baseSpeedY = 2f;   // سرعة الالتفاف فوق وتحت

    void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();

        // جلب رقم الحساسية الذي اختاره اللاعب من القائمة الرئيسية
        float sensitivityMultiplier = PlayerPrefs.GetFloat("CameraSensitivity", 1f);

        if (freeLookCamera != null)
        {
            // نضرب السرعة الأساسية في المضاعف
            // فلو اختار اللاعب رقم 2، ستصبح السرعة الضعف!
            freeLookCamera.m_XAxis.m_MaxSpeed = baseSpeedX * sensitivityMultiplier;
            freeLookCamera.m_YAxis.m_MaxSpeed = baseSpeedY * sensitivityMultiplier;
        }
    }
}