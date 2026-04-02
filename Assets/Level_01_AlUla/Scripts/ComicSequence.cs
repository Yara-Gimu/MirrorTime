using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class ComicSequence : MonoBehaviour
{
    [Header("--- إعدادات الصوت والانتقال ---")]
    public AudioClip glassSmashSound; 
    private AudioSource audioSource;
    public string hubWorldSceneName = "The Hub World"; 

    [Header("--- نظام الإدخال للتخطي ---")]
    [Tooltip("نفس الزر اللي استخدمتيه للكسر، هنا بنستخدمه للتخطي")]
    public InputActionReference skipOrBreakAction; 

    [Header("--- إعدادات واجهة الكوميكس (UI) ---")]
    public Image flashPanel; 
    public Image comicDisplay; 
    public GameObject skipPromptText; 

    [Header("--- الصور والتوقيت ---")]
    public Sprite[] comicFrames; 
    public float flashSpeed = 0.5f; 
    public float frameDuration = 2.5f; 

    private bool isBroken = false;
    private bool isSequenceRunning = false;
    private Coroutine sequenceCoroutine;

    void Awake()
    {
        // سحبنا مكون الصوت في Awake عشان يكون جاهز أول ما يتفعل الكانفاس
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (flashPanel != null) flashPanel.canvasRenderer.SetAlpha(0f);
        if (comicDisplay != null) comicDisplay.gameObject.SetActive(false);
        if (skipPromptText != null) skipPromptText.SetActive(false);
    }

    void OnEnable()
    {
        if (skipOrBreakAction != null)
        {
            skipOrBreakAction.action.Enable(); 
            skipOrBreakAction.action.performed += OnInputPressed;
        }
    }

    void OnDisable()
    {
        if (skipOrBreakAction != null)
            skipOrBreakAction.action.performed -= OnInputPressed;
    }

    // 🌟 الدالة اللي راح تناديها المرآة (بوابة الدخول)
    public void StartSequenceFromMirror()
    {
        if (!isBroken)
        {
            isBroken = true;
            DisablePlayer();
            sequenceCoroutine = StartCoroutine(PlayCinematicSequence());
        }
    }

    // دالة لاستقبال زر التخطي
    private void OnInputPressed(InputAction.CallbackContext context)
    {
        if (isSequenceRunning)
        {
            SkipSequence();
        }
    }

    void DisablePlayer()
    {
        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
        if (player != null) player.enabled = false;
    }

    IEnumerator PlayCinematicSequence()
    {
        isSequenceRunning = true;

        if (glassSmashSound != null && audioSource != null) audioSource.PlayOneShot(glassSmashSound);
        if (skipPromptText != null) skipPromptText.SetActive(true);

        if (flashPanel != null) yield return FadeImage(flashPanel, 0f, 1f, flashSpeed);

        if (comicDisplay != null)
        {
            comicDisplay.gameObject.SetActive(true);
            for (int i = 0; i < comicFrames.Length; i++)
            {
                comicDisplay.sprite = comicFrames[i];
                
                if (flashPanel != null)
                {
                    flashPanel.canvasRenderer.SetAlpha(1f);
                    yield return FadeImage(flashPanel, 1f, 0f, flashSpeed / 2f);
                }

                yield return new WaitForSeconds(frameDuration);
            }
        }

        if (flashPanel != null) yield return FadeImage(flashPanel, 0f, 1f, flashSpeed);

        FinishSequence();
    }

    void SkipSequence()
    {
        if (sequenceCoroutine != null) StopCoroutine(sequenceCoroutine);
        if (flashPanel != null) flashPanel.canvasRenderer.SetAlpha(1f);
        FinishSequence();
    }

    void FinishSequence()
    {
        isSequenceRunning = false;

        if (FadeManager.instance != null)
            FadeManager.instance.LoadSceneSmoothly(hubWorldSceneName);
        else
            SceneManager.LoadScene(hubWorldSceneName);
    }

    IEnumerator FadeImage(Image img, float startAlpha, float endAlpha, float duration)
    {
        if (img == null) yield break;

        float time = 0;
        img.canvasRenderer.SetAlpha(startAlpha);
        
        while (time < duration)
        {
            time += Time.deltaTime;
            img.canvasRenderer.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / duration));
            yield return null;
        }
        img.canvasRenderer.SetAlpha(endAlpha);
    }
}