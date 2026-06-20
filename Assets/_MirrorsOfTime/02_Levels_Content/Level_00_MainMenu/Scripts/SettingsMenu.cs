using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;
using UnityEngine.InputSystem; 
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("إعدادات الصوت")]
    public AudioMixer mainMixer; 
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voiceSlider; // 🌟 إضافة سلايدر الحوارات الجديد

    [Header("إعدادات رسومات PC")]
    public TMP_Dropdown antiAliasingDropdown;
    public TMP_Dropdown shadowQualityDropdown; // 🌟 الإصلاح هنا: تمت إضافة كلمة TMP_Dropdown

    [Header("--- إعدادات الإدخال (Cross-Platform) ---")]
    public InputActionReference cancelAction; 
    public Button backButton; 

    void Start()
    {
        // سحب قيم الصوت المحفوظة
        float savedMaster = PlayerPrefs.GetFloat("SavedMasterVol", 1f);
        float savedMusic = PlayerPrefs.GetFloat("SavedMusicVol", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SavedSFXVol", 1f);
        float savedVoice = PlayerPrefs.GetFloat("SavedVoiceVol", 1f); 

        if (masterSlider != null) masterSlider.value = savedMaster;
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;
        if (voiceSlider != null) voiceSlider.value = savedVoice; 

        SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
        SetVoiceVolume(savedVoice); 

        // إعدادات الرسومات
        if (antiAliasingDropdown != null)
        {
            int savedAA = PlayerPrefs.GetInt("AntiAliasing", 1);
            antiAliasingDropdown.value = savedAA;
            SetAntiAliasing(savedAA);
            antiAliasingDropdown.onValueChanged.AddListener(SetAntiAliasing);
        }

        if (shadowQualityDropdown != null)
        {
            int savedShadows = PlayerPrefs.GetInt("ShadowQuality", 2);
            shadowQualityDropdown.value = savedShadows;
            SetShadowQuality(savedShadows);
            shadowQualityDropdown.onValueChanged.AddListener(SetShadowQuality);
        }
    }

    void OnEnable()
    {
        if (cancelAction != null)
        {
            cancelAction.action.Enable();
            cancelAction.action.performed += OnCancelPressed;
        }
    }

    void OnDisable()
    {
        if (cancelAction != null) cancelAction.action.performed -= OnCancelPressed;
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        if (backButton != null) backButton.onClick.Invoke(); 
        else if (PauseMenuManager.Instance != null) PauseMenuManager.Instance.CloseSettings();
    }

    public void SetMasterVolume(float volume)
    {
        mainMixer.SetFloat("MasterVol", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SavedMasterVol", volume); 
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SavedMusicVol", volume); 
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SavedSFXVol", volume); 
    }

    public void SetVoiceVolume(float volume)
    {
        mainMixer.SetFloat("VoiceVol", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SavedVoiceVol", volume); 
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetAntiAliasing(int aaIndex)
    {
        int aaValue = 0;
        if (aaIndex == 1) aaValue = 2;
        else if (aaIndex == 2) aaValue = 4;
        else if (aaIndex == 3) aaValue = 8;

        QualitySettings.antiAliasing = aaValue;
        PlayerPrefs.SetInt("AntiAliasing", aaIndex);
        PlayerPrefs.Save();
    }

    public void SetShadowQuality(int shadowIndex)
    {
        if (shadowIndex == 0) QualitySettings.shadows = ShadowQuality.Disable;
        else if (shadowIndex == 1) QualitySettings.shadows = ShadowQuality.HardOnly;
        else if (shadowIndex == 2) QualitySettings.shadows = ShadowQuality.All;

        PlayerPrefs.SetInt("ShadowQuality", shadowIndex);
        PlayerPrefs.Save();
    }
}