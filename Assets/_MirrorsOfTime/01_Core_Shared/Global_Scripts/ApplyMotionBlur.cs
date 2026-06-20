using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // نفترض أنك تستخدمين URP

public class ApplyMotionBlur : MonoBehaviour
{
    private Volume globalVolume;
    private MotionBlur motionBlur;

    void Start()
    {
        globalVolume = GetComponent<Volume>();

        // نبحث عن تأثير Motion Blur داخل إعدادات الفوليوم
        if (globalVolume != null && globalVolume.profile.TryGet(out motionBlur))
        {
            // نقرأ خيار اللاعب
            int isBlurOn = PlayerPrefs.GetInt("MotionBlur", 1);
            
            // نفعل التأثير أو نلغيه بناءً على الخيار
            motionBlur.active = (isBlurOn == 1);
        }
    }
}