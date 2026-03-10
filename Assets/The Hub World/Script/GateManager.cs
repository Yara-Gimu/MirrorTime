using UnityEngine;

public class GateManager : MonoBehaviour
{
    [Header("--- Bawabats (Side Gates) ---")]
    public GateController[] sideGates; 

    [Header("--- Main Gate Visuals ---")]
    public Renderer mainGateRenderer;    
    public GameObject mainGatePortalObj; 

    [Header("--- Material Settings ---")]
    public Material litMaterial;   
    public Material unlitMaterial; 
    
    public int rightMaterialIndex = 0; 
    public int leftMaterialIndex = 1;  

    void Start()
    {
        UpdateGatesState();
    }

    public void UpdateGatesState()
    {
        // 🏗️ الترقية المعمارية: القراءة من المدير المركزي بدلاً من PlayerPrefs
        int progress = SaveManager.Instance != null ? SaveManager.Instance.currentGateProgress : 0;

        for (int i = 0; i < sideGates.Length; i++)
        {
            if (sideGates[i] != null)
            {
                sideGates[i].CheckPermission(); 
            }
        }

        UpdateMainGateMaterials(progress);
    }

    void UpdateMainGateMaterials(int progress)
    {
        Material[] mats = mainGateRenderer.materials;

        if (progress >= 2) 
        {
            if (mats.Length > rightMaterialIndex) mats[rightMaterialIndex] = litMaterial;
        }
        else 
        {
            if (mats.Length > rightMaterialIndex) mats[rightMaterialIndex] = unlitMaterial;
        }

        if (progress >= 4)
        {
            if (mats.Length > leftMaterialIndex) mats[leftMaterialIndex] = litMaterial;
            if (mainGatePortalObj) mainGatePortalObj.SetActive(true);
        }
        else
        {
            if (mats.Length > leftMaterialIndex) mats[leftMaterialIndex] = unlitMaterial;
            if (mainGatePortalObj) mainGatePortalObj.SetActive(false);
        }

        mainGateRenderer.materials = mats;
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