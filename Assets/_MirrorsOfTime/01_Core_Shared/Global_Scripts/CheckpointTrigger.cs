using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("إعدادات نقطة الحفظ")]
    [Tooltip("يفعل هذا الخيار نفسه تلقائياً بعد الحفظ لكي لا تتكرر العملية")]
    public bool hasSaved = false;

    void OnTriggerEnter(Collider other)
    {
        // نتأكد أن المجسم الذي مر من هنا هو "نوار" (يجب أن يكون التاق الخاص بها Player)
        // ونتأكد أنه لم يتم الحفظ في هذه النقطة مسبقاً
        if (other.CompareTag("Player") && !hasSaved)
        {
            hasSaved = true;

            // نأخذ موقع نوار واسم المشهد ونرسله للمدير ليقوم بالحفظ
            SaveManager.Instance.SaveGame(other.transform.position, SceneManager.GetActiveScene().name);

            Debug.Log("✨ تم الوصول لنقطة الحفظ!");
            
            // 💡 ملاحظة للتطوير: يمكنك هنا إضافة كود تشغيل أيقونة حفظ صغيرة في زاوية الشاشة
        }
    }
}