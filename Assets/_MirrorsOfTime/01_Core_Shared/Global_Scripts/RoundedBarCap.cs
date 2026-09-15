using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways] // يخلي السكربت يشتغل حتى وأنتِ تصممين بدون ما تشغلين اللعبة
public class RoundedBarCap : MonoBehaviour
{
    [Tooltip("اسحبي شريط الطاقة (PowerBar_Fill) هنا")]
    public Image barFill;
    
    [Tooltip("تعديل الزاوية إذا كان مكان البداية مختلف")]
    public float offsetAngle = 0f;

    void Update()
    {
        if (barFill == null) return;

        // حساب الزاوية بناءً على نسبة التعبئة (من 0 إلى 1)
        float fillAmount = barFill.fillAmount;
        
        // ضرب النسبة في 360 درجة، وبالسالب لأن التعبئة مع عقارب الساعة
        float angle = fillAmount * -360f; 

        // تطبيق الدوران على المحور (Pivot)
        transform.localRotation = Quaternion.Euler(0, 0, angle + offsetAngle);
    }
}