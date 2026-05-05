using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // 👑 النمط المنفرد (Singleton) - يمكن لأي كود في اللعبة التحدث معه مباشرة
    public static SaveManager Instance { get; private set; }

    [Header("--- بيانات اللاعب الحالية ---")]
    public int currentGateProgress = 0; // 0=العلا، 1=ثاج، 2=الفاو، 3=تاروت
    public bool hasPlayedBefore = false;
    
    // 🎬 المتغير الجديد لتتبع المشهد السينمائي في غرفة البوابات
    public bool hasSeenHubIntro = false; 

    void Awake()
    {
        // إذا كان هناك مدير حفظ آخر، دمره. نحن نحتاج واحداً فقط!
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // لا تدمره عند تغيير المشاهد
            LoadGame(); // أول ما تشتغل اللعبة، اقرأ البيانات
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- دوال الحفظ والتحميل المركزية ---

    public void SaveGame()
    {
        PlayerPrefs.SetInt("GateProgress", currentGateProgress);
        PlayerPrefs.SetInt("HasPlayedBefore", hasPlayedBefore ? 1 : 0);
        
        // 💾 حفظ حالة المشهد السينمائي
        PlayerPrefs.SetInt("HasSeenHubIntro", hasSeenHubIntro ? 1 : 0); 
        
        PlayerPrefs.Save();
        
        Debug.Log("💾 [SaveManager] تم حفظ تقدم نوار بنجاح!");
    }

    public void LoadGame()
    {
        currentGateProgress = PlayerPrefs.GetInt("GateProgress", 0);
        hasPlayedBefore = PlayerPrefs.GetInt("HasPlayedBefore", 0) == 1;
        
        // 📂 قراءة حالة المشهد السينمائي
        hasSeenHubIntro = PlayerPrefs.GetInt("HasSeenHubIntro", 0) == 1; 
        
        Debug.Log("📂 [SaveManager] تم تحميل ملف اللعبة. المرحلة الحالية: " + currentGateProgress);
    }

    // --- دوال مساعدة للواجهات والبوابات ---

    public bool HasSaveData()
    {
        return hasPlayedBefore;
    }

    public void StartNewGame()
    {
        currentGateProgress = 0;
        hasPlayedBefore = true;
        
        // 🔄 إعادة تعيين المشهد السينمائي ليعمل مرة أخرى في اللعبة الجديدة
        hasSeenHubIntro = false; 
        
        SaveGame();
        
        Debug.Log("✨ [SaveManager] تم تصفير البيانات وبدء رحلة جديدة!");
    }

    public void UnlockNextGate()
    {
        currentGateProgress++;
        SaveGame();
        
        // لاحقاً سنرسل إشارة هنا لمدير البيانات (Telemetry) لتسجيل فتح البوابة
    }
}