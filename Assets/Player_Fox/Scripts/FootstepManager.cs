using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepArchitecture : MonoBehaviour
{
    [System.Serializable]
    public class SurfaceAudio
    {
        public AudioClip[] walkSounds;
        public AudioClip[] runSounds;
        public AudioClip[] jumpTakeoffSounds; 
        public AudioClip[] jumpLandSounds; 
        public AudioClip[] crawlSounds; 
        
        [Header("--- مؤثرات الغبار (VFX) ---")]
        [Tooltip("اسحبي مجسم الغبار الخاص بهذي الأرضية هنا")]
        public ParticleSystem dustVFXPrefab; 
    }

    [Header("--- أصوات الأسطح ---")]
    public SurfaceAudio sand;
    public SurfaceAudio stone;
    public SurfaceAudio wood;

    [Header("--- إعدادات الاستشعار (Raycast) ---")]
    public float rayDistance = 1.5f; 

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f; 
        audioSource.volume = 0.4f; 
    }

    public void PlayFootstepEvent(string action)
    {
        Vector3 rayStart = transform.position + (Vector3.up * 0.1f);

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayDistance))
        {
            string surfaceTag = hit.collider.tag;
            SurfaceAudio currentSurface;

            switch (surfaceTag)
            {
                case "Stone": currentSurface = stone; break;
                case "Wood":  currentSurface = wood; break;
                case "Sand": 
                default:      currentSurface = sand; break;
            }

            // ==========================================
            // 1. نظام تشغيل الصوت (كودك الأصلي الممتاز)
            // ==========================================
            AudioClip[] selectedSounds = null;
            switch (action)
            {
                case "Walk": selectedSounds = currentSurface.walkSounds; break;
                case "Run":  selectedSounds = currentSurface.runSounds; break;
                case "JumpTakeoff": selectedSounds = currentSurface.jumpTakeoffSounds; break; 
                case "JumpLand": selectedSounds = currentSurface.jumpLandSounds; break;
                case "Crawl": selectedSounds = currentSurface.crawlSounds; break; 
            }

            if (selectedSounds != null && selectedSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, selectedSounds.Length);
                
                audioSource.pitch = (action == "Crawl") ? Random.Range(0.8f, 0.95f) : Random.Range(0.9f, 1.1f);
                
                float volumeModifier = 1f;
                if (action == "JumpLand") volumeModifier = 1f;
                else if (action == "JumpTakeoff") volumeModifier = 0.7f;
                else if (action == "Crawl") volumeModifier = 0.25f; 
                else volumeModifier = Random.Range(0.7f, 0.9f);
                
                audioSource.PlayOneShot(selectedSounds[randomIndex], volumeModifier);
            }

            // ==========================================
            // 2. نظام تشغيل الغبار (VFX) - AAA Style
            // ==========================================
            if (currentSurface.dustVFXPrefab != null)
            {
                // نحدد نسبة ظهور الغبار حسب الحركة
                float spawnChance = 0f;
                if (action == "Run" || action == "JumpLand") spawnChance = 1.0f; // 100% مع الركض والقفز
                else if (action == "Walk") spawnChance = 0.4f; // 40% مع المشي
                else if (action == "Crawl") spawnChance = 0.1f; // 10% مع الزحف
                
                // إذا رمينا النرد وطلع الرقم ضمن النسبة المسموحة، نطلع الغبار!
                if (Random.value <= spawnChance)
                {
                    // ننسخ الغبار في نقطة الاصطدام بالضبط، ونخليه يطالع لفوق حسب ميلان الأرض
                    ParticleSystem spawnedDust = Instantiate(currentSurface.dustVFXPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    
                    // نمسح مجسم الغبار من اللعبة بعد ثانيتين عشان ما نستهلك الذاكرة
                    Destroy(spawnedDust.gameObject, 2f); 
                }
            }
        }
    }
}