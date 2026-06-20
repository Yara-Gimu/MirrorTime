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

    [Tooltip("الوقت بالثواني لانتظار فتح الشاشة السوداء بالكامل قبل بدء أنيميشن الإفاقة")]
    public float wakeupDelay = 4.5f; 

    [Header("Environment")]
    public GameObject[] floorPieces; 

    [Header("Cinematic Effects")]
    public ParticleSystem heavyDust; 

    [Header("Audio Theatre (إصلاحات الصوت السينمائي)")]
    public AudioSource audioSource; 
    public AudioClip crashSound;     
    public AudioClip collapseSound; // 🌟 صوت الانهيار الصخري البديل لكاميرا الانسداد
    public AudioClip coughSound;    // 🌟 صوت كحة نوار عند الإفاقة لضمان عمله

    private bool hasTriggered = false; 

    public void StartCinematicTeleport()
    {
        if (!hasTriggered)
        {
            hasTriggered = true; 
            StartCoroutine(TransitionSequence());
        }
    }

    IEnumerator TransitionSequence()
    {
        yield return null;

        if (crashSound != null && audioSource != null) audioSource.PlayOneShot(crashSound);
        
        // 🌟 تشغيل صوت الانهيار الصخري القوي فوراً خلف الشاشة السوداء ليسمعه اللاعب بوضوح
        if (collapseSound != null && audioSource != null) audioSource.PlayOneShot(collapseSound);

        foreach (GameObject piece in floorPieces)
        {
            if (piece != null) piece.SetActive(false);
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.position = caveSpawnPoint.position;
        player.rotation = caveSpawnPoint.rotation;

        if (cc != null) cc.enabled = true;

        if (heavyDust != null) heavyDust.Play(); 

        yield return new WaitForSeconds(wakeupDelay);

        // 🌟 تشغيل أنيميشن الإفاقة مع صوت الكحة في نفس اللحظة لمنع أي خلل
        if (playerAnimator != null && !string.IsNullOrEmpty(wakeupTriggerName))
        {
            playerAnimator.SetTrigger(wakeupTriggerName);
            if (coughSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(coughSound);
            }
        }
    }
}