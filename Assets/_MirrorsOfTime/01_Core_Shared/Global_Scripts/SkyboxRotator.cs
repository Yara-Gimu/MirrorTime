using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Header("--- إعدادات حركة السماء ---")]
    [Tooltip("سرعة دوران الغيوم (خليها رقم صغير جداً عشان تطلع طبيعية)")]
    public float rotationSpeed = 0.5f; 
    
    // 🌟 الترقية المعمارية: تعريف المتغيرات مرة واحدة للسرعة القصوى
    private Material skyboxMat;
    private static readonly int RotationID = Shader.PropertyToID("_Rotation");
    private float currentRotation = 0f;

    void Start()
    {
        skyboxMat = RenderSettings.skybox;
        if (skyboxMat != null)
        {
            currentRotation = skyboxMat.GetFloat(RotationID);
        }
    }

    void Update()
    {
        if (skyboxMat != null)
        {
            currentRotation += rotationSpeed * Time.deltaTime;

            if (currentRotation >= 360f) 
            {
                currentRotation -= 360f;
            }

            // 🌟 استخدام الـ ID بدلاً من الكلمة النصية يضاعف سرعة المعالجة
            skyboxMat.SetFloat(RotationID, currentRotation);
        }
    }
}