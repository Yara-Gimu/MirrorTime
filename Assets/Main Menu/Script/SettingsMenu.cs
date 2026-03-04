using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // ضروري عشان الصوت
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer mainMixer; // هنا بنسحب الميكسر اللي سويناه

    // دالة الصوت الرئيسي
    public void SetMasterVolume(float volume)
    {
        // اللوغاريتم عشان الصوت يكون طبيعي للأذن البشرية
        mainMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
    }

    // دالة صوت الموسيقى
    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
    }

    // دالة صوت المؤثرات
    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
    }

    // دالة الجودة (Low, Medium, High)
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // دالة ملء الشاشة
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}