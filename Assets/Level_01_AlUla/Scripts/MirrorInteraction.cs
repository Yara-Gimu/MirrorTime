using UnityEngine;

public class MirrorInteraction : MonoBehaviour
{
    [Header("عناصر واجهة المستخدم")]
    public GameObject interactUI; // مكان نص التفاعل (اضغط E)
    public GameObject comicCanvas; // شاشة الكوميكس المؤقتة

    private bool isPlayerNear = false;

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            BreakTheMirror();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    void BreakTheMirror()
    {
        // 🎬 التعديل السحري: نكلم المخرج يسوي تظليم، يفتح الكوميكس، ويخفي زر E
        if (FadeManager.instance != null)
        {
            FadeManager.instance.ShowUIWithFade(comicCanvas, interactUI);
        }
        else
        {
            // لو المخرج مو موجود (للاحتياط) يفتحها فجأة
            if (comicCanvas != null) comicCanvas.SetActive(true);
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}