using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.5f; // ارتفاع الطفو
    [SerializeField] private float floatFrequency = 1f;   // سرعة الطفو

    [Header("Rotation Settings")]
    [SerializeField] private bool enableRotation = true;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 30f, 0f); // سرعة الدوران لكل محور

    // متغير لحفظ الموقع الأساسي لتجنب تحرك العنصر عن مكانه الأصلي
    private Vector3 startPosition;

    void Start()
    {
        // حفظ الموقع الابتدائي عند بدء اللعبة
        startPosition = transform.position;
    }

    void Update()
    {
        AnimateFloating();
        
        if (enableRotation)
        {
            AnimateRotation();
        }
    }

    private void AnimateFloating()
    {
        // حساب الارتفاع الجديد باستخدام دالة الساين
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        
        // تطبيق الموقع الجديد مع الحفاظ على قيم X و Z الأصلية
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void AnimateRotation()
    {
        // تدوير العنصر بسلاسة
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}