using UnityEngine;

public class GateManager : MonoBehaviour
{
    [Header("--- البوابات الجانبية (Side Gates) ---")]
    public GateController[] sideGates; 

    [Header("--- البوابة الرئيسية (Main Gate) ---")]
    [Tooltip("اسحبي مجسم البوابة الرئيسية اللي عليه سكريبت MainGateController هنا")]
    public MainGateController mainGate; // استبدلنا كل الماتيريال بمرجع واحد نظيف

    void Start()
    {
        UpdateGatesState();
    }

    public void UpdateGatesState()
    {
        // 🏗️ الترقية المعمارية: القراءة من المدير المركزي
        int progress = SaveManager.Instance != null ? SaveManager.Instance.currentGateProgress : 0;

        // 1. تحديث البوابات الجانبية
        for (int i = 0; i < sideGates.Length; i++)
        {
            if (sideGates[i] != null)
            {
                sideGates[i].CheckPermission(); 
            }
        }

        // 2. تحديث البوابة الرئيسية (تعتمد على الكود النظيف اللي سويناه لها)
        if (mainGate != null)
        {
            mainGate.CheckGateStatus();
        }
    }

    // تم التعديل لتعتمد على SaveManager
    public void LevelCompleted()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UnlockNextGate();
            UpdateGatesState();
        }
    }
}