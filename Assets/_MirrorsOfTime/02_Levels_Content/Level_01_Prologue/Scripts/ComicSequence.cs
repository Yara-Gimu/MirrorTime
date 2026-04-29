using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class ComicSequence : MonoBehaviour
{
    [Header("--- إعدادات حركة المراية السحرية ---")]
    public FloatingObject mirrorToMove; 
    public float moveDistance = 1.5f; 

    [Header("--- إعدادات الصوت والانتقال ---")]
    public AudioClip glassSmashSound; 
    private AudioSource audioSource;
    public string hubWorldSceneName = "The Hub World"; 

    [Header("--- نظام الإدخال للتخطي ---")]
    public InputActionReference skipOrBreakAction; 

    [Header("--- إعدادات واجهة الكوميكس (UI) ---")]
    public Image flashPanel; 
    public Image comicDisplay; 
    public GameObject skipPromptText; 

    [Header("--- الصور والتوقيت ---")]
    public Sprite[] comicFrames; 
    public float flashSpeed = 0.2f; 
    public float frameDuration = 2.5f; 
    
    [Header("--- توقيت الإخراج السينمائي ---")]
    public float reachAnimationDelay = 0.8f; 

    private bool isBroken = false;
    private Coroutine sequenceCoroutine;
    private RectTransform flashRectTransform;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (flashPanel != null) flashRectTransform = flashPanel.GetComponent<RectTransform>();
    }

    void Start()
    {
        if (flashPanel != null) flashPanel.canvasRenderer.SetAlpha(0f);
        if (flashRectTransform != null) flashRectTransform.localScale = Vector3.zero;
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

    public void StartSequenceFromMirror()
    {
        if (!isBroken)
        {
            isBroken = true;
            sequenceCoroutine = StartCoroutine(PlayCinematicSequence());
        }
    }

    IEnumerator PlayCinematicSequence()
    {

        // 🌟 1. السحر الجديد: نلف نوار عشان تطالع في المراية بنعومة!
        StartCoroutine(SmoothRotatePlayerToMirror());

        // 2. المراية تقرب
        if (mirrorToMove != null) mirrorToMove.MoveMirrorForward(moveDistance, reachAnimationDelay);

        // 3. ننتظر نوار تمد يدها وتلف
        yield return new WaitForSeconds(reachAnimationDelay);

        // 4. تجميد اللاعب
        DisablePlayer();

        // 5. الصوت والكسر
        if (glassSmashSound != null && audioSource != null) audioSource.PlayOneShot(glassSmashSound);
        if (skipPromptText != null) skipPromptText.SetActive(true);

        // 6. الفلاش اللي يكبر
        yield return StartCoroutine(PulseFlash(flashSpeed));

        // 7. عرض الكوميكس
        if (comicDisplay != null)
        {
            comicDisplay.gameObject.SetActive(true);
            foreach (var frame in comicFrames)
            {
                comicDisplay.sprite = frame;
                yield return StartCoroutine(FadeImage(flashPanel, 0.5f, 0f, flashSpeed)); 
                yield return new WaitForSeconds(frameDuration);
            }
        }

        FinishSequence();
    }

    // ==========================================
    // 🌟 دالة دوران نوار السينمائي
    // ==========================================
    IEnumerator SmoothRotatePlayerToMirror()
    {
        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
        if (player == null || mirrorToMove == null) yield break;

        Transform pTransform = player.transform;
        
        // نحسب الاتجاه للمراية (نقفل محور Y عشان نوار ما تطالع في الأرض أو السماء وتخرب وقفتها)
        Vector3 directionToMirror = (mirrorToMove.transform.position - pTransform.position).normalized;
        directionToMirror.y = 0f; 

        if (directionToMirror != Vector3.zero)
        {
            // الدوران الهدف اللي نبغى نوصل له
            Quaternion targetRotation = Quaternion.LookRotation(directionToMirror);
            
            float elapsed = 0f;
            // نستخدم نفس وقت مدة اليد عشان الحركة تكون متزامنة 100%
            float duration = reachAnimationDelay; 

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                // Slerp: دالة رياضية تلف المجسم بنعومة من زاويته الحالية للزاوية الهدف
                pTransform.rotation = Quaternion.Slerp(pTransform.rotation, targetRotation, elapsed / duration);
                yield return null;
            }
            
            // تأكيد الوقفة الصحيحة في النهاية
            pTransform.rotation = targetRotation; 
        }
    }

    IEnumerator PulseFlash(float duration)
    {
        flashRectTransform.localScale = Vector3.zero;
        flashPanel.canvasRenderer.SetAlpha(1f);
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            flashRectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 2f, time / duration);
            yield return null;
        }
    }

    void DisablePlayer()
    {
        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
        if (player != null) player.enabled = false;
    }

    private void OnInputPressed(InputAction.CallbackContext context) => SkipSequence();

    void SkipSequence()
    {
        if (sequenceCoroutine != null) StopCoroutine(sequenceCoroutine);
        FinishSequence();
    }

    void FinishSequence()
    {
        SceneManager.LoadScene(hubWorldSceneName);
    }

    IEnumerator FadeImage(Image img, float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            img.canvasRenderer.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / duration));
            yield return null;
        }
    }
}