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
        
        [Header("--- مؤثرات الغبار/التطاير (VFX) ---")]
        public ParticleSystem dustVFXPrefab; 
    }

    [Header("--- أصوات الأسطح ---")]
    public SurfaceAudio sand;
    public SurfaceAudio stone;
    public SurfaceAudio wood;
    public SurfaceAudio grass; 

    [Header("--- إعدادات الاستشعار (SphereCast) ---")]
    public float rayDistance = 0.5f; 
    [Tooltip("نصف قطر كرة الاستشعار لضمان دقة اصطدام القدم")]
    public float sphereRadius = 0.3f; 

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f; 
        audioSource.volume = 0.4f; 
    }

    public void PlayFootstepEvent(string action)
    {
        Vector3 rayStart = transform.position + (Vector3.up * 0.5f); // رفعنا نقطة البداية قليلاً

        // 🌟 الإصلاح السحري: استخدام SphereCast بدلاً من Raycast لضمان عدم تفويت الحواف أو الدرج
        if (Physics.SphereCast(rayStart, sphereRadius, Vector3.down, out RaycastHit hit, rayDistance))
        {
            string surfaceTag = hit.collider.tag;
            SurfaceAudio currentSurface;

            switch (surfaceTag)
            {
                case "Stone": currentSurface = stone; break;
                case "Wood":  currentSurface = wood; break;
                case "Grass": currentSurface = grass; break; 
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

            if (currentSurface.dustVFXPrefab != null)
            {
                float spawnChance = 0f;
                if (action == "Run" || action == "JumpLand") spawnChance = 1.0f; 
                else if (action == "Walk") spawnChance = 0.4f; 
                else if (action == "Crawl") spawnChance = 0.1f; 
                
                if (Random.value <= spawnChance)
                {
                    ParticleSystem spawnedDust = Instantiate(currentSurface.dustVFXPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(spawnedDust.gameObject, 2f); 
                }
            }
        }
    }
}