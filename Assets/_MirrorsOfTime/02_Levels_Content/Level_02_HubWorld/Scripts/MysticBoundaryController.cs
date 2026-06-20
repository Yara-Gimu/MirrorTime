using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MysticBoundaryController : MonoBehaviour
{
    [Header("--- إعدادات المسافات ---")]
    public Transform hubCenter; 
    public float warningDistance = 100f; 
    public float maxDistance = 150f; 
    public Transform safeReturnPoint; 

    [Header("--- إعدادات التأثيرات ---")]
    public CanvasGroup fogFadeCanvas; 
    public AudioSource mysticWindSound; 
    [Range(0, 1)] public float maxWindVolume = 0.6f;

    private GameObject player;
    private PlayerStateMachine cachedPlayerMovement; // 🌟 التخزين المعماري
    private bool isTeleporting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            cachedPlayerMovement = player.GetComponent<PlayerStateMachine>();
        }

        if (fogFadeCanvas != null) fogFadeCanvas.alpha = 0f;
        if (mysticWindSound != null) {
            mysticWindSound.volume = 0;
            mysticWindSound.loop = true;
            mysticWindSound.Play();
        }
    }

    void Update()
    {
        if (player == null || isTeleporting || hubCenter == null) return;

        float distance = Vector3.Distance(player.transform.position, hubCenter.position);

        if (distance < warningDistance)
        {
            fogFadeCanvas.alpha = Mathf.Lerp(fogFadeCanvas.alpha, 0, Time.deltaTime);
            mysticWindSound.volume = Mathf.Lerp(mysticWindSound.volume, 0, Time.deltaTime);
        }
        else if (distance >= warningDistance && distance < maxDistance)
        {
            float proximity = (distance - warningDistance) / (maxDistance - warningDistance);
            fogFadeCanvas.alpha = proximity * 0.8f; 
            mysticWindSound.volume = proximity * maxWindVolume;
        }
        else if (distance >= maxDistance)
        {
            StartCoroutine(LostInFogRoutine());
        }
    }

    IEnumerator LostInFogRoutine()
    {
        isTeleporting = true;

        if (cachedPlayerMovement != null) cachedPlayerMovement.enabled = false;

        while (fogFadeCanvas.alpha < 1f)
        {
            fogFadeCanvas.alpha += Time.deltaTime * 3f;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        player.transform.position = safeReturnPoint.position;
        Vector3 lookAtPos = new Vector3(hubCenter.position.x, player.transform.position.y, hubCenter.position.z);
        player.transform.LookAt(lookAtPos);

        yield return new WaitForSeconds(0.5f);

        while (fogFadeCanvas.alpha > 0f)
        {
            fogFadeCanvas.alpha -= Time.deltaTime * 1.5f;
            mysticWindSound.volume -= Time.deltaTime * 0.5f;
            yield return null;
        }

        if (cachedPlayerMovement != null) cachedPlayerMovement.enabled = true;
        isTeleporting = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (hubCenter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hubCenter.position, warningDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hubCenter.position, maxDistance);
        }
    }
}