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
    public float fadeTime = 4f;

    private bool isPlayerInside = false;
    
    // 🌟 المتغير المعماري لحفظ وتتبع عملية التلاشي الحالية
    private Coroutine currentFadeCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            isPlayerInside = true;
            
            // 🌟 نوقف الكوروتين الخاص بالصوت فقط!
            if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
            
            currentFadeCoroutine = StartCoroutine(CrossfadeMixer(outsideVolumeParam, insideVolumeParam));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerInside)
        {
            isPlayerInside = false;
            
            if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
            
            currentFadeCoroutine = StartCoroutine(CrossfadeMixer(insideVolumeParam, outsideVolumeParam));
        }
    }

    IEnumerator CrossfadeMixer(string fadeOutParam, string fadeInParam)
    {
        float currentTime = 0;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / fadeTime;

            float fadeInLinear = Mathf.Lerp(0.0001f, 1f, t);
            float fadeOutLinear = Mathf.Lerp(1f, 0.0001f, t);

            float fadeInDb = Mathf.Log10(fadeInLinear) * 20f;
            float fadeOutDb = Mathf.Log10(fadeOutLinear) * 20f;

            if (mixer != null)
            {
                mixer.SetFloat(fadeInParam, fadeInDb);
                mixer.SetFloat(fadeOutParam, fadeOutDb);
            }
            yield return null;
        }
    }
}