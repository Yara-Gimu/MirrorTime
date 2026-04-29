using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MysticBoundaryController : MonoBehaviour
{
    [Header("--- إعدادات المسافات ---")]
    [Tooltip("مركز الخريطة (المنصة)")]
    public Transform hubCenter; 
    
    [Tooltip("المسافة التي يبدأ عندها الضباب والصوت بالظهور (بداية التحذير)")]
    public float warningDistance = 100f; 

    [Tooltip("المسافة النهائية التي عندها تضيع نوار تماماً وتنتقل")]
    public float maxDistance = 150f; 
    
    [Tooltip("المكان الذي تعود إليه نوار")]
    public Transform safeReturnPoint; 

    [Header("--- إعدادات التأثيرات ---")]
    public CanvasGroup fogFadeCanvas; 
    public AudioSource mysticWindSound; 
    [Range(0, 1)] public float maxWindVolume = 0.6f;

    private GameObject player;
    private bool isTeleporting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (fogFadeCanvas != null) fogFadeCanvas.alpha = 0f;
        if (mysticWindSound != null) {
            mysticWindSound.volume = 0;
            mysticWindSound.loop = true;
            mysticWindSound.Play();
        }
    }

    void Update()
    {
        if (player == null || isTeleporting || hubCenter == null) return;

        float distance = Vector3.Distance(player.transform.position, hubCenter.position);

        // 1. منطقة الأمان (داخل حدود التحذير)
        if (distance < warningDistance)
        {
            fogFadeCanvas.alpha = Mathf.Lerp(fogFadeCanvas.alpha, 0, Time.deltaTime);
            mysticWindSound.volume = Mathf.Lerp(mysticWindSound.volume, 0, Time.deltaTime);
        }
        // 2. منطقة التحذير (الضباب والصوت يزيدان تدريجياً)
        else if (distance >= warningDistance && distance < maxDistance)
        {
            // حساب نسبة القرب من النهاية (0 إلى 1)
            float proximity = (distance - warningDistance) / (maxDistance - warningDistance);
            
            // نرفع الشفافية والصوت بناءً على النسبة
            fogFadeCanvas.alpha = proximity * 0.8f; // يوصل لـ 80% ضباب قبل الانتقال المفاجئ
            mysticWindSound.volume = proximity * maxWindVolume;
        }
        // 3. تجاوز الحدود النهائية (الانتقال الكامل)
        else if (distance >= maxDistance)
        {
            StartCoroutine(LostInFogRoutine());
        }
    }

    IEnumerator LostInFogRoutine()
    {
        isTeleporting = true;

        // تعطيل الحركة
        var playerMovement = player.GetComponent<PlayerStateMachine>();
        if (playerMovement != null) playerMovement.enabled = false;

        // تعتيم كامل وسريع للشاشة
        while (fogFadeCanvas.alpha < 1f)
        {
            fogFadeCanvas.alpha += Time.deltaTime * 3f;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // النقل وتوجيه الوجه للمركز
        player.transform.position = safeReturnPoint.position;
        Vector3 lookAtPos = new Vector3(hubCenter.position.x, player.transform.position.y, hubCenter.position.z);
        player.transform.LookAt(lookAtPos);

        yield return new WaitForSeconds(0.5f);

        // إعادة كل شيء للطبيعة تدريجياً
        while (fogFadeCanvas.alpha > 0f)
        {
            fogFadeCanvas.alpha -= Time.deltaTime * 1.5f;
            mysticWindSound.volume -= Time.deltaTime * 0.5f;
            yield return null;
        }

        if (playerMovement != null) playerMovement.enabled = true;
        isTeleporting = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (hubCenter != null)
        {
            // دائرة خضراء: بداية التحذير
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hubCenter.position, warningDistance);
            // دائرة حمراء: نقطة الانتقال
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hubCenter.position, maxDistance);
        }
    }
}