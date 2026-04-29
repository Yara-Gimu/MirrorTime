using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance; 
    
    [Header("إعدادات الستارة")]
    public Image blackScreen;
    public float fadeSpeed = 1.5f; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 1. الاشتراك في حدث تحميل المشهد أول ما يتفعل السكريبت
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 2. فك الاشتراك لتجنب أخطاء الذاكرة (خطوة هندسية مهمة جداً)
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 3. هذه الدالة ستعمل تلقائياً وبشكل سحري في بداية كل مشهد جديد
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // إذا ضاعت الستارة لأي سبب، يبحث عنها مجدداً (كما طلب الدكتور)
        if (blackScreen == null)
        {
            blackScreen = GetComponentInChildren<Image>(true);
        }

        // تشغيل التلاشي (فتح الستارة) فور الدخول للمشهد
        StartCoroutine(FadeIn());
    }

    // 1. دالة الانتقال بين المشاهد (للمراحل المستقبلية)
    public void LoadSceneSmoothly(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    // 2. دالة لظهور الكوميكس أو القوائم داخل نفس المشهد بنعومة! ✨
    public void ShowUIWithFade(GameObject uiToEnable, GameObject uiToDisable = null)
    {
        StartCoroutine(FadeOutShowUIFadeIn(uiToEnable, uiToDisable));
    }

    IEnumerator FadeIn()
    {
        if (blackScreen == null) yield break; // حماية إضافية

        blackScreen.gameObject.SetActive(true);
        Color c = blackScreen.color;
        c.a = 1f; 
        blackScreen.color = c;

        while (blackScreen.color.a > 0f)
        {
            c.a -= Time.deltaTime * fadeSpeed;
            blackScreen.color = c;
            yield return null;
        }
        blackScreen.gameObject.SetActive(false); 
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            Color c = blackScreen.color;
            c.a = 0f; 
            blackScreen.color = c;

            while (blackScreen.color.a < 1f)
            {
                c.a += Time.deltaTime * fadeSpeed;
                blackScreen.color = c;
                yield return null;
            }
        }

        // الانتقال للمشهد الجديد
        SceneManager.LoadScene(sceneName);
        
        // ملاحظة: تم حذف StartCoroutine(FadeIn) من هنا، لأن دالة OnSceneLoaded 
        // ستتكفل بفتح الستارة تلقائياً أول ما يخلص تحميل المشهد الجديد!
    }

    // الدالة السحرية لظهور الكوميكس
    IEnumerator FadeOutShowUIFadeIn(GameObject uiToEnable, GameObject uiToDisable)
    {
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            Color c = blackScreen.color;
            c.a = 0f; 

            // تظليم الشاشة أولاً
            while (blackScreen.color.a < 1f)
            {
                c.a += Time.deltaTime * fadeSpeed;
                blackScreen.color = c;
                yield return null;
            }
        }

        // تفعيل الكوميكس وإخفاء زر حرف E واللاعب في الظلام
        if (uiToEnable != null) uiToEnable.SetActive(true);
        if (uiToDisable != null) uiToDisable.SetActive(false);

        // فتح الشاشة مجدداً على الكوميكس!
        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            while (blackScreen.color.a > 0f)
            {
                c.a -= Time.deltaTime * fadeSpeed;
                blackScreen.color = c;
                yield return null;
            }
            blackScreen.gameObject.SetActive(false);
        }
    }
}