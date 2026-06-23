using UnityEngine;
using System.Collections;

public class InGameNotificationManager : MonoBehaviour
{
    public static InGameNotificationManager Instance { get; private set; }

    [Header("--- واجهات الإشعار (UI) ---")]
    public CanvasGroup notificationPanel;
    public AudioSource chimeSound;

    [Header("--- إعدادات الأنيميشن ---")]
    public float fadeInSpeed = 5f;
    public float showDuration = 4f; 
    public float fadeOutSpeed = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
        Instance = this;
        
        // التأكد من إخفاء اللوحة وإطفائها تماماً عند بداية اللعبة
        if (notificationPanel != null) 
        {
            notificationPanel.alpha = 0f;
            notificationPanel.gameObject.SetActive(false);
        }
    }

    // 🌟 دالة الإظهار
    public void ShowNotification()
    {
        // تأكد من تشغيل المجسم قبل البدء
        if (notificationPanel != null) 
            notificationPanel.gameObject.SetActive(true); 
        
        StopAllCoroutines();
        StartCoroutine(NotificationRoutineSimple());
    }

    // 🌟 دالة الإخفاء الفوري المباشرة (التي تمنع التعليق عند إغلاق الكاميرا)
    public void HideNotificationImmediate()
    {
        StopAllCoroutines();
        if (notificationPanel != null) 
        {
            notificationPanel.alpha = 0f;
            // الضربة القاضية: إطفاء المجسم بالكامل
            notificationPanel.gameObject.SetActive(false); 
        }
    }

    private IEnumerator NotificationRoutineSimple()
    {
        if (chimeSound != null) chimeSound.Play();

        // ظهور اللوحة بسلاسة
        if (notificationPanel != null)
        {
            while (notificationPanel.alpha < 1f)
            {
                notificationPanel.alpha += Time.deltaTime * fadeInSpeed;
                yield return null;
            }
            notificationPanel.alpha = 1f;
        }

        yield return new WaitForSeconds(showDuration);

        // اختفاء اللوحة بسلاسة
        if (notificationPanel != null)
        {
            while (notificationPanel.alpha > 0f)
            {
                notificationPanel.alpha -= Time.deltaTime * fadeOutSpeed;
                yield return null;
            }
            notificationPanel.alpha = 0f;
            
            // إطفاء المجسم أيضاً بعد انتهاء العرض الطبيعي لضمان النظافة التامة
            notificationPanel.gameObject.SetActive(false); 
        }
    }
}