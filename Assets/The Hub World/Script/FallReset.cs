using UnityEngine;

public class FallReset : MonoBehaviour
{
    [Tooltip("اسحبي هنا الاوبجكت الذي يمثل نقطة العودة")]
    public Transform respawnPoint; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 🏗️ الترقية المعمارية: إرسال إشارة لمدير البيانات لتسجيل مكان السقوط (Heatmap)
            // EventManager.Trigger("Telemetry_Player_Fell", other.transform.position);

            CharacterController cc = other.GetComponent<CharacterController>();
            
            if (cc != null)
            {
                cc.enabled = false;

                other.transform.position = respawnPoint.position;
                other.transform.rotation = respawnPoint.rotation;

                cc.enabled = true;
            }
        }
    }
}