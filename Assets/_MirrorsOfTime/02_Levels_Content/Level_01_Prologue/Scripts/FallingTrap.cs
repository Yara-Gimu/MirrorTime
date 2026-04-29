using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.Audio;

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
    
    [Header("--- المؤثرات الصوتية (البيئة والصخور) ---")]
    [SerializeField] ParticleSystem impactDust; 
    [Tooltip("هذي السماعة للصخور (خليها 3D ومكانها عند السقف)")]
    [SerializeField] AudioSource environmentAudioSource; 
    
    [Header("--- المؤثرات الصوتية (السينمائية والكحة) ---")]
    [Tooltip("هذي السماعة للكحة (خليها 2D عشان تنسمع بوضوح)")]
    [SerializeField] AudioSource cinematicAudioSource; 
    [SerializeField] AudioClip coughSound; 
    [Tooltip("كم ثانية تنتظر نورة بعد الكحة الأولى عشان تكح المرة الثانية؟")]
    [SerializeField] float timeBetweenCoughs = 2.8f;

    [Header("--- الميكسر ---")]
    [SerializeField] AudioMixer mixer; 
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

            StartCoroutine(AudioDuckingSequence());

            foreach (var rock in rocks)
            {
                StartCoroutine(FallToTarget(rock));
            }
        }
    }

    IEnumerator AudioDuckingSequence()
    {
        float fadeOutTime = 0.2f; 
        float holdTime = 2.5f;    
        float fadeInTime = 2.0f;  
        
        float targetVolume = -20f;
        float originalVolume = 0f; 
        float timer = 0f;

        while(timer < fadeOutTime) 
        {
            timer += Time.deltaTime;
            if (mixer != null) mixer.SetFloat(musicVolumeParam, Mathf.Lerp(originalVolume, targetVolume, timer / fadeOutTime));
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

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

        // 🌟 نستخدم السماعة السينمائية للكحة!
        if (cinematicAudioSource != null && coughSound != null)
        {
            cinematicAudioSource.PlayOneShot(coughSound); 
            StartCoroutine(PlaySecondCough(timeBetweenCoughs)); 
        }
    }

    IEnumerator PlaySecondCough(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 🌟 نستخدم السماعة السينمائية للكحة الثانية!
        if (cinematicAudioSource != null && coughSound != null)
        {
            cinematicAudioSource.PlayOneShot(coughSound);
        }
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

        // 🌟 نستخدم سماعة البيئة لصوت الصخور!
        if (environmentAudioSource != null && soundToPlay != null)
            environmentAudioSource.PlayOneShot(soundToPlay); 
        
        if (impulseToTrigger != null)
            impulseToTrigger.GenerateImpulseWithForce(1f); 
    }
}