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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (blackScreen == null)
        {
            blackScreen = GetComponentInChildren<Image>(true);
        }
        StartCoroutine(FadeIn());
    }

    public void LoadSceneSmoothly(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    public void ShowUIWithFade(GameObject uiToEnable, GameObject uiToDisable = null)
    {
        StartCoroutine(FadeOutShowUIFadeIn(uiToEnable, uiToDisable));
    }

    IEnumerator FadeIn()
    {
        if (blackScreen == null) yield break; 

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

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOutShowUIFadeIn(GameObject uiToEnable, GameObject uiToDisable)
    {
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            Color c = blackScreen.color;
            c.a = 0f; 

            while (blackScreen.color.a < 1f)
            {
                c.a += Time.deltaTime * fadeSpeed;
                blackScreen.color = c;
                yield return null;
            }
        }

        if (uiToEnable != null) uiToEnable.SetActive(true);
        if (uiToDisable != null) uiToDisable.SetActive(false);

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