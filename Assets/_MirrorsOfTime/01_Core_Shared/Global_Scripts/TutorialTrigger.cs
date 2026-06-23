using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    // Flag بسيط جداً لضمان ظهور التنبيه مرة واحدة فقط
    private bool hasTriggered = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (InGameNotificationManager.Instance != null)
            {
                // استدعاء الأنيميشن فقط!
                InGameNotificationManager.Instance.ShowNotification();
                hasTriggered = true; // تم التفعيل، لن يعمل مرة أخرى
            }
        }
    }
}