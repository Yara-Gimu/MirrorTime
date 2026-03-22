using UnityEngine;
using System.Collections;
using Unity.Cinemachine; // 🌟 السر هنا: أضفنا هذي المكتبة عشان يونيتي يفهم الإمبلس

public class FallingTrap : MonoBehaviour
{
    // سوينا كلاس صغير داخلي عشان نرتب الإعدادات في الإنسبكتور لكل صخرة لحالها
    [System.Serializable]
    public class RockPiece
    {
        [Tooltip("المجسم اللي بيطيح")]
        public Transform rockTransform;
        
        [Tooltip("نقطة السقوط (الهدف)")]
        public Transform targetDropPosition;
        
        [Tooltip("كم ثانية يتأخر قبل لا يطيح؟ (الأساسي خليه 0، الباقين 0.3 و 0.6)")]
        public float fallDelay = 0f;

        [Tooltip("مقطع الصوت الخاص بهذي الصخرة لما تضرب الأرض")]
        public AudioClip rockSound; 
        
        // 🌟 التعديل الجديد: خانة عشان نسحب فيها الـ Impulse Source حق الصخرة
        [Tooltip("مكون Cinemachine Impulse Source الموجود على مجسم الصخرة")]
        public CinemachineImpulseSource impulseSource;
    }

    [Header("قائمة الصخور اللي بتطيح")]
    [Tooltip("حددي عدد الصخور واسحبي كل صخرة وهدفها وصوتها وهاذ الامبلس")]
    [SerializeField] RockPiece[] rocks; 

    [Header("إعدادات السقوط السينمائي")]
    [SerializeField] float gravityAcceleration = 15f; 
    
    [Header("--- المؤثرات السينمائية ---")]
    [SerializeField] ParticleSystem impactDust; 
    [SerializeField] AudioSource audioSource; 

    private bool hasFallen = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasFallen)
        {
            hasFallen = true; 
            
            // هذي الحلقة (foreach) تمر على كل الصخور وتخليها تطيح بناءً على وقت التأخير حقها
            foreach (var rock in rocks)
            {
                StartCoroutine(FallToTarget(rock));
            }
        }
    }

    IEnumerator FallToTarget(RockPiece rockPiece)
    {
        // 1. إذا كان فيه تأخير (Delay)، ننتظر قبل ما تبدأ الصخرة تطيح
        if (rockPiece.fallDelay > 0)
        {
            yield return new WaitForSeconds(rockPiece.fallDelay);
        }

        float currentSpeed = 0f;

        // 2. حركة السقوط
        while (Vector3.Distance(rockPiece.rockTransform.position, rockPiece.targetDropPosition.position) > 0.05f)
        {
            currentSpeed += gravityAcceleration * Time.deltaTime;
            rockPiece.rockTransform.position = Vector3.MoveTowards(rockPiece.rockTransform.position, rockPiece.targetDropPosition.position, currentSpeed * Time.deltaTime);
            yield return null; 
        }

        // 3. استقرار الصخرة في الإحداثيات المحددة
        rockPiece.rockTransform.position = rockPiece.targetDropPosition.position;
        
        // 4. 💥 تشغيل المؤثرات والصوت والاهتزاز الخاص بهذي الصخرة بالذات
        // أرسلنا الامبلس سورس للدالة
        PlayImpactEffects(rockPiece.targetDropPosition.position, rockPiece.rockSound, rockPiece.impulseSource);
    }

    // 🌟 حدثنا هذي الدالة لتستقبل الـ ImpulseSource
    private void PlayImpactEffects(Vector3 dropPosition, AudioClip soundToPlay, CinemachineImpulseSource impulseToTrigger)
    {
        // تشغيل الغبار في مكان الصخرة اللي طاحت الحين
        if (impactDust != null)
        {
            impactDust.transform.position = dropPosition;
            impactDust.Play();
        }

        // تشغيل الصوت المخصص للصخرة (إذا كنتِ حاطة لها صوت)
        if (audioSource != null && soundToPlay != null)
        {
            audioSource.PlayOneShot(soundToPlay); 
        }
        
        // 🌟 أهم إضافة: اهتزاز الشاشة الآن!
        if (impulseToTrigger != null)
        {
            // إطلاق الاهتزاز بقوة 1 (يمكنك مضاعفتها هنا)
            impulseToTrigger.GenerateImpulse();
            Debug.Log("💥 الشاشة تهتز الآن عند سقوط الصخرة!");
        }
    }
}