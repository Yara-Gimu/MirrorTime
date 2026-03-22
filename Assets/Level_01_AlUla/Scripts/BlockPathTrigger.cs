using UnityEngine;

public class BlockPathTrigger : MonoBehaviour
{
    [Header("الجدار المخفي اللي بيقفل الطريق")]
    public GameObject invisibleWall;

    private void OnTriggerEnter(Collider other)
    {
        // إذا نوار عدت من هنا
        if (other.CompareTag("Player"))
        {
            // فعل الجدار وراها
            if (invisibleWall != null)
            {
                invisibleWall.SetActive(true);
            }

            // احذف هذا الزناد عشان ما يشتغل الكود مليون مرة ويستهلك أداء
            Destroy(gameObject);
        }
    }
}