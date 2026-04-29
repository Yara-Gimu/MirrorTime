using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("إعدادات الصوت")]
    public AudioMixer mainMixer; 
    
    [Header("اربطي السلايدرات هنا")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 1. أول ما تبدأ اللعبة، نبحث عن قيم الصوت المحفوظة
        // إذا اللاعب أول مرة يلعب، بنعطيه القيمة الافتراضية 1 (يعني 100%)
        float savedMaster = PlayerPrefs.GetFloat("SavedMasterVol", 1f);
        float savedMusic = PlayerPrefs.GetFloat("SavedMusicVol", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SavedSFXVol", 1f);

        // 2. نخلي السلايدرات في الشاشة تتحرك وتطابق القيم المحفوظة
        if (masterSlider != null) masterSlider.value = savedMaster;
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        // 3. نطبق هذي القيم على الخلاط (الميكسر) عشان الصوت يشتغل فوراً بالوزنية الصح
        SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    public void SetMasterVolume(float volume)
    {
        mainMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SavedMasterVol", volume); // احفظ التغيير فوراً
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SavedMusicVol", volume); // احفظ التغيير فوراً
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SavedSFXVol", volume); // احفظ التغيير فوراً
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}