using UnityEngine;
using UnityEngine.EventSystems; 
using TMPro; 

// هذا السطر يخلي يونيتي يضيف (AudioSource) تلقائياً للزر عشان ما تنسين!
[RequireComponent(typeof(AudioSource))] 
public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    [Header("Settings")]
    public TextMeshProUGUI buttonText; 

    [Header("Colors")]
    public Color normalTextColor = new Color32(46, 35, 31, 255); 
    public Color hoverTextColor = new Color32(232, 224, 213, 255); 

    [Header("Glow Settings")]
    public Color glowColor = new Color32(232, 224, 213, 150); 
    [Range(0f, 1f)]
    public float glowPower = 0.5f; 

    [Header("Audio Settings (إعدادات الصوت)")]
    public AudioClip hoverSound; // صوت المرور على الزر
    public AudioClip clickSound; // صوت الضغط/الاختيار
    private AudioSource audioSource;

    private Material textMaterial;
    private Vector3 originalScale; 

    void Awake()
    {
        originalScale = transform.localScale;
        
        if (buttonText != null)
        {
            textMaterial = buttonText.fontMaterial;
        }

        // تجهيز مشغل الصوت وإقفال التشغيل التلقائي
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        ResetButton();
    }

    // --- أوامر المرور (Hover) بالماوس والقير ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyHoverEffect();
        PlayHoverSound();
    }

    public void OnSelect(BaseEventData eventData)
    {
        ApplyHoverEffect();
        PlayHoverSound();
    }

    // --- أوامر الخروج (Exit) ---
    public void OnPointerExit(PointerEventData eventData)
    {
        ResetButton();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ResetButton();
    }

    // --- أوامر الضغط/الاختيار (Click) بالماوس أو القير ---
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClickSound();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PlayClickSound();
    }

    // ✨ --- دوال التأثيرات ---
    void ApplyHoverEffect()
    {
        if(buttonText != null) buttonText.color = hoverTextColor;
        if(textMaterial != null)
        {
            textMaterial.EnableKeyword("GLOW_ON");
            textMaterial.SetColor("_GlowColor", glowColor);
            textMaterial.SetFloat("_GlowPower", glowPower);
        }
        transform.localScale = originalScale * 1.05f; 
    }

    void ResetButton()
    {
        if(buttonText != null) buttonText.color = normalTextColor;
        if(textMaterial != null)
        {
            textMaterial.DisableKeyword("GLOW_ON");
            textMaterial.SetFloat("_GlowPower", 0f);
        }
        transform.localScale = originalScale;
    }

    // 🎵 --- دوال تشغيل الصوت ---
    void PlayHoverSound()
    {
        if (hoverSound != null)
        {
            // تغيير النغمة بشكل بسيط جداً عشان ما يصير الصوت مكرر وممل
            audioSource.pitch = Random.Range(0.95f, 1.05f); 
            audioSource.PlayOneShot(hoverSound);
        }
    }

    void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // تغيير النغمة للضغطة
            audioSource.PlayOneShot(clickSound);
        }
    }
}