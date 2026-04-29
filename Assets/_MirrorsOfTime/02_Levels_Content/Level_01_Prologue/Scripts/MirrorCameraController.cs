using UnityEngine;
using Unity.Cinemachine;

public class MirrorCameraController : MonoBehaviour
{
    [Header("--- إعدادات كاميرا المراية ---")]
    [Tooltip("اسحبي كاميرا السينماشين الخاصة بالمراية هنا")]
    public CinemachineCamera interactionCamera;

    [Tooltip("الرقم العالي اللي يخلي الكاميرا تفوز (مثلاً 100 عشان تغطي على الزحف)")]
    public int activePriority = 100;

    [Tooltip("الرقم الضعيف وقت ما تكون نوار بعيدة")]
    public int idlePriority = 5;

    // 🌟 1. أول ما تدخل نوار المنطقة (التريجر)، الكاميرا تشتغل فوراً!
    private void OnTriggerEnter(Collider other)
    {
        // نتأكد إن اللي دخل هو اللاعب مو أي شيء ثاني
        if (other.CompareTag("Player"))
        {
            if (interactionCamera != null)
            {
                interactionCamera.Priority = activePriority;
                Debug.Log("🎥 نوار دخلت المنطقة: الكاميرا استعدت للقطة!");
            }
        }
    }

    // 🌟 2. لو نوار غيرت رأيها ومشت بعيد بدون ما تضغط E، ترجع الكاميرا طبيعية
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