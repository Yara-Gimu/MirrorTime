using UnityEngine;
using UnityEngine.SceneManagement; 

public class WinTrigger : MonoBehaviour
{
    [Header("Settings")]
    public int levelNumber = 1; 
    public string hubSceneName = "MainHub"; // تأكدي أن اسم المشهد هنا يطابق اسم المشهد في ملفاتك

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            FinishLevel();
        }
    }

    void FinishLevel()
    {
        Debug.Log("🎉 مبروك! خلصت المرحلة");

        // 1. حفظ التقدم
        int currentProgress = PlayerPrefs.GetInt("GateProgress", 0);

        if (levelNumber > currentProgress)
        {
            PlayerPrefs.SetInt("GateProgress", levelNumber);
            PlayerPrefs.Save(); 
            Debug.Log("تم حفظ التقدم: " + levelNumber);
        }

        // 2. الانتقال للمشهد
        // التصحيح هنا: استخدمنا المتغير بدلاً من كتابة الاسم مباشرة
        SceneManager.LoadScene(hubSceneName); 
    }
}