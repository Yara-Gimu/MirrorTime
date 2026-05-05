using UnityEngine;
using Unity.Cinemachine;

public class MirrorCameraController : MonoBehaviour
{
    [Header("--- إعدادات كاميرا الاقتراب ---")]
    [Tooltip("كاميرا السينماشين اللي تقرب لما نوار تدخل منطقة المرآة")]
    public CinemachineCamera interactionCamera;

    [Tooltip("رقم الأولوية لما نوار تكون عند المرآة (مثلاً 100)")]
    public int activePriority = 100;

    [Tooltip("رقم الأولوية لما نوار تبعد (مثلاً 5)")]
    public int idlePriority = 5;

    // 🌟 أول ما تدخل نوار المنطقة، الكاميرا تقرب وتستعد
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionCamera != null)
            {
                interactionCamera.Priority = activePriority;
                Debug.Log("🎥 نوار دخلت المنطقة: كاميرا التفاعل اشتغلت!");
            }
        }
    }

    // 🌟 لو نوار مشت بعيد بدون ما تضغط E، ترجع الكاميرا طبيعية
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionCamera != null)
            {
                interactionCamera.Priority = idlePriority;
                Debug.Log("🚶 نوار طلعت من المنطقة: رجعنا للكاميرا الأساسية.");
            }
        }
    }
}