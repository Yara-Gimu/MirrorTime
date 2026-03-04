using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Slider))]
public class SliderAudio : MonoBehaviour
{
    [Header("إعدادات الصوت")]
    public AudioClip tickSound; // ضعي هنا صوت التكتكة أو الاحتكاك الحجري

    [Header("إعدادات التوقيت")]
    [Tooltip("المدة الزمنية بين كل تكتكة وأخرى (لمنع تداخل الأصوات)")]
    public float tickCooldown = 0.05f; 
    
    private AudioSource audioSource;
    private Slider slider;
    private float nextTickTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        slider = GetComponent<Slider>();

        // نمنع تشغيل الصوت تلقائياً عند بدء اللعبة
        audioSource.playOnAwake = false;

        // السحر هنا: نربط دالة الصوت بحركة السلايدر برمجياً بدون ما نتدخل في الانسبكتور
        slider.onValueChanged.AddListener(PlayTickSound);
    }

    // هذه الدالة تشتغل كل ما تحرك السلايدر
    public void PlayTickSound(float value)
    {
        // نتحقق إذا كان الوقت الحالي تجاوز وقت "الراحة" المسموح به
        if (tickSound != null && Time.time >= nextTickTime)
        {
            // تغيير النغمة بشكل عشوائي بسيط لزيادة الواقعية
            audioSource.pitch = Random.Range(0.95f, 1.05f); 
            audioSource.PlayOneShot(tickSound);
            
            // نحدث وقت التكتكة القادمة
            nextTickTime = Time.time + tickCooldown;
        }
    }
}