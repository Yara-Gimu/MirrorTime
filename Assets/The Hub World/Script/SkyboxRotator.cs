using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Header("--- إعدادات حركة السماء ---")]
    [Tooltip("سرعة دوران الغيوم (خليها رقم صغير جداً عشان تطلع طبيعية)")]
    public float rotationSpeed = 0.5f; 

    void Update()
    {
        // 1. نتأكد إن اللعبة فعلاً فيها Skybox شغال
        if (RenderSettings.skybox != null)
        {
            // 2. نقرأ درجة الدوران الحالية من الماتيريال
            float currentRotation = RenderSettings.skybox.GetFloat("_Rotation");
            
            // 3. نزيد الدوران مع مرور الوقت
            currentRotation += rotationSpeed * Time.deltaTime;

            // 4. خطوة تنظيف: عشان الرقم ما يكبر للمالانهاية ويثقل الذاكرة، نصفره إذا كمل لفة كاملة
            if (currentRotation >= 360f) 
            {
                currentRotation -= 360f;
            }

            // 5. نطبق الدوران الجديد على السماء
            RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
        }
    }
}