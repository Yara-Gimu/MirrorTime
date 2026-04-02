using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioZoneTransition : MonoBehaviour
{
    [Header("إعدادات الميكسر")]
    public AudioMixer mixer;

    [Header("أسماء المتغيرات في الميكسر")]
    public string outsideVolumeParam = "OutsideVol";
    public string insideVolumeParam = "InsideVol";

    [Header("سرعة الانتقال")]
    [Tooltip("وقت التلاشي بالثواني (الأفضل بين 3 إلى 5 ثواني للكهوف)")]
    public float fadeTime = 4f;

    // 🌟 السلاح السري: قفل عشان نمنع تداخل الأصوات لو اللاعب تحرك يمين ويسار عند الباب
    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        // إذا نوار دخلت الكهف (وهي ما كانت داخله من قبل)
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            isPlayerInside = true;
            StopAllCoroutines();
            StartCoroutine(CrossfadeMixer(outsideVolumeParam, insideVolumeParam));
            Debug.Log("🎶 دخول الكهف: تدرج احترافي للصوت...");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // إذا نوار طلعت من الكهف (وهي كانت داخله)
        if (other.CompareTag("Player") && isPlayerInside)
        {
            isPlayerInside = false;
            StopAllCoroutines();
            StartCoroutine(CrossfadeMixer(insideVolumeParam, outsideVolumeParam));
            Debug.Log("🎶 خروج للصحراء: تدرج احترافي للرياح...");
        }
    }

    // 🌟 المعادلة السحرية (اللوغاريتمية) لانتقال الصوت
    IEnumerator CrossfadeMixer(string fadeOutParam, string fadeInParam)
    {
        float currentTime = 0;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            
            // نحسب النسبة من 0 إلى 1
            float t = currentTime / fadeTime;

            // نحول التدرج الخطي إلى تدرج طبيعي للأذن (من 0.0001 للميوت، إلى 1 لأعلى صوت)
            float fadeInLinear = Mathf.Lerp(0.0001f, 1f, t);
            float fadeOutLinear = Mathf.Lerp(1f, 0.0001f, t);

            // نحول الرقم إلى ديسيبل (dB) ليفهمه الـ Audio Mixer
            float fadeInDb = Mathf.Log10(fadeInLinear) * 20f;
            float fadeOutDb = Mathf.Log10(fadeOutLinear) * 20f;

            // نطبق التعديل
            if (mixer != null)
            {
                mixer.SetFloat(fadeInParam, fadeInDb);
                mixer.SetFloat(fadeOutParam, fadeOutDb);
            }
            
            yield return null;
        }
    }
}