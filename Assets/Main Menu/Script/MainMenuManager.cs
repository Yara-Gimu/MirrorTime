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
        // هل اللاعب لعب من قبل؟
        if (PlayerPrefs.HasKey("HasPlayedBefore")) 
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
        // نحفظ إن اللاعب بدأ اللعبة عشان يظهر زر الإكمال المرة الجاية
        PlayerPrefs.SetInt("HasPlayedBefore", 1); 
        PlayerPrefs.Save();

        // 🎬 التعديل السحري: الانتقال بنعومة لممر العلا
        if (FadeManager.instance != null)
            FadeManager.instance.LoadSceneSmoothly(newGameSceneName);
        else
            SceneManager.LoadScene(newGameSceneName); // انتقال عادي لو الستارة مو موجودة
    }

    public void OnContinueClicked()
    {
        // 🎬 التعديل السحري: الانتقال بنعومة للغرفة الحارسة
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