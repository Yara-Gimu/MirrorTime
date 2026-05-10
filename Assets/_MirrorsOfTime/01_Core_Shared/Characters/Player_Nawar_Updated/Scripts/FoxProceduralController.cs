using UnityEngine;

public class FoxProceduralController : MonoBehaviour
{
    [Header("عظام الذيل")]
    public Transform tailBase;
    public Transform tailTip;

    [Header("عظام الأذنين")]
    public Transform earLeft;
    public Transform earRight;

    [Header("إعدادات الحركة")]
    [Range(0, 20)] public float wagSpeed = 5f;   // سرعة الهز
    [Range(0, 50)] public float wagAmount = 20f;  // قوة/زاوية الهز
    [Range(0, 1)] public float tipDelay = 0.2f;   // تأخير بسيط لطرف الذيل ليعطي مرونة

    // حفظ الدوران الأصلي للعظام
    private Quaternion initialTailBase;
    private Quaternion initialTailTip;
    private Quaternion initialEarL;
    private Quaternion initialEarR;

    void Start()
    {
        // حفظ الوضعية الأساسية للعظام عند بدء اللعبة
        if (tailBase) initialTailBase = tailBase.localRotation;
        if (tailTip) initialTailTip = tailTip.localRotation;
        if (earLeft) initialEarL = earLeft.localRotation;
        if (earRight) initialEarR = earRight.localRotation;
    }

    // نستخدم LateUpdate لأننا نريد تعديل العظام بعد أن ينهي الـ Animator عمله
    void LateUpdate()
    {
        float time = Time.time * wagSpeed;

        // 1. تحريك قاعدة الذيل (حركة Sin ناعمة)
        if (tailBase != null)
        {
            float angle = Mathf.Sin(time) * wagAmount;
            tailBase.localRotation = initialTailBase * Quaternion.Euler(0, angle, 0);
        }

        // 2. تحريك طرف الذيل مع تأخير بسيط (يعطي إحساس الوزن والمرونة)
        if (tailTip != null)
        {
            float angle = Mathf.Sin(time - tipDelay) * (wagAmount * 1.2f);
            tailTip.localRotation = initialTailTip * Quaternion.Euler(0, angle, 0);
        }

        // 3. تحريك الأذنين (حركة أخف وأسرع قليلاً لتبدو كأنها تتفاعل مع الهواء)
        if (earLeft != null && earRight != null)
        {
            float earAngle = Mathf.Sin(time * 1.2f) * (wagAmount * 0.3f);
            earLeft.localRotation = initialEarL * Quaternion.Euler(earAngle, 0, 0);
            earRight.localRotation = initialEarR * Quaternion.Euler(earAngle, 0, 0);
        }
    }
}