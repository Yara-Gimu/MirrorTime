using UnityEngine;
using UnityEngine.UI;
using System.Collections; // ضروري للـ Coroutine

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Slider))]
public class SliderAudio : MonoBehaviour
{
    [Header("إعدادات الصوت")]
    public AudioClip tickSound; 

    [Header("إعدادات التوقيت")]
    [Tooltip("المدة الزمنية بين كل تكتكة وأخرى (لمنع تداخل الأصوات)")]
    public float tickCooldown = 0.05f; 
    
    private AudioSource audioSource;
    private Slider slider;
    private float nextTickTime = 0f;
    
    // 🌟 حماية ضد تشغيل الصوت وقت تحميل الإعدادات المحفوظة
    private bool isInitialized = false; 

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        slider = GetComponent<Slider>();

        audioSource.playOnAwake = false;
        slider.onValueChanged.AddListener(PlayTickSound);
    }

    IEnumerator Start()
    {
        // 🌟 ننتظر نهاية الفريم الأول حتى تنتهي كل السكربتات من قراءة وتطبيق الإعدادات
        yield return new WaitForEndOfFrame();
        isInitialized = true; 
    }

    public void PlayTickSound(float value)
    {
        // إذا اللعبة لسه تحمل الإعدادات، لا تشغل الصوت!
        if (!isInitialized) return;

        if (tickSound != null && Time.time >= nextTickTime)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); 
            audioSource.PlayOneShot(tickSound);
            
            nextTickTime = Time.time + tickCooldown;
        }
    }
}