#pragma warning disable CS0618 // كاتم التحذيرات الصفراء ليونيتي 6
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using Unity.Cinemachine; 

public class PhotoModeManager : MonoBehaviour
{
    [Header("--- واجهات المستخدم ---")]
    public GameObject photoModeRootUI;     
    public CanvasGroup mainHUDCanvas;      
    public GameObject cameraGridOverlay;   
    public CanvasGroup flashEffectCanvas;  
    public GameObject controlBarPanel; 

    [Header("--- إعدادات الإدخال (Input) ---")]
    public InputActionReference togglePhotoModeAction; 
    public InputActionReference captureAction;
    public InputActionReference zoomAction;       
    public InputActionReference toggleGridAction; 

    [Header("--- إعدادات الكاميرا والزووم (Cinemachine) ---")]
    [Tooltip("اسحبي مجسم الكاميرا السينمائية FreeLook وضعيها هنا (يقبل أي مجسم GameObject!)")]
    public GameObject cinemachineCameraObject; 
    
    public Camera photoCamera; 
    public float zoomSpeed = 20f; // السرعة الأساسية
    public float minFOV = 20f; 
    public float maxFOV = 60f; 
    
    [Header("--- الصوت والمؤثرات ---")]
    public AudioSource cameraClickSound;
    public AudioSource lensZoomSound; 
    public float flashSpeed = 4f;

    private CinemachineFreeLook cmFreeLook; 
    private bool isPhotoModeActive = false; 
    private bool isTakingPhoto = false;
    private bool isGridVisible = true;
    private float targetFOV;

    private void Start()
    {
        if (photoModeRootUI != null) photoModeRootUI.SetActive(false);
        if (flashEffectCanvas != null) flashEffectCanvas.alpha = 0f;
        
        // استخراج الكاميرا السينمائية بذكاء
        if (cinemachineCameraObject != null)
        {
            cmFreeLook = cinemachineCameraObject.GetComponent<CinemachineFreeLook>();
            if (cmFreeLook != null) targetFOV = cmFreeLook.m_Lens.FieldOfView;
        }
        else if (photoCamera != null) targetFOV = photoCamera.fieldOfView;
    }

    private void OnEnable()
    {
        if (togglePhotoModeAction != null)
        {
            togglePhotoModeAction.action.Enable();
            togglePhotoModeAction.action.performed += TogglePhotoMode;
        }
        if (captureAction != null) captureAction.action.Enable();
        if (zoomAction != null) zoomAction.action.Enable();
        if (toggleGridAction != null)
        {
            toggleGridAction.action.Enable();
            toggleGridAction.action.performed += ToggleGrid; 
        }
        if (captureAction != null) captureAction.action.performed += OnCapturePressed;
    }

    private void OnDisable()
    {
        if (togglePhotoModeAction != null) togglePhotoModeAction.action.performed -= TogglePhotoMode;
        if (captureAction != null) captureAction.action.performed -= OnCapturePressed;
        if (toggleGridAction != null) toggleGridAction.action.performed -= ToggleGrid;
    }

    private void Update()
    {
        if (isPhotoModeActive) HandleZoom(); 
    }

    private void TogglePhotoMode(InputAction.CallbackContext context)
    {
        isPhotoModeActive = !isPhotoModeActive;

        // 🌟 بأسلوب البساطة: بمجرد ضغط P، اقتل أي تنبيه مفتوح فوراً!
        if (InGameNotificationManager.Instance != null)
        {
            InGameNotificationManager.Instance.HideNotificationImmediate();
        }

        if (photoModeRootUI != null) photoModeRootUI.SetActive(isPhotoModeActive);

        if (mainHUDCanvas != null)
        {
            mainHUDCanvas.alpha = isPhotoModeActive ? 0f : 1f;
            mainHUDCanvas.interactable = !isPhotoModeActive;
            mainHUDCanvas.blocksRaycasts = !isPhotoModeActive;
        }

        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            if (isPhotoModeActive)
            {
                playerInput.actions.FindAction("Move")?.Disable();
                playerInput.actions.FindAction("Jump")?.Disable();
                playerInput.actions.FindAction("Attack")?.Disable();
                playerInput.actions.FindAction("Pause")?.Disable(); 
            }
            else
            {
                // عند الإغلاق: نوقف الكوروتين لضمان عدم ظهور الواجهات
                StopAllCoroutines();
                isTakingPhoto = false;

                playerInput.actions.FindAction("Move")?.Enable();
                playerInput.actions.FindAction("Jump")?.Enable();
                playerInput.actions.FindAction("Attack")?.Enable();
                playerInput.actions.FindAction("Pause")?.Enable();
            }
        }

        if (!isPhotoModeActive)
        {
            targetFOV = maxFOV;
            if (cmFreeLook != null) cmFreeLook.m_Lens.FieldOfView = maxFOV;
            else if (photoCamera != null) photoCamera.fieldOfView = maxFOV;
        }
    }

    private void HandleZoom()
    {
        if (zoomAction == null) return;
        
        float zoomInput = zoomAction.action.ReadValue<float>();

        if (zoomInput != 0)
        {
            // 🌟 الخدعة الذكية: الماوس يعطي أرقاماً ضخمة (120)، والكنترولر صغيراً (1).
            // إذا كان الرقم ضخماً (ماوس)، نعطيه قوة ضرب 3.5، وإذا كان صغيراً، نتركه كما هو.
            float force = (zoomInput > 10 || zoomInput < -10) ? Mathf.Sign(zoomInput) * 3.5f : zoomInput;

            if (lensZoomSound != null && !lensZoomSound.isPlaying) lensZoomSound.Play();
            
            // نزيد السرعة العامة ليكون الزووم ملحوظاً وسريعاً
            targetFOV -= force * (zoomSpeed * 2.5f) * Time.deltaTime;
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        }
        else
        {
            if (lensZoomSound != null && lensZoomSound.isPlaying) lensZoomSound.Stop();
        }
        
        // تطبيق الزووم على الكاميرا السينمائية
        if (cmFreeLook != null)
        {
            cmFreeLook.m_Lens.FieldOfView = Mathf.Lerp(cmFreeLook.m_Lens.FieldOfView, targetFOV, Time.deltaTime * 10f);
        }
        else if (photoCamera != null)
        {
            photoCamera.fieldOfView = Mathf.Lerp(photoCamera.fieldOfView, targetFOV, Time.deltaTime * 10f);
        }
    }

    private void ToggleGrid(InputAction.CallbackContext context)
    {
        if (!isPhotoModeActive) return; 
        if (cameraGridOverlay != null)
        {
            isGridVisible = !isGridVisible;
            cameraGridOverlay.SetActive(isGridVisible);
        }
    }

    private void OnCapturePressed(InputAction.CallbackContext context)
    {
        if (!isPhotoModeActive || isTakingPhoto) return; 
        StartCoroutine(TakePhotoRoutine());
    }

    IEnumerator TakePhotoRoutine()
    {
        isTakingPhoto = true;

        if (cameraGridOverlay != null) cameraGridOverlay.SetActive(false);
        if (controlBarPanel != null) controlBarPanel.SetActive(false); 

        yield return new WaitForEndOfFrame();

        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "MirrorsOfTime");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string fileName = "RuneShot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string fullPath = Path.Combine(folderPath, fileName);
        ScreenCapture.CaptureScreenshot(fullPath);

        if (cameraClickSound != null) cameraClickSound.Play();
        if (flashEffectCanvas != null) flashEffectCanvas.alpha = 1f;

        while (flashEffectCanvas != null && flashEffectCanvas.alpha > 0f)
        {
            flashEffectCanvas.alpha -= Time.deltaTime * flashSpeed;
            yield return null;
        }

        // لا نعيد الواجهات إلا إذا كانت الكاميرا ما تزال مفتوحة
        if (isPhotoModeActive)
        {
            if (controlBarPanel != null) controlBarPanel.SetActive(true);
            if (cameraGridOverlay != null) cameraGridOverlay.SetActive(isGridVisible); 
        }

        isTakingPhoto = false;
    }
}