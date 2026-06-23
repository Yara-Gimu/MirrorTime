using UnityEngine;
using System.Collections;

public class SaveNotificationManager : MonoBehaviour
{
    public static SaveNotificationManager Instance { get; private set; }

    [Header("--- واجهة الحفظ (UI) ---")]
    [Tooltip("اسحبي Canvas Group الخاص بلوحة الحفظ الجديدة هنا")]
    public CanvasGroup savePanelGroup;
    public AudioSource saveSound; // (اختياري) صوت خفيف عند الحفظ

    [Header("--- إعدادات الأنيميشن ---")]
    public float fadeInSpeed = 3f;
    public float showDuration = 3f; 
    public float fadeOutSpeed = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
        Instance = this;
        
        if (savePanelGroup != null) savePanelGroup.alpha = 0f;
    }

    public void ShowSaveNotification()
    {
        if (gameObject.activeInHierarchy && savePanelGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(SaveRoutine());
        }
    }

    private IEnumerator SaveRoutine()
    {
        if (saveSound != null) saveSound.Play();

        // ظهور هادئ
        while (savePanelGroup.alpha < 1f)
        {
            savePanelGroup.alpha += Time.deltaTime * fadeInSpeed;
            yield return null;
        }
        savePanelGroup.alpha = 1f;

        // الانتظار
        yield return new WaitForSeconds(showDuration);

        // اختفاء هادئ
        while (savePanelGroup.alpha > 0f)
        {
            savePanelGroup.alpha -= Time.deltaTime * fadeOutSpeed;
            yield return null;
        }
        savePanelGroup.alpha = 0f;
    }
}