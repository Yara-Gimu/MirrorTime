using UnityEngine;
using UnityEngine.Playables; // عشان نتحكم بالـ Timeline

public class HubDirector : MonoBehaviour
{
    [Header("--- إعدادات المشهد السينمائي ---")]
    public PlayableDirector introTimeline; // اسحبي التايم لاين هنا
    public GameObject playerCharacter; // مجسم شخصية نوار

    void Start()
    {
        // نسأل الـ SaveManager: هل اللاعب شاف المشهد من قبل؟
        if (SaveManager.Instance.hasSeenHubIntro == false)
        {
            // اللاعب جاي من الافتتاحية لأول مرة!
            PlayIntroCutscene();
        }
        else
        {
            // اللاعب رجع يكمل اللعبة، نتخطى المشهد
            SkipIntroCutscene();
        }
    }

    void PlayIntroCutscene()
    {
        // 1. إخفاء الشخصية أو تعطيل التحكم
        playerCharacter.SetActive(false); 
        
        // 2. تشغيل المشهد السينمائي
        introTimeline.Play();

        // 3. تحديث الحفظ عشان ما ينعاد المشهد مستقبلاً
        SaveManager.Instance.hasSeenHubIntro = true;
        SaveManager.Instance.SaveGame();
        
        // (تقدرين تستخدمين الـ EventManager هنا عشان ترجعين التحكم لنوار بعد ما يخلص التايم لاين)
    }

    void SkipIntroCutscene()
    {
        // 1. إيقاف التايم لاين تماماً
        introTimeline.gameObject.SetActive(false); 
        
        // 2. تفعيل شخصية نوار فوراً في الغرفة
        playerCharacter.SetActive(true);
    }
}