using UnityEngine;
using UnityEngine.UI;

public class VibrationSettingsUI : MonoBehaviour
{
    [Header("--- زر اهتزاز يد التحكم ---")]
    public Toggle vibrationToggle;

    void Start()
    {
        int isVibrationOn = PlayerPrefs.GetInt("ControllerVibration", 1);
        
        if (vibrationToggle != null)
        {
            vibrationToggle.SetIsOnWithoutNotify(isVibrationOn == 1);
            vibrationToggle.onValueChanged.AddListener(UpdateVibration);
        }
    }

    public void UpdateVibration(bool isEnabled)
    {
        PlayerPrefs.SetInt("ControllerVibration", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
        
        // إذا تم إغلاق الاهتزاز، تأكدي من إيقاف أي اهتزاز يعمل حالياً
        if (!isEnabled)
        {
            StopAllVibrations();
        }
    }

    private void StopAllVibrations()
    {
        // إذا كنتِ تستخدمين New Input System للـ Gamepad:
        // if (UnityEngine.InputSystem.Gamepad.current != null)
        // {
        //     UnityEngine.InputSystem.Gamepad.current.SetMotorSpeeds(0f, 0f);
        // }
    }
}