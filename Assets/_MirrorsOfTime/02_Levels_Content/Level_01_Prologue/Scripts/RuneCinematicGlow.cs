using System.Collections;
using UnityEngine;

public class RuneCinematicGlow : MonoBehaviour
{
    [Header("Runes Setup")]
    public Renderer[] runes;

    [Header("Glow Settings")]
    [ColorUsage(true, true)] 
    public Color targetGlowColor = Color.cyan;

    [Header("Timing Settings")]
    public float delayBetweenRunes = 0.6f; 
    public float fadeDuration = 3.0f;     

    [Header("The Feel (المنحنى السينمائي)")]
    public AnimationCurve appearanceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private static readonly string ColorProperty = "Color_80E46BEA";
    private bool hasTriggered = false;

    private void Start()
    {
        Debug.Log("🚀 اللعبة بدأت: جاري إطفاء النقوش...");
        foreach (Renderer rune in runes)
        {
            if (rune != null)
            {
                rune.material.SetColor(ColorProperty, Color.black); 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("🚶‍♀️ جسم لمس التريجر! اسم الجسم: " + other.gameObject.name + " | التاق حقه: " + other.tag);

        if (!hasTriggered && other.CompareTag("Player"))
        {
            Debug.Log("✅ نورة دخلت التريجر بنجاح! جاري تشغيل موجة النور...");
            hasTriggered = true;
            StartCoroutine(PlayWave());
        }
    }

    private IEnumerator PlayWave()
    {
        foreach (Renderer rune in runes)
        {
            if (rune != null) StartCoroutine(FadeGlow(rune));
            yield return new WaitForSeconds(delayBetweenRunes);
        }
    }

    private IEnumerator FadeGlow(Renderer rune)
    {
        float timer = 0f;
        Material mat = rune.material;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            
            float curveVal = appearanceCurve.Evaluate(progress);
            Color currentColor = targetGlowColor * curveVal;

            mat.SetColor(ColorProperty, currentColor);
            yield return null;
        }
        
        mat.SetColor(ColorProperty, targetGlowColor);
    }
}