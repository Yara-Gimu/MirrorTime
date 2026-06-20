using UnityEngine;

public class FlowerParticleController : MonoBehaviour
{
    [Header("References")]
    public Terrain activeTerrain; 
    public ParticleSystem petalParticles; 

    [Header("Settings")]
    public int flowerLayerIndex = 0; 
    public float minMoveSpeed = 1f; 
    
    [Tooltip("كم مرة في الثانية نفحص الأرض؟ (0.1 يعني 10 مرات بدل 60) لتوفير الأداء")]
    public float checkInterval = 0.1f; 

    private CharacterController controller; 
    private Vector3 lastPosition;
    private float timer = 0f;
    private bool isCurrentlyOnFlower = false;

    void Start()
    {
        controller = GetComponent<CharacterController>(); 
        lastPosition = transform.position;

        if (activeTerrain == null)
            activeTerrain = Terrain.activeTerrain;
    }

    void Update()
    {
        float currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;
        
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            isCurrentlyOnFlower = IsStandingOnFlower();
            timer = 0f;
        }
        
        if (currentSpeed > minMoveSpeed && isCurrentlyOnFlower)
        {
            if (!petalParticles.isPlaying) petalParticles.Play();
        }
        else
        {
            if (petalParticles.isPlaying) petalParticles.Stop();
        }
    }

    bool IsStandingOnFlower()
    {
        // 🌟 حماية 1: التأكد من وجود الأرضية
        if (activeTerrain == null || activeTerrain.terrainData == null) return false;

        TerrainData terrainData = activeTerrain.terrainData;
        
        // 🌟 حماية 2: التأكد أن خريطة الزهور موجودة فعلاً لمنع خطأ OutOfBounds Crash
        if (flowerLayerIndex >= terrainData.detailPrototypes.Length) return false;

        Vector3 playerPos = transform.position;
        Vector3 terrainPos = activeTerrain.transform.position;

        float relativeX = playerPos.x - terrainPos.x;
        float relativeZ = playerPos.z - terrainPos.z;

        int mapX = Mathf.FloorToInt((relativeX / terrainData.size.x) * terrainData.detailResolution);
        int mapZ = Mathf.FloorToInt((relativeZ / terrainData.size.z) * terrainData.detailResolution);

        if (mapX >= 0 && mapX < terrainData.detailResolution && mapZ >= 0 && mapZ < terrainData.detailResolution)
        {
            int[,] detailMap = terrainData.GetDetailLayer(mapX, mapZ, 1, 1, flowerLayerIndex);
            return detailMap[0, 0] > 0;
        }

        return false;
    }
}