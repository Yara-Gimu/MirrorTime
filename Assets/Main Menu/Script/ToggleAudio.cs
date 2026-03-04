using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Toggle))]
public class ToggleAudio : MonoBehaviour
{
    [Header("إعدادات الصوت")]
    public AudioClip toggleOnSound;  // صوت التفعيل (وضع علامة الصح)
    public AudioClip toggleOffSound; // صوت الإلغاء (إزالة علامة الصح)

    private AudioSource audioSource;
    private Toggle toggle;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        toggle = GetComponent<Toggle>();
        
        // نربط الدالة بتغير حالة المربع برمجياً
        toggle.onValueChanged.AddListener(PlayToggleSound);
    }

    // هذي الدالة تشتغل تلقائياً كل ما ضغط اللاعب على المربع
    public void PlayToggleSound(bool isOn)
    {
        // تغيير النغمة بشكل عشوائي بسيط لزيادة الواقعية
        audioSource.pitch = Random.Range(0.95f, 1.05f);

        // إذا اللاعب حط صح
        if (isOn && toggleOnSound != null)
        {
            audioSource.PlayOneShot(toggleOnSound);
        }
        // إذا اللاعب شال الصح
        else if (!isOn && toggleOffSound != null)
        {
            audioSource.PlayOneShot(toggleOffSound);
        }
    }
}