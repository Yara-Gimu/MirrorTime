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

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    // 1. دالة الانتقال بين المشاهد (للمراحل المستقبلية)
    public void LoadSceneSmoothly(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    // 2. دالة جديدة: لظهور الكوميكس أو القوائم داخل نفس المشهد بنعومة! ✨
    public void ShowUIWithFade(GameObject uiToEnable, GameObject uiToDisable = null)
    {
        StartCoroutine(FadeOutShowUIFadeIn(uiToEnable, uiToDisable));
    }

    IEnumerator FadeIn()
    {
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

        SceneManager.LoadScene(sceneName);
        yield return null;
        StartCoroutine(FadeIn());
    }

    // الدالة السحرية لظهور الكوميكس
    IEnumerator FadeOutShowUIFadeIn(GameObject uiToEnable, GameObject uiToDisable)
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

        // تفعيل الكوميكس وإخفاء زر حرف E واللاعب في الظلام
        if (uiToEnable != null) uiToEnable.SetActive(true);
        if (uiToDisable != null) uiToDisable.SetActive(false);

        // فتح الشاشة مجدداً على الكوميكس!
        while (blackScreen.color.a > 0f)
        {
            c.a -= Time.deltaTime * fadeSpeed;
            blackScreen.color = c;
            yield return null;
        }
        blackScreen.gameObject.SetActive(false);
    }
}