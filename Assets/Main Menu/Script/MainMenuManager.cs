using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton; 

    [Header("Panels")]
    public GameObject mainButtonsPanel; 
    public GameObject settingsPanel;    
    public GameObject creditsPanel;    

    [Header("Scenes Configuration")]
    [Tooltip("اسم مشهد البداية (ممر العلا)")]
    public string newGameSceneName = "Level_01_AlUla"; 
    [Tooltip("اسم مشهد الغرفة الحارسة")]
    public string hubWorldSceneName = "The Hub World"; 

    void Start()
    {
        // 🏗️ الترقية المعمارية: 
        // بدلاً من PlayerPrefs، سنسأل مدير الحفظ (الذي سنبنيه لاحقاً)
        // bool hasSave = SaveManager.Instance.HasSaveData();
        
        // مؤقتاً لحين بناء الـ SaveManager، سنستخدم طريقة بسيطة
        bool hasSave = PlayerPrefs.HasKey("HasPlayedBefore"); 

        if (hasSave) 
        {
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }

        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }

    public void OnNewGameClicked()
    {
        // 🏗️ الترقية المعمارية (قريباً):
        // SaveManager.Instance.StartNewGame(); // تصفير الحفظ القديم
        // EventManager.Trigger("Telemetry_NewGameStarted"); // إرسال إشارة للبيانات

        PlayerPrefs.SetInt("HasPlayedBefore", 1); 
        PlayerPrefs.Save();

        if (FadeManager.instance != null)
            FadeManager.instance.LoadSceneSmoothly(newGameSceneName);
        else
            SceneManager.LoadScene(newGameSceneName); 
    }

    public void OnContinueClicked()
    {
        // 🏗️ الترقية المعمارية (قريباً):
        // EventManager.Trigger("Telemetry_GameContinued"); 

        if (FadeManager.instance != null)
            FadeManager.instance.LoadSceneSmoothly(hubWorldSceneName);
        else
            SceneManager.LoadScene(hubWorldSceneName);
    }

    public void OnSettingsClicked()
    {
        mainButtonsPanel.SetActive(false); 
        settingsPanel.SetActive(true);    
    }

    public void OnCreditsClicked()
    {
        mainButtonsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void OnBackClicked() 
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainButtonsPanel.SetActive(true); 
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Game Closed"); 
    }
}