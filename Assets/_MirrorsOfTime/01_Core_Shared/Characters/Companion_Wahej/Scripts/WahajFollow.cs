using UnityEngine;

public class WahajFollow : MonoBehaviour
{
    [Header("Target Setup")]
    public Transform player; // هنا نسحب نوار
    public Vector3 offset = new Vector3(1f, 1.5f, -1f); // مكان وهج (يمين، فوق، ورا شوي)

    [Header("Float Settings")]
    public float followSpeed = 4f; // سرعة اللحاق بنوار
    public float floatSpeed = 2f; // سرعة الطفو فوق وتحت
    public float floatHeight = 0.2f; // مسافة الطفو

    void Update()
    {
        if (player == null) return;

        // 1. حساب المكان المطلوب بناءً على مكان نوار والـ Offset
        Vector3 targetPosition = player.position + player.TransformDirection(offset);

        // 2. إضافة حركة الطفو السحرية (فوق وتحت)
        targetPosition.y += Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // 3. التحرك بنعومة سينمائية للمكان المطلوب
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // 4. الدوران عشان وهج تطالع نفس الاتجاه اللي تطالعه نوار
        transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, Time.deltaTime * followSpeed);
    }
}