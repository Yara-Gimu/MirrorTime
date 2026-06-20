using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupItem : MonoBehaviour
{
    [Header("تعريف الأداة")]
    [Tooltip("اسم فريد لا يتكرر لهذه الأداة، مثلا: Ancient_Key_01 أو Lantern")]
    public string itemID; 

    void Start()
    {
        // 1. أول ما يبدأ المشهد، الأداة تسأل مدير الحفظ: هل اسمي موجود في حقيبة نوار؟
        // (نتأكد أولاً أن مدير الحفظ جاهز لتجنب أخطاء بداية التشغيل)
        if (SaveManager.Instance != null && SaveManager.Instance.gameData.collectedTools.Contains(itemID))
        {
            // إذا كان اسمها موجوداً، يعني نوار التقطتها في جلسة لعب سابقة
            // لذلك ندمر المجسم فوراً قبل أن يراه اللاعب حتى لا يظهر مرتين
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 2. عندما تلمس نوار الأداة
        if (other.CompareTag("Player"))
        {
            // نضيف اسم الأداة لقائمة الأدوات المحفوظة
            SaveManager.Instance.gameData.collectedTools.Add(itemID);

            // نحفظ تقدم اللعبة فوراً في الخلفية مع مكان نوار الحالي
            SaveManager.Instance.SaveGame(other.transform.position, SceneManager.GetActiveScene().name);

            // يمكنك هنا إضافة سطر برمجي لإرسال الأداة لنظام الـ UI (المخزون) الخاص بك
            
            Debug.Log("✨ تم التقاط الأداة وحفظها في الحقيبة: " + itemID);

            // أخيراً، نخفي الأداة من البيئة
            Destroy(gameObject);
        }
    }
}