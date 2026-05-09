using System.Collections;
using UnityEngine;

public class CinematicCaveTransition : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player; 
    public Transform caveSpawnPoint; 
    public Animator playerAnimator; 
    
    [Tooltip("اسم التريقر في الأنميتر لحركة الإفاقة (مثل: Cough)")]
    public string wakeupTriggerName = "Cough"; 

    [Header("Environment")]
    public GameObject[] floorPieces; 

    [Header("Cinematic Timing (السر هنا)")]
    [Tooltip("كم ثانية يستمر مشهد الزلزال في التايم لاين قبل ما يتم نقل اللاعب للكهف؟")]
    public float timeBeforeTeleport = 7f; // رجعنا هذا الرقم عشان نوزن الكود مع الكاميرا

    [Header("Cinematic Effects")]
    public ParticleSystem heavyDust; 

    [Header("Audio Theatre")]
    public AudioSource audioSource; 
    public AudioClip crashSound;    
    public AudioClip coughSound;    
    public float timeBetweenCoughs = 2.8f; 

    private bool hasTriggered = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; 
            StartCoroutine(TransitionSequence());
        }
    }

    IEnumerator TransitionSequence()
    {
        // 1. انتظار التايم لاين: نعطي الكاميرات الأولى وقتها تخلص مشهدها (الزلزال مثلاً)
        yield return new WaitForSeconds(timeBeforeTeleport);

        // 2. الآن (بعد ما انتهى الوقت) الشاشة أكيد صارت سوداء من التايم لاين، ننقل اللاعب بسلام!
        if (crashSound != null) audioSource.PlayOneShot(crashSound);

        foreach (GameObject piece in floorPieces)
        {
            if (piece != null) piece.SetActive(false);
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = caveSpawnPoint.position;
        player.rotation = caveSpawnPoint.rotation;

        if (cc != null) cc.enabled = true;

        // 3. تشغيل الغبار والأنيميشن والأصوات في مكانها الصحيح داخل الكهف
        if (heavyDust != null) heavyDust.Play(); 

        if (playerAnimator != null && !string.IsNullOrEmpty(wakeupTriggerName))
        {
            playerAnimator.SetTrigger(wakeupTriggerName);
        }

        if (coughSound != null) 
        {
            audioSource.PlayOneShot(coughSound); 
            StartCoroutine(PlaySecondCough(timeBetweenCoughs)); 
        }
    }

    IEnumerator PlaySecondCough(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && coughSound != null)
        {
            audioSource.PlayOneShot(coughSound);
        }
    }
}