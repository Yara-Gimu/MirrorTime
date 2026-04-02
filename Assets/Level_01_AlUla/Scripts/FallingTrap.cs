using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.Audio; // 🌟 ضروري للتحكم في الميكسر

public class FallingTrap : MonoBehaviour
{
    [System.Serializable]
    public class RockPiece
    {
        public Transform rockTransform;
        public Transform targetDropPosition;
        public float fallDelay = 0f;
        public AudioClip rockSound; 
        public CinemachineImpulseSource impulseSource;
    }

    [Header("قائمة الصخور")]
    [SerializeField] RockPiece[] rocks; 

    [Header("إعدادات السقوط")]
    [SerializeField] float gravityAcceleration = 15f; 
    
    [Header("--- المؤثرات الصوتية والميكسر ---")]
    [SerializeField] ParticleSystem impactDust; 
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioMixer mixer; // 🌟 اسحبي الميكسر هنا
    
    // 🌟 تم تعديل القيمة الافتراضية هنا لتطابق اسم الميكسر اللي سويناه
    [SerializeField] string musicVolumeParam = "InsideVol"; 

    private bool hasFallen = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasFallen)
        {
            hasFallen = true; 
            
            Animator playerAnim = other.GetComponent<Animator>();
            if (playerAnim != null)
            {
                StartCoroutine(DelayedCough(playerAnim, 0.7f));
            }

            // 🌟 خفض الموسيقى بنعومة (AAA Ducking)
            StartCoroutine(AudioDuckingSequence());

            foreach (var rock in rocks)
            {
                StartCoroutine(FallToTarget(rock));
            }
        }
    }

    // 🌟 نظام Ducking احترافي (AAA) ذو تدرج ناعم
    IEnumerator AudioDuckingSequence()
    {
        float fadeOutTime = 0.2f; // 1. نزول سريع جداً مع الضربة
        float holdTime = 2.5f;    // 2. بقاء الصوت منخفض أثناء غبار الصخور
        float fadeInTime = 2.0f;  // 3. رجوع هادئ للموسيقى
        
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

    IEnumerator DelayedCough(Animator anim, float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetTrigger("Cough");
    }

    IEnumerator FallToTarget(RockPiece rockPiece)
    {
        if (rockPiece.fallDelay > 0) yield return new WaitForSeconds(rockPiece.fallDelay);

        float currentSpeed = 0f;
        while (Vector3.Distance(rockPiece.rockTransform.position, rockPiece.targetDropPosition.position) > 0.05f)
        {
            currentSpeed += gravityAcceleration * Time.deltaTime;
            rockPiece.rockTransform.position = Vector3.MoveTowards(rockPiece.rockTransform.position, rockPiece.targetDropPosition.position, currentSpeed * Time.deltaTime);
            yield return null; 
        }

        rockPiece.rockTransform.position = rockPiece.targetDropPosition.position;
        PlayImpactEffects(rockPiece.targetDropPosition.position, rockPiece.rockSound, rockPiece.impulseSource);
    }

    private void PlayImpactEffects(Vector3 dropPosition, AudioClip soundToPlay, CinemachineImpulseSource impulseToTrigger)
    {
        if (impactDust != null && !impactDust.isPlaying) impactDust.Play();

        if (audioSource != null && soundToPlay != null)
            audioSource.PlayOneShot(soundToPlay); 
        
        if (impulseToTrigger != null)
            impulseToTrigger.GenerateImpulseWithForce(1f); 
    }
}