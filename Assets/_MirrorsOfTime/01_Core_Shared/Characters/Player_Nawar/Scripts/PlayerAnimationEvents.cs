using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [Header("إعدادات الصوت")]
    public AudioSource playerAudioSource;
    public AudioClip coughSound;

    // 🌟 هذه الدالة سيقوم الأنيميشن باستدعائها بنفسه في اللحظة الحاسمة!
    public void PlayCoughEvent()
    {
        if (playerAudioSource != null && coughSound != null)
        {
            playerAudioSource.PlayOneShot(coughSound);
        }
    }
}