using System.Collections;
using UnityEngine;

public class RuneCinematicFade : MonoBehaviour
{
    [Header("Runes Setup")]
    public Renderer[] runes;

    [Header("Timing Settings")]
    public float delayBetweenRunes = 0.6f; // التأخير بين كل نقش ونقش
    public float fadeDuration = 2.0f;     // مدة ظهور النقش الواحد (زيديها للبطء)

    [Header("The Feel (المنحنى السينمائي)")]
    [Tooltip("ارسمي المنحنى بحيث يبدأ منخفضاً جداً ثم يرتفع بقوة في النهاية")]
    public AnimationCurve appearanceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private static readonly int FadeAmountID = Shader.PropertyToID("_FadeAmount");
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(PlayWave());
        }
    }

    private IEnumerator PlayWave()
    {
        foreach (Renderer rune in runes)
        {
            if (rune != null) StartCoroutine(FadeIn(rune));
            yield return new WaitForSeconds(delayBetweenRunes);
        }
    }

    private IEnumerator FadeIn(Renderer rune)
    {
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            // السحر هنا: نأخذ الوقت ونحوله لقيمة بناءً على المنحنى اللي رسمتيه
            float curvedValue = appearanceCurve.Evaluate(progress);

            rune.GetPropertyBlock(propBlock);
            propBlock.SetFloat(FadeAmountID, curvedValue);
            rune.SetPropertyBlock(propBlock);

            yield return null;
        }
        
        // التأكيد على القيمة النهائية
        propBlock.SetFloat(FadeAmountID, 1f);
        rune.SetPropertyBlock(propBlock);
    }
}