using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class CinematicIntroHandler : MonoBehaviour
{
    [Header("توقيت المشهد السينمائي")]
    [SerializeField] float totalSceneDuration = 39f; 
    
    [Header("توقيت المشي التلقائي")]
    [SerializeField] float timeToStartWalk = 5.5f; 
    [SerializeField] float walkSpeed = 2.0f; 
    [SerializeField] float walkDuration = 4.0f; 

    [Header("السكربتات اللي نبغى نقفلها وقت المشهد")]
    [SerializeField] Behaviour[] scriptsToLock; 

    Animator animator;
    CharacterController characterController;
    private bool isCinematicActive = false; // 🌟 لمعرفة إذا المشهد شغال

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // 1. قفل التحكم فوراً عند بداية اللعبة
        LockPlayerControls(true);
        isCinematicActive = true;

        if (animator != null) 
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsGrounded", true); 
        }

        StartCoroutine(StartCinematicSequence());
    }

    // 🌟 السر هنا: جاذبية مستمرة طول المشهد عشان نوار ما تسبح في الهواء!
    void Update()
    {
        if (isCinematicActive && characterController != null)
        {
            if (!characterController.isGrounded)
            {
                characterController.Move(new Vector3(0, -9.81f * Time.deltaTime, 0));
            }
        }
    }

    IEnumerator StartCinematicSequence()
    {
        float startTime = Time.time;

        yield return new WaitForSeconds(timeToStartWalk);

        if (animator != null) animator.SetFloat("Speed", 0.25f); 

        float walkStartTime = Time.time;
        while (Time.time < walkStartTime + walkDuration)
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

        // ننتظر باقي الـ 39 ثانية
        float timeRemaining = totalSceneDuration - (Time.time - startTime);
        if (timeRemaining > 0)
        {
            yield return new WaitForSeconds(timeRemaining);
        }

        // 2. انتهى المشهد الكلي! نفتح التحكم
        isCinematicActive = false;
        LockPlayerControls(false);
    }

    void LockPlayerControls(bool isLocked)
    {
        foreach (var script in scriptsToLock)
        {
            if (script != null) script.enabled = !isLocked; 
        }
    }
}