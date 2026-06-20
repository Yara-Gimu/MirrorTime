using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Cinemachine; 
using System.Collections;

public class MirrorCinematicSequence : MonoBehaviour
{
    [Header("--- مجسم المرآة (لأخذ الماتيريال منه) ---")]
    public GameObject mirrorObject; 

    [Header("--- كاميرا سطح المرآة (الريندر) ---")]
    public Camera renderTextureCamera; 

    [Header("--- الكاميرا السينمائية للانعكاس ---")]
    public CinemachineCamera reflectionShotCamera; // لازلنا نحتاجها عشان نصعدها لفوق
    public float reflectionViewTime = 2.0f;
    public float cameraUpwardMovement = 0.2f;
    public float holdTimeBeforeSmash = 1.5f; 

    [Header("--- تتبع الوجه ---")]
    public Transform playerFaceTarget;

    [Header("--- الماتيريال (الظهور التدريجي) ---")]
    public Material idleMirrorMaterial;
    public Material reflectionMaterial;
    public float reflectionFadeDuration = 1.0f;

    [Header("--- خدعة الكسر ---")]
    public GameObject[] crackOverlays;
    public float crackViewTime = 0.4f;

    [Header("--- الصوت والانتقال ---")]
    public AudioClip glassSmashSound; 
    private AudioSource audioSource;
    public string hubWorldSceneName = "The Hub World"; 

    [Header("--- الفلاش (UI) ---")]
    public Image flashPanel; 
    public float flashSpeed = 0.8f; 

    private bool isSequenceStarted = false;
    private RectTransform flashRectTransform;
    private PlayerStateMachine cachedPlayer;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (flashPanel != null) flashRectTransform = flashPanel.GetComponent<RectTransform>();
        cachedPlayer = FindFirstObjectByType<PlayerStateMachine>();
    }

    void Start()
    {
        if (flashPanel != null) flashPanel.canvasRenderer.SetAlpha(0f);
        if (flashRectTransform != null) flashRectTransform.localScale = Vector3.zero;
        
        foreach (var crack in crackOverlays)
        {
            if (crack != null) crack.SetActive(false);
        }

        if (mirrorObject != null && idleMirrorMaterial != null)
        {
            MeshRenderer glassRenderer = mirrorObject.GetComponentInChildren<MeshRenderer>();
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
        if (renderTextureCamera != null) 
        {
            renderTextureCamera.gameObject.SetActive(true);
            StartCoroutine(TrackPlayerFace());
        }

        StartCoroutine(FadeReflectionMaterial());

        // 🌟 الكاميرا اشتغلت خلاص من السكربت الأول.. هنا بس بنعطيها أمر الصعود
        if (reflectionShotCamera != null)
        {
            StartCoroutine(CinematicCameraUpwardPan());
        }

        yield return new WaitForSeconds(reflectionViewTime);
        yield return new WaitForSeconds(holdTimeBeforeSmash);

        if (glassSmashSound != null && audioSource != null) audioSource.PlayOneShot(glassSmashSound);
        
        foreach (var crack in crackOverlays)
        {
            if (crack != null) crack.SetActive(true);
        }

        yield return new WaitForSeconds(crackViewTime);
        
        yield return StartCoroutine(PulseFlash(flashSpeed));

        FinishSequence();
    }

    IEnumerator TrackPlayerFace()
    {
        while (true)
        {
            if (renderTextureCamera != null && playerFaceTarget != null)
            {
                renderTextureCamera.transform.LookAt(playerFaceTarget);
            }
            else if (renderTextureCamera != null && cachedPlayer != null)
            {
                renderTextureCamera.transform.LookAt(cachedPlayer.transform.position + Vector3.up * 1.5f);
            }
            yield return null;
        }
    }

    IEnumerator FadeReflectionMaterial()
    {
        if (mirrorObject == null || idleMirrorMaterial == null || reflectionMaterial == null) yield break;
        
        MeshRenderer glassRenderer = mirrorObject.GetComponentInChildren<MeshRenderer>();
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

    IEnumerator CinematicCameraUpwardPan()
    {
        if (reflectionShotCamera == null) yield break;

        Transform camTransform = reflectionShotCamera.transform;
        Vector3 startPos = camTransform.position;
        Vector3 targetPos = startPos + (Vector3.up * cameraUpwardMovement);

        float elapsed = 0f;
        while (elapsed < reflectionViewTime)
        {
            elapsed += Time.deltaTime;
            camTransform.position = Vector3.Lerp(startPos, targetPos, elapsed / reflectionViewTime);
            yield return null;
        }
        camTransform.position = targetPos;
    }

    IEnumerator PulseFlash(float duration)
    {
        if (flashRectTransform == null || flashPanel == null) yield break;
        
        flashRectTransform.localScale = Vector3.zero;
        flashPanel.canvasRenderer.SetAlpha(1f);
        float time = 0;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            flashRectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 15f, time / duration);
            yield return null;
        }
    }

    void FinishSequence()
    {
        SceneManager.LoadScene(hubWorldSceneName);
    }
}