using UnityEngine;
using UnityEngine.EventSystems; 

// هذا السطر يخلي يونيتي يضيف (AudioSource) تلقائياً
[RequireComponent(typeof(AudioSource))] 
public class TabButtonSFX : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler, ISubmitHandler
{
    [Header("Audio Settings (إعدادات الصوت)")]
    public AudioClip hoverSound; // صوت المرور على الزر
    public AudioClip clickSound; // صوت الضغط/الاختيار
    private AudioSource audioSource;

    void Awake()
    {
        // تجهيز مشغل الصوت
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // توجيه الصوت لقناة الـ SFX (عشان سلايدر المؤثرات يقدر يتحكم فيه)
        // ملاحظة: يفضل تعيين الـ Output يدوياً من الإنسبكتور
    }

    // --- أوامر المرور (Hover) بالماوس والقير ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverSound();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHoverSound();
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

    // 🎵 --- دوال تشغيل الصوت ---
    void PlayHoverSound()
    {
        if (hoverSound != null)
        {
            // تغيير النغمة بشكل بسيط جداً عشان ما يصير الصوت مكرر
            audioSource.pitch = Random.Range(0.95f, 1.05f); 
            audioSource.PlayOneShot(hoverSound);
        }
    }

    void PlayClickSound()
    {
        if (clickSound != null)
        {
            // تغيير النغمة للضغطة
            audioSource.pitch = Random.Range(0.9f, 1.1f); 
            audioSource.PlayOneShot(clickSound);
        }
    }
}