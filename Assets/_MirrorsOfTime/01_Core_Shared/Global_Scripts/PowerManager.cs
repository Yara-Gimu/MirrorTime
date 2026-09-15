using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("اسحبي صورة PowerBar_Fill هنا")]
    public Image powerBarFill;

    [Header("Power Settings")]
    public float maxPower = 100f;
    public float currentPower;
    public float regenRate = 15f; // سرعة تجدد الطاقة في الثانية

    [Header("AAA Math Magic")]
    // خدعة التطابق مع رسمة Figma:
    // الفراغ الشفاف للوصول لبداية القوس (30 درجة / 360 = 0.0833)
    private float emptyOffset = 0.0833f;
    // طول القوس الفعلي (120 درجة / 360 = 0.3333)
    private float fillLength = 0.3333f;

    void Start()
    {
        currentPower = maxPower; // نبدأ بطاقة كاملة
        UpdateUI();
    }

    void Update()
    {
        // تجدد الطاقة التلقائي بمرور الوقت
        if (currentPower < maxPower)
        {
            currentPower += regenRate * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, 0, maxPower);
            UpdateUI();
        }
    }

// دالة نستخدمها لما اللاعب يستخدم قدرة من العجلة
    public void UsePower(float cost)
    {
        if (currentPower >= cost)
        {
            currentPower -= cost;
            UpdateUI();
            Debug.Log("تم استخدام القدرة بنجاح! التكلفة: " + cost);
        }
        else
        {
            Debug.Log("طاقة غير كافية!");
        }
    }

    private void UpdateUI()
    {
        if (powerBarFill != null)
        {
            // نحول الطاقة إلى نسبة مئوية
            float powerPercentage = currentPower / maxPower;
            
            // نطبق المعادلة عشان التعبئة تمشي داخل القوس الزجاجي فقط
            powerBarFill.fillAmount = emptyOffset + (powerPercentage * fillLength);
        }
    }
}