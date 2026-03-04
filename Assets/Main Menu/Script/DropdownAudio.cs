using UnityEngine;
using TMPro; // ضروري للتعامل مع قوائم TextMeshPro
using UnityEngine.EventSystems; // ضروري للتعرف على النقرات

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownAudio : MonoBehaviour, IPointerClickHandler
{
    [Header("إعدادات الصوت")]
    public AudioClip openSound;   // صوت فتح القائمة (حفيف ورق أو انزلاق)
    public AudioClip selectSound; // صوت تأكيد الاختيار (نقرة زجاجية/نحاسية)

    private AudioSource audioSource;
    private TMP_Dropdown dropdown;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        dropdown = GetComponent<TMP_Dropdown>();
        
        // ربط دالة الاختيار بتغير القيمة
        dropdown.onValueChanged.AddListener(PlaySelectSound);
    }

    // هذه الدالة تشتغل بمجرد النقر على القائمة لفتحها
    public void OnPointerClick(PointerEventData eventData)
    {
        if (openSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(openSound);
        }
    }

    // هذه الدالة تشتغل لما اللاعب يختار لغة أو جودة جديدة
    public void PlaySelectSound(int index)
    {
        if (selectSound != null)
        {
            audioSource.pitch = 1f; // نغمة ثابتة للتأكيد
            audioSource.PlayOneShot(selectSound);
        }
    }
}