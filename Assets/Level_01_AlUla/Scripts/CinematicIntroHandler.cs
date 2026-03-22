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

    [Header("السكربت الأساسي (ضروري عشان نوقفه)")]
    [SerializeField] PlayerStateMachine mainPlayerScript; 

    Animator animator;
    CharacterController characterController;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // 1. تنويم السكربت الأساسي
        if (mainPlayerScript != null) 
        {
            mainPlayerScript.enabled = false;
        }

        if (animator != null) 
        {
            animator.SetFloat("Speed", 0f);
            // 🪂 الحل السحري: نجبر الأنيميتور يفهم إنها على الأرض عشان ما تطير
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
                // 🧲 استخدمنا Move مع جاذبية قوية عشان نضمن إنها تلصق بالأرض وما تعلق بالهواء
                Vector3 moveDir = transform.forward * walkSpeed;
                moveDir.y = -9.81f; 
                characterController.Move(moveDir * Time.deltaTime); 
            }
            yield return null; 
        }

        if (animator != null) animator.SetFloat("Speed", 0f); 

        // 2. انتهى المشهد! نرجع التحكم
        if (mainPlayerScript != null) 
        {
            mainPlayerScript.enabled = true;
        }
    }
}