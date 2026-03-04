using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicSequence : MonoBehaviour
{
    [Header("إعدادات الكسر")]
    public AudioClip glassSmashSound; // حطي هنا صوت كسر زجاج
    private AudioSource audioSource;
    
    [Tooltip("اكتبي هنا اسم مشهد الغرفة الحارسة بالضبط زي ما حفظتيه")]
    public string hubWorldSceneName = "The Hub World"; // تأكدي إن الاسم مطابق 100%

    private bool isBroken = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // إذا اللاعب ضغط Space والمرآة لسى ما انكسرت
        if (!isBroken && Input.GetKeyDown(KeyCode.Space))
        {
            isBroken = true;
            SmashAndTransport();
        }
    }

    void SmashAndTransport()
    {
        // 1. تشغيل صوت الكسر
        if (glassSmashSound != null)
        {
            audioSource.PlayOneShot(glassSmashSound);
        }

        // 2. إضافة تأثير اهتزاز الشاشة (خدعة بصرية سريعة)
        transform.localPosition = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), 0);

        // 3. ننتظر ثانية ونص (عشان اللاعب يسمع الكسرة وينفجع) بعدين ننقله للمشهد الثاني
        Invoke("LoadHubWorld", 1.5f);
    }

    void LoadHubWorld()
    {
        // 🎬 التعديل السحري: الانتقال بنعومة للغرفة الحارسة بدلاً من القطع المفاجئ
        if (FadeManager.instance != null)
            FadeManager.instance.LoadSceneSmoothly(hubWorldSceneName);
        else
            SceneManager.LoadScene(hubWorldSceneName); // انتقال عادي لو الستارة مو موجودة
    }
}