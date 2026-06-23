using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBlinkingDot : MonoBehaviour
{
    [Header("--- إعدادات الوميض ---")]
    [Tooltip("اسحبي صورة النقطة الحمراء هنا")]
    public Image dotImage;
    
    [Tooltip("سرعة الوميض (بالثواني)")]
    public float blinkSpeed = 0.6f;

    private Coroutine blinkCoroutine;

    private void OnEnable()
    {
        // يبدأ الوميض فوراً بمجرد ظهور شاشة التصوير
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private void OnDisable()
    {
        // يوقف الوميض لتوفير موارد الجهاز عند إغلاق الكاميرا
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        if (dotImage != null) dotImage.enabled = true; // إعادتها للحالة الأصلية
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            if (dotImage != null) 
            {
                // هذه الحركة تعكس الحالة: إذا كانت ظاهرة تختفي، وإذا كانت مختفية تظهر
                dotImage.enabled = !dotImage.enabled; 
            }
            // نستخدم Realtime لكي لا تتأثر بتجميد اللعبة
            yield return new WaitForSecondsRealtime(blinkSpeed);
        }
    }
}