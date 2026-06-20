using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Toggle))]
public class ToggleAudio : MonoBehaviour
{
    [Header("إعدادات الصوت")]
    public AudioClip toggleOnSound;  
    public AudioClip toggleOffSound; 

    private AudioSource audioSource;
    private Toggle toggle;
    private bool isInitialized = false; // 🌟 حماية التحميل

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(PlayToggleSound);
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        isInitialized = true;
    }

    public void PlayToggleSound(bool isOn)
    {
        if (!isInitialized) return; // منع الإزعاج عند بدء اللعبة

        audioSource.pitch = Random.Range(0.95f, 1.05f);

        if (isOn && toggleOnSound != null)
        {
            audioSource.PlayOneShot(toggleOnSound);
        }
        else if (!isOn && toggleOffSound != null)
        {
            audioSource.PlayOneShot(toggleOffSound);
        }
    }
}