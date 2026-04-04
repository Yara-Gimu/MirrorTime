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
        public AudioClip[] crawlSounds; // 🌟 أضفنا فئة الزحف هنا
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

    // action parameter takes: "Walk", "Run", "JumpTakeoff", "JumpLand", or "Crawl"
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

            AudioClip[] selectedSounds = null;
            switch (action)
            {
                case "Walk": selectedSounds = currentSurface.walkSounds; break;
                case "Run":  selectedSounds = currentSurface.runSounds; break;
                case "JumpTakeoff": selectedSounds = currentSurface.jumpTakeoffSounds; break; 
                case "JumpLand": selectedSounds = currentSurface.jumpLandSounds; break;
                case "Crawl": selectedSounds = currentSurface.crawlSounds; break; // 🌟 ربطنا الزحف
            }

            if (selectedSounds != null && selectedSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, selectedSounds.Length);
                
                // 🌟 إذا كان زحف، ننزل الـ Pitch شوي عشان يعطي إحساس السحب والكشط
                audioSource.pitch = (action == "Crawl") ? Random.Range(0.8f, 0.95f) : Random.Range(0.9f, 1.1f);
                
                // 🌟 وزن قوة الصوت
                float volumeModifier = 1f;
                if (action == "JumpLand") volumeModifier = 1f;
                else if (action == "JumpTakeoff") volumeModifier = 0.7f;
                else if (action == "Crawl") volumeModifier = 0.25f; // الزحف صوته خافت جداً
                else volumeModifier = Random.Range(0.7f, 0.9f);
                
                audioSource.PlayOneShot(selectedSounds[randomIndex], volumeModifier);
            }
        }
    }
}