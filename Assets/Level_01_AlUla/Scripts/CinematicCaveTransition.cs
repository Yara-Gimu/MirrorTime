using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; 

public class CinematicCaveTransition : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player; 
    public Transform caveSpawnPoint; 
    public Animator playerAnimator; 
    
    [Tooltip("اسم التريقر في الأنميتر لحركة الإفاقة (مثل: Cough)")]
    public string wakeupTriggerName = "Cough"; 

    [Header("Environment")]
    [Tooltip("المربعات أو الصخور اللي بتختفي عشان تفتح السقف")]
    public GameObject[] floorPieces; 

    [Header("Cinematic UI & Effects")]
    public CanvasGroup blackScreen; 
    public ParticleSystem heavyDust; 

    [Header("Audio Theatre & Mixer")]
    public AudioSource audioSource; 
    public AudioClip rumbleSound;   
    public AudioClip crashSound;    
    public AudioClip coughSound;    
    
    public AudioMixer mixer; 
    public string musicVolumeParam = "InsideVol"; 

    private bool hasTriggered = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; 
            StartCoroutine(CinematicSequence());
        }
    }

    IEnumerator CinematicSequence()
    {
        // ==========================================
        // 1. لحظة الزلزال والإدراك
        // ==========================================
        if (rumbleSound != null) audioSource.PlayOneShot(rumbleSound);
        yield return new WaitForSeconds(2f); 

        // ==========================================
        // 2. القطع للسواد (Smash Cut) والانهيار
        // ==========================================
        // إظلام الشاشة فوراً
        if (blackScreen != null) blackScreen.alpha = 1f; 
        
        // تشغيل صوت الحطام
        if (crashSound != null) audioSource.PlayOneShot(crashSound);

        // تشغيل نظام خفض الموسيقى (Audio Ducking)
        StartCoroutine(AudioDuckingSequence());

        // إخفاء الأرضيات فوراً عشان تنفتح الفتحة فوق في السقف
        foreach (GameObject piece in floorPieces)
        {
            if (piece != null) piece.SetActive(false);
        }

        // ==========================================
        // 3. النقل (الترسبن) بالخفاء وراء الكواليس
        // ==========================================
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = caveSpawnPoint.position;
        player.rotation = caveSpawnPoint.rotation;

        if (cc != null) cc.enabled = true;

        // انتظار لسماع صدى الحطام في الظلام التام
        yield return new WaitForSeconds(3f); 

        // ==========================================
        // 4. الإفاقة (الفتح التدريجي والأنيميشن)
        // ==========================================
        
        // تشغيل الغبار الآن ليتزامن مع فتح الشاشة
        if (heavyDust != null) heavyDust.Play(); 

        // تشغيل حركة الكحة أو الإفاقة باستخدام التريقر
        if (playerAnimator != null && !string.IsNullOrEmpty(wakeupTriggerName))
        {
            playerAnimator.SetTrigger(wakeupTriggerName);
        }

        // صوت الكحة
        if (coughSound != null) audioSource.PlayOneShot(coughSound);
        
        // تلاشي الشاشة السوداء ببطء
        float fadeDuration = 4f; 
        float timer = 0;
        
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (blackScreen != null) blackScreen.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
    }

    // ==========================================
    // نظام خفض الموسيقى (Audio Ducking)
    // ==========================================
    IEnumerator AudioDuckingSequence()
    {
        float fadeOutTime = 0.2f; // نزول سريع جداً مع الضربة
        float holdTime = 4.5f;    // البقاء منخفضاً طوال فترة السواد
        float fadeInTime = 3.0f;  // رجوع هادئ للموسيقى مع فتح الشاشة

        float targetVolume = -20f;
        float originalVolume = 0f; 
        float timer = 0f;

        // 1. خفض سريع للصوت (Fade Out)
        while(timer < fadeOutTime) 
        {
            timer += Time.deltaTime;
            if (mixer != null) mixer.SetFloat(musicVolumeParam, Mathf.Lerp(originalVolume, targetVolume, timer / fadeOutTime));
            yield return null;
        }

        // 2. البقاء على الصوت المنخفض (Hold)
        yield return new WaitForSeconds(holdTime);

        // 3. عودة الصوت الطبيعي بنعومة (Fade In)
        timer = 0f;
        while(timer < fadeInTime) 
        {
            timer += Time.deltaTime;
            if (mixer != null) mixer.SetFloat(musicVolumeParam, Mathf.Lerp(targetVolume, originalVolume, timer / fadeInTime));
            yield return null;
        }
    }
}