using UnityEngine;
using System.Collections.Generic;

public class FallReset : MonoBehaviour
{
    [Tooltip("اسحبي هنا الاوبجكت الذي يمثل نقطة العودة")]
    public Transform respawnPoint; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 🌟 إرسال البيانات بشكل صحيح لدراسة تحركات اللاعب!
            Dictionary<string, object> fallData = new Dictionary<string, object>
            {
                { "FallPosition", other.transform.position }
            };
            EventManager.TriggerEvent("Telemetry_Player_Fell", fallData);

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