using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class FootstepManager : MonoBehaviour
{
    [Header("--- أصوات الأسطح المختلفة (AAA) ---")]
    [Tooltip("أصوات المشي على الصحراء/الرمل")]
    public AudioClip[] sandSounds;
    
    [Tooltip("أصوات المشي داخل الكهف/الصخر")]
    public AudioClip[] stoneSounds;
    
    [Tooltip("أصوات المشي على الخشب/الجسور أو السلالم")]
    public AudioClip[] woodSounds;

    [Header("--- إعدادات التوقيت ---")]
    public float walkStepInterval = 0.5f; 
    public float runStepInterval = 0.3f;
    public float runSpeedThreshold = 4f; 

    [Header("--- إعدادات الاستشعار (Raycast) ---")]
    [Tooltip("طول الليزر اللي بيقيس الأرض، خليه أطول من نوار بشوي")]
    public float rayDistance = 1.5f; 

    private AudioSource audioSource;
    private CharacterController controller;
    private float stepTimer = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        audioSource.spatialBlend = 1f; 
        audioSource.volume = 0.6f;
    }

    void Update()
    {
        // إذا نوار تتحرك وتلمس الأرض
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime;
            float currentInterval = (controller.velocity.magnitude > runSpeedThreshold) ? runStepInterval : walkStepInterval;

            if (stepTimer >= currentInterval)
            {
                // 🌟 ننادي الدالة الذكية بدال الدالة العادية
                PlaySurfaceFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f; 
        }
    }

    // 🌟 الدالة الذكية اللي تستشعر نوع الأرض
    private void PlaySurfaceFootstep()
    {
        // 1. نطلق ليزر من منتصف نوار للأسفل
        Vector3 rayStart = transform.position;

        // 2. إذا الليزر ضرب الأرض (Collider)
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayDistance))
        {
            // 3. نقرأ التاج (Tag) حق الأرض اللي ضربناها
            string surfaceTag = hit.collider.tag;
            AudioClip[] selectedSounds = null;

            // 4. نختار مجموعة الأصوات حسب التاج
            switch (surfaceTag)
            {
                case "Stone":
                    selectedSounds = stoneSounds;
                    break;
                case "Wood":
                    selectedSounds = woodSounds;
                    break;
                case "Sand":
                default: // إذا مافي تاج معين، نعتبره رمل كافتراضي
                    selectedSounds = sandSounds;
                    break;
            }

            // 5. نشغل صوت عشوائي من المجموعة اللي اخترناها
            if (selectedSounds != null && selectedSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, selectedSounds.Length);
                audioSource.pitch = Random.Range(0.85f, 1.15f); // تغيير Pitch لواقعية أكثر
                audioSource.PlayOneShot(selectedSounds[randomIndex]);
            }
        }
    }

    // 🌟 دالة مساعدة عشان نرسم الليزر في اليونيتي وتشوفينه بعينك للتأكد
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);
    }
}