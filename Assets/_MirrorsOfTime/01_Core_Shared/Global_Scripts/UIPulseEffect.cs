using UnityEngine;
using UnityEngine.UI;

public class UIPulseEffect : MonoBehaviour
{
    private Image image;
    public float pulseSpeed = 3f; // سرعة النبض (الوميض)
    public float minAlpha = 0.3f; // أقل درجة خفوت (لكي لا تختفي تماماً)
    public float maxAlpha = 1f;   // أقصى درجة إضاءة

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (image == null) return;
        
        // عملية حسابية تصنع وميضاً ناعماً يشبه التنفس البطيء
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        Color newColor = image.color;
        newColor.a = alpha;
        image.color = newColor;
    }
}