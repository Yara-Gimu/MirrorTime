using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Audio Theatre")]
    public AudioSource audioSource; 
    public AudioClip crashSound;    
    public AudioClip coughSound;    
    
    // 🌟 المتغير للتحكم بوقت الكحة الثانية
    [Tooltip("كم ثانية تنتظر نورة بعد الكحة الأولى عشان تكح المرة الثانية؟")]
    public float timeBetweenCoughs = 2.8f; 

    private bool hasTriggered = false; 

    private void OnTriggerEnter(Collider other)
    {
        // هذا السطر بيعلمك في الكونسول مين اللي لمس التريجر بالضبط
        Debug.Log("شيء لمسني واسمه: " + other.name + " وعنده تاغ: " + other.tag);
        // التأكد أن اللاعب هو من دخل المنطقة ولم يتم تفعيل المشهد من قبل
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; 
            StartCoroutine(CinematicSequence());
        }
    }

    IEnumerator CinematicSequence()
    {
        // ==========================================
        // 1. لحظة الزلزال والإدراك (تزامن مع التايم لاين)
        // ==========================================
        
        // ننتظر 3.5 ثانية (مدة اهتزاز الكاميرا في الـ Timeline + نص ثانية أمان)
        yield return new WaitForSeconds(7f); 

        // ==========================================
        // 2. القطع للسواد (Smash Cut) والانهيار
        // ==========================================
        // إظلام الشاشة فوراً (Alpha = 1) لإخفاء عملية النقل
        if (blackScreen != null) blackScreen.alpha = 1f; 
        
        // تشغيل صوت الانهيار القوي
        if (crashSound != null) audioSource.PlayOneShot(crashSound);

        // إخفاء الأرضيات/الصخور لفتح الفتحة في السقف
        foreach (GameObject piece in floorPieces)
        {
            if (piece != null) piece.SetActive(false);
        }

        // ==========================================
        // 3. النقل (الترسبن) بالخفاء وراء الكواليس
        // ==========================================
        CharacterController cc = player.GetComponent<CharacterController>();
        
        // تعطيل الـ CharacterController مؤقتاً للسماح بنقل الموقع
        if (cc != null) cc.enabled = false;

        player.position = caveSpawnPoint.position;
        player.rotation = caveSpawnPoint.rotation;

        if (cc != null) cc.enabled = true;

        // انتظار في الظلام التام لسماع صدى الحطام وإعطاء جو درامي
        yield return new WaitForSeconds(3f); 

        // ==========================================
        // 4. الإفاقة (الفتح التدريجي والأنيميشن)
        // ==========================================
        
        // تشغيل تأثير الغبار ليتزامن مع فتح العين/الشاشة
        if (heavyDust != null) heavyDust.Play(); 

        // تفعيل أنيميشن الكحة أو الإفاقة
        if (playerAnimator != null && !string.IsNullOrEmpty(wakeupTriggerName))
        {
            playerAnimator.SetTrigger(wakeupTriggerName);
        }

        // تشغيل صوت الكحة المزدوجة
        if (coughSound != null) 
        {
            audioSource.PlayOneShot(coughSound); // الكحة الأولى
            StartCoroutine(PlaySecondCough(timeBetweenCoughs)); // الكحة الثانية بعد الانتظار
        }
        
        // تلاشي الشاشة السوداء ببطء (Fade Out) لإظهار الكهف الجديد
        float fadeDuration = 4f; 
        float timer = 0;
        
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (blackScreen != null) 
                blackScreen.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
    }

    // ==========================================
    // دالة لتشغيل الكحة الثانية بناءً على الوقت المحدد
    // ==========================================
    IEnumerator PlaySecondCough(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && coughSound != null)
        {
            audioSource.PlayOneShot(coughSound);
        }
    }
}