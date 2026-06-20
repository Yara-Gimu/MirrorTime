using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // 🌟 استخدام النظام الحديث

public class InteractCrawlDirector : MonoBehaviour
{
    [Header("إعدادات المسار")]
    public Transform tunnelEndPoint; 
    public float crawlSpeed = 1.2f;
    public float stopDistance = 0.3f; 

    [Header("إعدادات الأنيميتور والوقت")]
    public string proneBool = "isProne"; 
    public string speedFloat = "Speed"; 
    public float startDelay = 1.5f; 

    [Header("--- نظام الإدخال الجديد (Cross-Platform) ---")]
    [Tooltip("اسحبي أكشن الزحف هنا (زر C أو زر الدائرة في اليد)")]
    public InputActionReference crawlAction; // 🌟 ترقية الزر ليدعم جميع المنصات

    private bool canCrawl = false;
    private bool isCrawling = false;
    private GameObject playerRef;

    void OnEnable()
    {
        if (crawlAction != null)
        {
            crawlAction.action.Enable();
            crawlAction.action.performed += OnCrawlActionPressed;
        }
    }

    void OnDisable()
    {
        if (crawlAction != null)
        {
            crawlAction.action.performed -= OnCrawlActionPressed;
        }
    }

    private void OnCrawlActionPressed(InputAction.CallbackContext context)
    {
        // إذا كانت نوار عند المدخل ولم تبدأ بالزحف بعد وضغطت الزر
        if (canCrawl && !isCrawling && playerRef != null)
        {
            StartCoroutine(ForceCrawlSequence(playerRef));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCrawl = true;
            playerRef = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canCrawl = false;
            if (!isCrawling) playerRef = null; 
        }
    }

    IEnumerator ForceCrawlSequence(GameObject player)
    {
        isCrawling = true;
        canCrawl = false; 

        MonoBehaviour playerStateMachine = player.GetComponent("PlayerStateMachine") as MonoBehaviour;
        if (playerStateMachine != null) playerStateMachine.enabled = false;

        Animator anim = player.GetComponentInChildren<Animator>();
        CharacterController cc = player.GetComponent<CharacterController>();

        float originalHeight = 2f; 
        Vector3 originalCenter = Vector3.zero;
        if (cc != null)
        {
            originalHeight = cc.height;
            originalCenter = cc.center;
            
            cc.height = 0.6f; 
            cc.center = new Vector3(originalCenter.x, 0.3f, originalCenter.z); 
        }

        if (anim != null) anim.SetBool(proneBool, true); 

        yield return new WaitForSeconds(startDelay);

        if (anim != null) anim.SetFloat(speedFloat, 1f);

        while (true)
        {
            Vector3 flatPlayerPos = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            Vector3 flatTargetPos = new Vector3(tunnelEndPoint.position.x, 0, tunnelEndPoint.position.z);

            if (Vector3.Distance(flatPlayerPos, flatTargetPos) <= stopDistance) break;

            Vector3 direction = (tunnelEndPoint.position - player.transform.position).normalized;
            direction.y = 0; 
            
            if (direction != Vector3.zero)
            {
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }

            if (cc != null) cc.Move(direction * crawlSpeed * Time.deltaTime);
            else player.transform.position += direction * crawlSpeed * Time.deltaTime;

            yield return null; 
        }

        if (anim != null)
        {
            anim.SetBool(proneBool, false); 
            anim.SetFloat(speedFloat, 0f);
            anim.Play("Idle"); 
        }

        if (cc != null)
        {
            cc.height = originalHeight;
            cc.center = originalCenter;
        }

        if (playerStateMachine != null) playerStateMachine.enabled = true;
        
        isCrawling = false;
        playerRef = null;
    }
}