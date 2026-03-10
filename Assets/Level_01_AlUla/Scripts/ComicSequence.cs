using UnityEngine;
using UnityEngine.InputSystem; // ضروري لنظام التحكم الجديد
using UnityEngine.SceneManagement;

public class ComicSequence : MonoBehaviour
{
    [Header("إعدادات الكسر")]
    public AudioClip glassSmashSound; 
    private AudioSource audioSource;
    
    [Tooltip("اكتبي هنا اسم مشهد الغرفة الحارسة بالضبط زي ما حفظتيه")]
    public string hubWorldSceneName = "The Hub World"; 

    [Header("نظام الإدخال (الجديد)")]
    public InputActionReference skipOrBreakAction; // اسحبي زر Space / أو إكس البلايستيشن هنا

    private bool isBroken = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (skipOrBreakAction != null)
        {
            skipOrBreakAction.action.Enable(); // 👈 السطر السحري لتفعيل الزر إجبارياً
            skipOrBreakAction.action.performed += OnBreakPressed;
        }
    }

    void OnDisable()
    {
        if (skipOrBreakAction != null)
            skipOrBreakAction.action.performed -= OnBreakPressed;
    }

    private void OnBreakPressed(InputAction.CallbackContext context)
    {
        // إذا ضغط الزر والمرآة لسى ما انكسرت
        if (!isBroken)
        {
            isBroken = true;
            
            // 👈 تخدير نوار: نطفي العقل المدبر حقها عشان ما تقفز في الخلفية!
            PlayerStateMachine player = FindObjectOfType<PlayerStateMachine>();
            if (player != null) player.enabled = false;

            SmashAndTransport();
        }
    }

    void SmashAndTransport()
    {
        if (glassSmashSound != null)
        {
            audioSource.PlayOneShot(glassSmashSound);
        }

        transform.localPosition = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), 0);

        Invoke("LoadHubWorld", 1.5f);
    }

    void LoadHubWorld()
    {
        // 🏗️ الترقية المعمارية 1: تحديث ملف الحفظ بأن اللاعب بدأ اللعبة رسمياً وتجاوز الافتتاحية
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.StartNewGame();
        }

        // 🏗️ الترقية المعمارية 2: إرسال إشارة لمدير البيانات لتسجيل الوقت المستغرق
        // EventManager.Trigger("Telemetry_Prologue_Completed");

        if (FadeManager.instance != null)
            FadeManager.instance.LoadSceneSmoothly(hubWorldSceneName);
        else
            SceneManager.LoadScene(hubWorldSceneName);
    }
}