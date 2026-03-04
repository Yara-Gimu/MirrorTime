using UnityEngine;

public class GateManager : MonoBehaviour
{
    [Header("--- Bawabats (Side Gates) ---")]
    // اسحبي هنا كائنات البوابات الجانبية بالترتيب
    public GateController[] sideGates; 

    [Header("--- Main Gate Visuals ---")]
    public Renderer mainGateRenderer;    // اسحبي هنا مجسم البوابة الكبير
    public GameObject mainGatePortalObj; // الاوبجكت المتوهج (الممر) اللي يدخله اللاعب

    [Header("--- Material Settings ---")]
    public Material litMaterial;   // الماتيريال المضيء
    public Material unlitMaterial; // الماتيريال الطافي
    
    // حددي أرقام الخانات (Element Index) من الإنسبكتور
    public int rightMaterialIndex = 0; // رقم خانة النقش اليمين
    public int leftMaterialIndex = 1;  // رقم خانة النقش اليسار

    void Start()
    {
        UpdateGatesState();
    }

    void UpdateGatesState()
    {
        // 1. نقرأ المستوى الحالي
        int progress = PlayerPrefs.GetInt("GateProgress", 0);

        // 2. تحديث البوابات الجانبية (العلا، ثاج، إلخ)
        for (int i = 0; i < sideGates.Length; i++)
        {
            // البوابة تفتح إذا كان اللاعب خلص المرحلة السابقة لها
            // (أو نعتمد على اللوجيك اللي داخل البوابة نفسها، هذا مجرد تأكيد)
            if (sideGates[i] != null)
            {
                sideGates[i].CheckPermission(); 
            }
        }

        // 3. تحديث البوابة الرئيسية (تغيير الماتيريال)
        UpdateMainGateMaterials(progress);
    }

    void UpdateMainGateMaterials(int progress)
    {
        // ننسخ قائمة الماتيريال الحالية عشان نعدل عليها
        Material[] mats = mainGateRenderer.materials;

        // --- التحكم في النقش اليمين (يضيء بعد مرحلة 2) ---
        if (progress >= 2) 
        {
            // تغيير لـ مضيء
            if (mats.Length > rightMaterialIndex) mats[rightMaterialIndex] = litMaterial;
        }
        else 
        {
            // تغيير لـ طافي
            if (mats.Length > rightMaterialIndex) mats[rightMaterialIndex] = unlitMaterial;
        }

        // --- التحكم في النقش اليسار + البوابة (يضيء بعد مرحلة 4) ---
        if (progress >= 4)
        {
            // تغيير لـ مضيء
            if (mats.Length > leftMaterialIndex) mats[leftMaterialIndex] = litMaterial;
            
            // فتح بوابة النهاية
            if (mainGatePortalObj) mainGatePortalObj.SetActive(true);
        }
        else
        {
            // تغيير لـ طافي
            if (mats.Length > leftMaterialIndex) mats[leftMaterialIndex] = unlitMaterial;
            
            // إغلاق بوابة النهاية
            if (mainGatePortalObj) mainGatePortalObj.SetActive(false);
        }

        // مهم جداً: تطبيق التغييرات على المجسم
        mainGateRenderer.materials = mats;
    }

    // هذه الدالة للحفظ (يمكنك استخدامها أو الاعتماد على WinTrigger)
    public void LevelCompleted(int levelIndex)
    {
        int currentProgress = PlayerPrefs.GetInt("GateProgress", 0);
        
        if (levelIndex > currentProgress)
        {
            PlayerPrefs.SetInt("GateProgress", levelIndex);
            PlayerPrefs.Save();
            
            // تحديث حالة البوابات مباشرة بعد الحفظ عشان اللاعب يشوف التغيير فوراً
            UpdateGatesState();
        }
    }
}