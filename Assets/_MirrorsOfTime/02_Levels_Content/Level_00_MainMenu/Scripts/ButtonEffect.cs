using UnityEngine;
using UnityEngine.EventSystems; // 🌟 ضروري للماوس ويد التحكم
using TMPro;

// 🌟 أضفنا ISelectHandler و IDeselectHandler لدعم يد السوني والإكس بوكس
public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("إعدادات النص")]
    public TMP_Text buttonText;

    [Header("إعدادات الألوان")]
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = new Color(0.9f, 0.8f, 0.6f, 1f); // اللون الذهبي

    [Header("إعدادات التوهج (Glow)")]
    public Color glowColor = new Color(0.9f, 0.8f, 0.6f, 1f);
    [Range(0, 1)] public float glowPower = 0.5f;

    [Header("إعدادات الصوت (Audio)")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private AudioSource audioSource;
    private Material textMat;
    private bool isHighlighted = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (buttonText != null)
        {
            // إنشاء نسخة من الماتيريال حتى لا نتلاعب بالخط الأصلي للعبة كاملة
            textMat = new Material(buttonText.fontMaterial);
            buttonText.fontMaterial = textMat;
        }
        
        ResetEffect();
    }

    // ==========================================
    // 1. أوامر الماوس (PC)
    // ==========================================
    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 🌟 حماية ذكية: لا تطفئ التوهج إذا كانت يد التحكم لا تزال تقف على الزر!
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            ResetEffect();
        }
    }

    // ==========================================
    // 2. أوامر يد التحكم والكيبورد (Consoles)
    // ==========================================
    public void OnSelect(BaseEventData eventData)
    {
        ApplyEffect();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ResetEffect();
    }

    // ==========================================
    // 3. المنطق البصري والصوتي (AAA Standard)
    // ==========================================
    private void ApplyEffect()
    {
        if (isHighlighted) return;
        isHighlighted = true;

        if (buttonText != null)
        {
            buttonText.color = hoverTextColor;
            if (textMat != null)
            {
                textMat.EnableKeyword("GLOW_ON");
                textMat.SetColor("_GlowColor", glowColor);
                textMat.SetFloat("_GlowPower", glowPower);
            }
        }

        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    private void ResetEffect()
    {
        isHighlighted = false;

        if (buttonText != null)
        {
            buttonText.color = normalTextColor;
            if (textMat != null)
            {
                textMat.DisableKeyword("GLOW_ON");
                textMat.SetFloat("_GlowPower", 0f);
            }
        }
    }
}