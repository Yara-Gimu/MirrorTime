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

    private static readonly int ColorPropertyID = Shader.PropertyToID("Color_80E46BEA");
    private bool hasTriggered = false;
    
    // 🌟 استخدام البلوك الموحد للذاكرة
    private MaterialPropertyBlock propBlock;

    private void Start()
    {
        propBlock = new MaterialPropertyBlock();
        
        Debug.Log("🚀 اللعبة بدأت: جاري إطفاء النقوش...");
        foreach (Renderer rune in runes)
        {
            if (rune != null)
            {
                // 🌟 إطفاء النقوش بطريقة آمنة على الذاكرة
                rune.GetPropertyBlock(propBlock);
                propBlock.SetColor(ColorPropertyID, Color.black);
                rune.SetPropertyBlock(propBlock);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            Debug.Log("✅ نوار دخلت التريجر بنجاح! جاري تشغيل موجة النور...");
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

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            
            float curveVal = appearanceCurve.Evaluate(progress);
            Color currentColor = targetGlowColor * curveVal;

            // 🌟 تغيير اللون بدون تدمير الـ Batching
            rune.GetPropertyBlock(propBlock);
            propBlock.SetColor(ColorPropertyID, currentColor);
            rune.SetPropertyBlock(propBlock);
            
            yield return null;
        }
        
        // التأكيد على اللون النهائي
        rune.GetPropertyBlock(propBlock);
        propBlock.SetColor(ColorPropertyID, targetGlowColor);
        rune.SetPropertyBlock(propBlock);
    }
}