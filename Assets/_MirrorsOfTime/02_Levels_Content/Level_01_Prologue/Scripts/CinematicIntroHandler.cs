using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class CinematicIntroHandler : MonoBehaviour
{
    [Header("توقيت المشي التلقائي")]
    [SerializeField] float timeToStartWalk = 5.5f; 
    [SerializeField] float walkSpeed = 2.0f; 
    [SerializeField] float walkDuration = 4.0f; 

    Animator animator;
    CharacterController characterController;
    private bool isWalkingSequenceActive = false; // للتحكم بالجاذبية الخاصة بهذا السكربت

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        isWalkingSequenceActive = true;

        if (animator != null) 
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsGrounded", true); 
        }

        StartCoroutine(StartWalkSequence());
    }

    // تطبيق الجاذبية فقط خلال فترة تدخل هذا السكربت
    void Update()
    {
        if (isWalkingSequenceActive && characterController != null)
        {
            if (!characterController.isGrounded)
            {
                characterController.Move(new Vector3(0, -9.81f * Time.deltaTime, 0));
            }
        }
    }

    IEnumerator StartWalkSequence()
    {
        // 1. انتظار الوقت المحدد قبل بداية المشي
        yield return new WaitForSeconds(timeToStartWalk);

        // 2. تشغيل أنيميشن المشي
        if (animator != null) animator.SetFloat("Speed", 0.25f); 

        // 3. تحريك اللاعب للأمام
        float walkStartTime = Time.time;
        while (Time.time < walkStartTime + walkDuration)
        {
            if (characterController != null)
            {
                Vector3 moveDir = transform.forward * walkSpeed;
                moveDir.y = -9.81f; // دمج الجاذبية مع الحركة
                characterController.Move(moveDir * Time.deltaTime); 
            }
            yield return null; 
        }

        // 4. إيقاف اللاعب بعد انتهاء وقت المشي
        if (animator != null) animator.SetFloat("Speed", 0f); 
        
        // إيقاف عمل التحديث الخاص بالجاذبية في هذا السكربت ليسلم المهمة لسكربت الحركة الأساسي
        isWalkingSequenceActive = false; 
    }
}