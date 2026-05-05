using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Cinemachine; 
using System.Collections;

public class MirrorCinematicSequence : MonoBehaviour
{
    [Header("--- إعدادات حركة المراية السحرية ---")]
    public FloatingObject mirrorToMove; 
    public float moveDistance = 1.5f; 

    [Header("--- كاميرا سطح المرآة (الريندر) ---")]
    public Camera renderTextureCamera; 

    [Header("--- الكاميرا السينمائية للانعكاس ---")]
    public CinemachineCamera reflectionShotCamera;
    
    [Tooltip("وقت حركة الكاميرا البطيئة للأعلى")]
    public float reflectionViewTime = 2.0f;
    
    [Tooltip("مقدار ارتفاع الكاميرا ببطء أثناء عرض الانعكاس")]
    public float cameraUpwardMovement = 0.2f;
    
    [Tooltip("الوقفة الدرامية: كم ثانية تثبت الكاميرا على وجه نوار قبل الكسر؟")]
    public float holdTimeBeforeSmash = 1.5f; 

    [Header("--- 🌟 إعدادات تتبع الوجه (الجديدة) ---")]
    [Tooltip("اسحبي التارقت (النقطة الفاضية) اللي حطيتيها في رأس نوار هنا")]
    public Transform playerFaceTarget;

    [Header("--- 🌟 إعدادات الماتيريال (الظهور التدريجي) ---")]
    [Tooltip("الماتيريال العادي للمراية (قبل لا تشتغل)")]
    public Material idleMirrorMaterial;
    [Tooltip("ماتيريال الانعكاس الحقيقي (الزجاج العاكس)")]
    public Material reflectionMaterial;
    [Tooltip("كم ثانية ياخذ التدرج عشان يظهر الانعكاس؟")]
    public float reflectionFadeDuration = 1.0f;

    [Header("--- إعدادات خدعة الكسر ---")]
    [Tooltip("حطي هنا قطع الكسر الأربع اللي رتبتيها قدام المراية")]
    public GameObject[] crackOverlays;
    
    [Tooltip("كم ثانية تظهر الكسرات قبل ما يبلع الفلاش الشاشة؟ (0.4 ثانية ممتازة جداً)")]
    public float crackViewTime = 0.4f;

    [Header("--- إعدادات الصوت والانتقال ---")]
    public AudioClip glassSmashSound; 
    private AudioSource audioSource;
    public string hubWorldSceneName = "The Hub World"; 

    [Header("--- إعدادات الفلاش (UI) ---")]
    public Image flashPanel; 
    public float flashSpeed = 0.8f; 
    
    [Header("--- توقيت الإخراج السينمائي ---")]
    public float reachAnimationDelay = 0.8f; 

    private bool isSequenceStarted = false;
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
        if (reflectionShotCamera != null) reflectionShotCamera.Priority = 0;

        // نتأكد إن قطع الكسر مخفية في البداية عشان ما تخرب المشهد
        foreach (var crack in crackOverlays)
        {
            if (crack != null) crack.SetActive(false);
        }

        // الكود يلقى الزجاج بنفسه ويحط عليه الماتيريال العادي في البداية 
        if (mirrorToMove != null && idleMirrorMaterial != null)
        {
            MeshRenderer glassRenderer = mirrorToMove.GetComponentInChildren<MeshRenderer>();
            if (glassRenderer != null) glassRenderer.material = idleMirrorMaterial;
        }
    }

    public void StartSequenceFromMirror()
    {
        if (!isSequenceStarted)
        {
            isSequenceStarted = true;
            StartCoroutine(PlayCinematicSequence());
        }
    }

    IEnumerator PlayCinematicSequence()
    {
        // 1. إيقاف تحكم اللاعب
        DisablePlayer();

        // 2. حركة نوار والمرآة
        StartCoroutine(SmoothRotatePlayerToMirror());
        StartCoroutine(MoveMirrorDirectlyToPlayer()); 

        // 3. انتظار وصول المرآة
        yield return new WaitForSeconds(reachAnimationDelay);

        // 4. تشغيل الكاميرات
        if (renderTextureCamera != null) 
        {
            renderTextureCamera.gameObject.SetActive(true);
            // 🌟 نشغل تتبع وجه نوار باستخدام التارقت
            StartCoroutine(TrackPlayerFace());
        }

        // 🌟 نشغل التدرج الناعم للماتيريال
        StartCoroutine(FadeReflectionMaterial());

        if (reflectionShotCamera != null)
        {
            reflectionShotCamera.gameObject.SetActive(true); 
            reflectionShotCamera.Priority = 200; 
            StartCoroutine(CinematicCameraUpwardPan());
        }

        // 5. انتظار انتهاء حركة الكاميرا
        yield return new WaitForSeconds(reflectionViewTime);

        // 6. الوقفة الدرامية (الصمت قبل الكسر)
        yield return new WaitForSeconds(holdTimeBeforeSmash);

        // 7. لحظة الكسر الحاسمة! 
        if (glassSmashSound != null && audioSource != null) audioSource.PlayOneShot(glassSmashSound);
        
        foreach (var crack in crackOverlays)
        {
            if (crack != null) crack.SetActive(true);
        }

        // 8. ننتظر جزء بسيط من الثانية 
        yield return new WaitForSeconds(crackViewTime);

        // 9. تأثير الفلاش الدائري المتوهج
        yield return StartCoroutine(PulseFlash(flashSpeed));

        // 10. الانتقال للمرحلة التالية
        FinishSequence();
    }

    // ==========================================
    // الكاميرا تطالع التارقت اللي حطيتيه في رأس نوار
    // ==========================================
    IEnumerator TrackPlayerFace()
    {
        while (true)
        {
            if (renderTextureCamera != null && playerFaceTarget != null)
            {
                renderTextureCamera.transform.LookAt(playerFaceTarget);
            }
            else if (renderTextureCamera != null)
            {
                PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
                if (player != null)
                    renderTextureCamera.transform.LookAt(player.transform.position + Vector3.up * 1.5f);
            }
            yield return null;
        }
    }

    // ==========================================
    // انتقال ناعم للماتيريال
    // ==========================================
    IEnumerator FadeReflectionMaterial()
    {
        if (mirrorToMove == null || idleMirrorMaterial == null || reflectionMaterial == null) yield break;
        
        MeshRenderer glassRenderer = mirrorToMove.GetComponentInChildren<MeshRenderer>();
        if (glassRenderer == null) yield break;

        Material matInstance = new Material(idleMirrorMaterial);
        glassRenderer.material = matInstance;

        float elapsed = 0f;
        while (elapsed < reflectionFadeDuration)
        {
            elapsed += Time.deltaTime;
            matInstance.Lerp(idleMirrorMaterial, reflectionMaterial, elapsed / reflectionFadeDuration);
            yield return null;
        }
        glassRenderer.material = reflectionMaterial;
    }

    // ==========================================
    // حركة الكاميرا السينمائية للأعلى
    // ==========================================
    IEnumerator CinematicCameraUpwardPan()
    {
        if (reflectionShotCamera == null) yield break;

        Transform camTransform = reflectionShotCamera.transform;
        Vector3 startPos = camTransform.position;
        Vector3 targetPos = startPos + (camTransform.up * cameraUpwardMovement);

        float elapsed = 0f;
        while (elapsed < reflectionViewTime)
        {
            elapsed += Time.deltaTime;
            camTransform.position = Vector3.Lerp(startPos, targetPos, elapsed / reflectionViewTime);
            yield return null;
        }
        camTransform.position = targetPos;
    }

    // ==========================================
    // تحريك المرآة باتجاه نوار مباشرة وتثبيتها
    // ==========================================
    IEnumerator MoveMirrorDirectlyToPlayer()
    {
        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
        if (player == null || mirrorToMove == null) yield break;

        mirrorToMove.enabled = false;

        Transform mTransform = mirrorToMove.transform;
        Vector3 startPos = mTransform.position;
        
        Vector3 directionToPlayer = (player.transform.position - mTransform.position).normalized;
        directionToPlayer.y = 0f; 

        Vector3 targetPos = startPos + (directionToPlayer * moveDistance);

        float elapsed = 0f;
        while (elapsed < reachAnimationDelay)
        {
            elapsed += Time.deltaTime;
            mTransform.position = Vector3.Lerp(startPos, targetPos, elapsed / reachAnimationDelay);
            yield return null;
        }
        mTransform.position = targetPos;
    }

    // ==========================================
    // لف اللاعب باتجاه المرآة
    // ==========================================
    IEnumerator SmoothRotatePlayerToMirror()
    {
        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
        if (player == null || mirrorToMove == null) yield break;

        Transform pTransform = player.transform;
        Vector3 directionToMirror = (mirrorToMove.transform.position - pTransform.position).normalized;
        directionToMirror.y = 0f; 

        if (directionToMirror != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToMirror);
            float elapsed = 0f;
            float duration = reachAnimationDelay; 

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                pTransform.rotation = Quaternion.Slerp(pTransform.rotation, targetRotation, elapsed / duration);
                yield return null;
            }
            pTransform.rotation = targetRotation; 
        }
    }

    // ==========================================
    // الفلاش والانتقال (يدعم الدائرة المتوهجة بمقاس 15)
    // ==========================================
    IEnumerator PulseFlash(float duration)
    {
        flashRectTransform.localScale = Vector3.zero;
        flashPanel.canvasRenderer.SetAlpha(1f);
        float time = 0;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            // الضرب في 15 يضمن تغطية الشاشة بالكامل
            flashRectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 15f, time / duration);
            yield return null;
        }
    }

    void DisablePlayer()
    {
        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
        if (player != null) player.enabled = false;
    }

    void FinishSequence()
    {
        SceneManager.LoadScene(hubWorldSceneName);
    }
}