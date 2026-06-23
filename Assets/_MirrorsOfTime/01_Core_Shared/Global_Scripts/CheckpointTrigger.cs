using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("إعدادات نقطة الحفظ")]
    public string checkpointID = "Flowers_Save_1"; 
    public bool hasSavedLocal = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // إذا حفظنا من قبل، لا تفعل شيئاً
        if (hasSavedLocal || PlayerPrefs.GetInt(checkpointID, 0) == 1) return;

        // 1. استدعاء مدير الحفظ الفعلي لحفظ بيانات نوار
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame(other.transform.position, SceneManager.GetActiveScene().name);
            Debug.Log($"💾 [Checkpoint] {checkpointID} تم الحفظ بنجاح!");
        }

        // 2. استدعاء لوحة الحفظ الزاويّة الهادئة
        if (SaveNotificationManager.Instance != null)
        {
            SaveNotificationManager.Instance.ShowSaveNotification();
        }

        // 3. إقفال هذه النقطة للأبد
        hasSavedLocal = true;
        PlayerPrefs.SetInt(checkpointID, 1);
        PlayerPrefs.Save();
    }
}