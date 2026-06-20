using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class MainMenuManager : MonoBehaviour
{
    [Header("--- الأزرار الرئيسية (لتركيز اليد) ---")]
    public GameObject continueButton; 
    public GameObject newGameButton;  
    
    [Header("--- أول زر في القوائم الأخرى (لتركيز اليد) ---")]
    public GameObject settingsFirstElement; 
    public GameObject creditsFirstElement; 

    [Header("Panels")]
    public GameObject mainButtonsPanel; 
    public GameObject settingsPanel;    
    public GameObject creditsPanel;     

    [Header("Scenes Configuration")]
    public string newGameSceneName = "Level_01_AlUla"; 
    public string hubWorldSceneName = "The Hub World"; 

    void Start()
    {
        bool hasSave = SaveManager.Instance.HasSaveData();
        continueButton.gameObject.SetActive(hasSave);

        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);

        SetFocusTo(hasSave ? continueButton : newGameButton);
    }

    public void OnNewGameClicked()
    {
        SaveManager.Instance.StartNewGame();
        EventManager.TriggerEvent("Telemetry_NewGameStarted");

        Time.timeScale = 1f; // 🌟 أمان لضمان عدم تجمد المشهد الجديد

        if (FadeManager.instance != null) FadeManager.instance.LoadSceneSmoothly(newGameSceneName);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(newGameSceneName);
    }

public void OnContinueClicked()
    {
        EventManager.TriggerEvent("Telemetry_GameContinued"); 

        Time.timeScale = 1f; 

        // 🌟 الإصلاح السحري: جلب اسم المشهد الفعلي الذي وقف فيه اللاعب من ملف الحفظ مباشرة بدلاً من كتابته يدوياً!
        string actualSavedScene = SaveManager.Instance.gameData.currentSceneName;

        if (FadeManager.instance != null) 
            FadeManager.instance.LoadSceneSmoothly(actualSavedScene);
        else 
            UnityEngine.SceneManagement.SceneManager.LoadScene(actualSavedScene);
    }

    public void OnSettingsClicked()
    {
        mainButtonsPanel.SetActive(false); 
        settingsPanel.SetActive(true); 
        SetFocusTo(settingsFirstElement);
    }

    public void OnCreditsClicked()
    {
        mainButtonsPanel.SetActive(false);
        creditsPanel.SetActive(true);
        SetFocusTo(creditsFirstElement);
    }

    public void OnBackClicked() 
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainButtonsPanel.SetActive(true); 
        
        bool hasSave = SaveManager.Instance.HasSaveData();
        SetFocusTo(hasSave ? continueButton : newGameButton);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    private void SetFocusTo(GameObject target)
    {
        if (target != null)
        {
            EventSystem.current.SetSelectedGameObject(null); 
            EventSystem.current.SetSelectedGameObject(target); 
        }
    }
}