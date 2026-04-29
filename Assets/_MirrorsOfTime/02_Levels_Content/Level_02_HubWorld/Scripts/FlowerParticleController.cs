using UnityEngine;

public class FlowerParticleController : MonoBehaviour
{
    [Header("References")]
    public Terrain activeTerrain; // اسحبي مجسم الـ Terrain هنا
    public ParticleSystem petalParticles; // اسحبي نظام الجزيئات هنا

    [Header("Settings")]
    public int flowerLayerIndex = 0; // رقم طبقة الزهور (0 لو كانت أول زهرة ضفتيها في قائمة الـ Details)
    public float minMoveSpeed = 1f; // أقل سرعة عشان يبدأ يطلع ورق

    private CharacterController controller; // أو Rigidbody لو تستخدمينه للحركة
    private Vector3 lastPosition;

    void Start()
    {
        // نجيب مكون الحركة عشان نتأكد إن نوار تركض مو واقفة
        controller = GetComponent<CharacterController>(); 
        lastPosition = transform.position;

        if (activeTerrain == null)
            activeTerrain = Terrain.activeTerrain;
    }

    void Update()
    {
        // حساب سرعة نوار الحالية
        float currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;
        
        // التحقق: هل نوار تركض؟ وهل هي واقفة على زهور؟
        if (currentSpeed > minMoveSpeed && IsStandingOnFlower())
        {
            if (!petalParticles.isPlaying)
                petalParticles.Play();
        }
        else
        {
            if (petalParticles.isPlaying)
                petalParticles.Stop();
        }
    }

    // دالة للتحقق من وجود زهور تحت نوار
    bool IsStandingOnFlower()
    {
        if (activeTerrain == null) return false;

        TerrainData terrainData = activeTerrain.terrainData;
        Vector3 playerPos = transform.position;
        Vector3 terrainPos = activeTerrain.transform.position;

        // حساب موقع اللاعب بالنسبة للـ Terrain
        float relativeX = playerPos.x - terrainPos.x;
        float relativeZ = playerPos.z - terrainPos.z;

        // تحويل الموقع إلى إحداثيات خريطة الـ Details
        int mapX = Mathf.FloorToInt((relativeX / terrainData.size.x) * terrainData.detailResolution);
        int mapZ = Mathf.FloorToInt((relativeZ / terrainData.size.z) * terrainData.detailResolution);

        // التأكد إننا داخل حدود الـ Terrain
        if (mapX >= 0 && mapX < terrainData.detailResolution && mapZ >= 0 && mapZ < terrainData.detailResolution)
        {
            // الكود الصحيح للمصفوفة ثنائية الأبعاد
            int[,] detailMap = terrainData.GetDetailLayer(mapX, mapZ, 1, 1, flowerLayerIndex);
            return detailMap[0, 0] > 0;
        }

        return false;
    }
}