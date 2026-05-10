using UnityEngine;
using UnityEngine.Events; // مهم جداً لإضافة الأحداث

[RequireComponent(typeof(Collider))]
public class UniversalTrigger : MonoBehaviour
{
    [Header("--- إعدادات التريقر ---")]
    [Tooltip("من هو المجسم المسموح له بتفعيل هذا الحدث؟")]
    public string targetTag = "Player"; 
    
    [Tooltip("هل تريدين تفعيل الحدث مرة واحدة فقط؟")]
    public bool triggerOnlyOnce = true; 
    
    [Header("--- الأحداث (ماذا سيحصل؟) ---")]
    // هنا يكمن السحر! قائمة فارغة تعبئينها من الـ Inspector
    public UnityEvent onTriggerEnterAction; 

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 1. التحقق من التاغ
        if (other.CompareTag(targetTag))
        {
            // 2. التحقق إذا كان مسموح يشتغل مرة واحدة واشتغل مسبقاً
            if (triggerOnlyOnce && hasTriggered) return;

            // 3. تنفيذ كل الأوامر الموجودة في الـ Inspector فوراً
            onTriggerEnterAction?.Invoke();
            
            hasTriggered = true;
        }
    }

    // دالة إضافية مفيدة لو أردتِ إعادة تفعيل التريقر لاحقاً (مثلاً عند إعادة المرحلة)
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}