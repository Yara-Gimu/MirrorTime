using UnityEngine;
using System.Collections;

public class FloatingObject : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.5f; // ارتفاع الطفو
    [SerializeField] private float floatFrequency = 1f;   // سرعة الطفو

    [Header("Rotation Settings")]
    [SerializeField] private bool enableRotation = true;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 30f, 0f); 

    // 🌟 غيرنا اسمها إلى basePosition عشان نقدر نحركها وتلحقها حركة الطفو
    private Vector3 basePosition;

    void Start()
    {
        // حفظ الموقع الابتدائي
        basePosition = transform.position;
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
        // نحسب ارتفاع الطفو لحاله
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        
        // 🌟 نطبق الطفو فوق الـ basePosition (سواء كانت ثابتة أو تتحرك!)
        transform.position = basePosition + new Vector3(0f, floatOffset, 0f);
    }

    private void AnimateRotation()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    // ==========================================
    // 🌟 السحر الإخراجي: دالة تحريك المراية لقدام
    // ==========================================
    public void MoveMirrorForward(float distance, float duration)
    {
        StartCoroutine(SmoothMove(distance, duration));
    }

    private IEnumerator SmoothMove(float distance, float duration)
    {
        Vector3 startPos = basePosition;
        // نحسب النقطة الجديدة لقدام (عكس اتجاه واجهة المراية أو معها حسب تصميمك)
        // إذا المراية لفتها معكوسة، غيري transform.forward إلى -transform.forward
        Vector3 targetPos = basePosition + (transform.forward * distance); 
        
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // 🌟 نحرك النقطة الأساسية بنعومة، والـ Update بيتكفل بإضافة الطفو عليها!
            basePosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            yield return null;
        }
        
        basePosition = targetPos; // تأكيد الوصول
    }
}