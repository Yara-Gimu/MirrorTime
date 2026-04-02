using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class CinematicIntroHandler : MonoBehaviour
{
    [Header("توقيت الكاميرات السينمائية (ثواني)")]
    [SerializeField] float timeToStartWalk = 5.5f; 
    
    [Header("إعدادات المشي التلقائي")]
    [SerializeField] float walkSpeed = 2.0f; 
    [SerializeField] float walkDuration = 4.0f; 

    [Header("السكربتات اللي نبغى نقفلها وقت المشهد")]
    [Tooltip("اسحبي هنا PlayerStateMachine، وأي سكربت مسؤول عن الماوس أو لف الكاميرا")]
    [SerializeField] Behaviour[] scriptsToLock; // 🌟 السر هنا لتقفيل كل شيء

    Animator animator;
    CharacterController characterController;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // 1. تنويم كل السكربتات المسؤولة عن التحكم
        LockPlayerControls(true);

        if (animator != null) 
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsGrounded", true); 
        }

        StartCoroutine(StartCinematicSequence());
    }

    IEnumerator StartCinematicSequence()
    {
        yield return new WaitForSeconds(timeToStartWalk);

        if (animator != null) animator.SetFloat("Speed", 0.25f); 

        float startTime = Time.time;
        while (Time.time < startTime + walkDuration)
        {
            if (characterController != null)
            {
                Vector3 moveDir = transform.forward * walkSpeed;
                moveDir.y = -9.81f; 
                characterController.Move(moveDir * Time.deltaTime); 
            }
            yield return null; 
        }

        if (animator != null) animator.SetFloat("Speed", 0f); 

        // 2. انتهى المشهد! نرجع نفتح كل السكربتات ونرجع التحكم
        LockPlayerControls(false);
    }

    // 🌟 دالة مساعدة تقفل وتفتح السكربتات بضغطة زر
    void LockPlayerControls(bool isLocked)
    {
        foreach (var script in scriptsToLock)
        {
            if (script != null)
            {
                script.enabled = !isLocked; 
            }
        }
    }
}