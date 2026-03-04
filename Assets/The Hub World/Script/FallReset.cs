using UnityEngine;

public class FallReset : MonoBehaviour
{
    [Tooltip("اسحبي هنا الاوبجكت الذي يمثل نقطة العودة")]
    public Transform respawnPoint; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            
            if (cc != null)
            {
                // 1. إيقاف المتحكم عشان يسمح لنا ننقله
                cc.enabled = false;

                // 2. النقل
                other.transform.position = respawnPoint.position;
                other.transform.rotation = respawnPoint.rotation;

                // 3. تشغيل المتحكم من جديد
                cc.enabled = true;
            }
        }
    }
}