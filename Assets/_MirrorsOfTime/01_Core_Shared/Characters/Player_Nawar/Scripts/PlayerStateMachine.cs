using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables; 

[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : MonoBehaviour
{
    [Header("--- إعدادات نوار الأساسية ---")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSmoothTime = 0.1f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2.5f;

    [Header("--- ميكانيكا القفز الاحترافية ---")]
    public float coyoteTime = 0.15f; 
    public float coyoteTimeCounter;

    [Header("--- المراجع ---")]
    public PlayableDirector director; 
    public Transform mainCamera;
    public Animator animator;
    public CharacterController Controller { get; private set; }

    public Vector2 CurrentMovementInput { get; private set; }
    public bool IsRunPressed { get; private set; }
    public bool IsJumpPressed { get; set; } 
    
    public float CurrentVelocityY { get; set; }
    public float TurnSmoothVelocity;

    public float originalHeight;
    public Vector3 originalCenter;

    // 🌟 الترقية المعمارية (AAA Caching): تعريف الحالات مرة واحدة فقط!
    public PlayerIdleState idleState;
    public PlayerMoveState moveState;
    public PlayerJumpState jumpState;
    public PlayerFallState fallState;
    public PlayerCutsceneState cutsceneState;

    private PlayerBaseState currentState;

    void Awake()
    {
        // 🌟 إنشاء الحالات في الذاكرة مرة واحدة فقط
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        jumpState = new PlayerJumpState(this);
        fallState = new PlayerFallState(this);
        cutsceneState = new PlayerCutsceneState(this);
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        Controller = GetComponent<CharacterController>();
        
        if (animator == null) animator = GetComponent<Animator>();
        if (mainCamera == null) mainCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalHeight = Controller.height;
        originalCenter = Controller.center;

        // نبدأ بحالة المشهد السينمائي
        SwitchState(cutsceneState);
    }

    void Update()
    {
        HandleCoyoteTime();
        if (currentState != null) currentState.UpdateState();
        UpdateAnimations();
    }

    private void HandleCoyoteTime()
    {
        if (Controller.isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetBool("IsGrounded", Controller.isGrounded);
            animator.SetFloat("VerticalVelocity", CurrentVelocityY);
        }
    }

    public void SwitchState(PlayerBaseState newState)
    {
        if (currentState != null) currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public void EndCutscene()
    {
        SwitchState(idleState);
    }

    // --- استقبال المدخلات ---
    public void OnMove(InputValue value) { CurrentMovementInput = value.Get<Vector2>(); }
    public void OnJump(InputValue value) { IsJumpPressed = value.isPressed; }
    public void OnSprint(InputValue value) { IsRunPressed = value.isPressed; }
    public void OnInteract(InputValue value)
    {
        if (value.isPressed && animator != null) animator.SetTrigger("Interact");
    }
}