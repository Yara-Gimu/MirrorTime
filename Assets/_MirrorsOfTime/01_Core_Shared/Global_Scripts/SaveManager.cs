using UnityEngine;
using System.Collections.Generic;
using Esper.ESave; 

[System.Serializable]
public class GameData
{
    public int currentGateProgress = 0; 
    public bool hasPlayedBefore = false;
    public bool hasSeenHubIntro = false;
    
    public string currentSceneName = "Prologue_Scene";
    public float playerPosX = 0f;
    public float playerPosY = 0f;
    public float playerPosZ = 0f;

    public List<string> collectedTools = new List<string>(); 
}

[RequireComponent(typeof(SaveFileSetup))] 
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("--- بيانات اللعبة ---")]
    public GameData gameData = new GameData();

    private SaveFile saveFile; 

    public int currentGateProgress 
    { 
        get { return gameData.currentGateProgress; } 
        set { gameData.currentGateProgress = value; } 
    }

    public bool hasSeenHubIntro 
    { 
        get { return gameData.hasSeenHubIntro; } 
        set { gameData.hasSeenHubIntro = value; } 
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            saveFile = GetComponent<SaveFileSetup>().GetSaveFile();
            LoadGame(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(Vector3 playerPosition, string sceneName)
    {
        gameData.playerPosX = playerPosition.x;
        gameData.playerPosY = playerPosition.y;
        gameData.playerPosZ = playerPosition.z;
        gameData.currentSceneName = sceneName;

        saveFile.AddOrUpdateData("Nawar_Progress", gameData);
        saveFile.Save(); 
        
        Debug.Log("💾 [SaveManager] تم حفظ تقدم نوار بنجاح!");
    }

    public void LoadGame()
    {
        saveFile.Load();

        if (saveFile.HasData("Nawar_Progress"))
        {
            gameData = saveFile.GetData<GameData>("Nawar_Progress");
            Debug.Log("📂 [SaveManager] تم تحميل اللعبة بنجاح.");
        }
        else
        {
            Debug.Log("📄 [SaveManager] لا يوجد ملف حفظ سابق. هذه رحلة جديدة.");
        }
    }

    public bool HasSaveData()
    {
        return saveFile != null && saveFile.HasData("Nawar_Progress");
    }

    public void StartNewGame()
    {
        gameData = new GameData();
        gameData.hasPlayedBefore = true;
        
        SaveGame(Vector3.zero, "Prologue_Scene");
        Debug.Log("✨ [SaveManager] تم بدء رحلة جديدة!");
    }

    public void UnlockNextGate()
    {
        gameData.currentGateProgress++;
        SaveGame(new Vector3(gameData.playerPosX, gameData.playerPosY, gameData.playerPosZ), gameData.currentSceneName);
    }
}