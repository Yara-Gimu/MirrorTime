using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : MonoBehaviour
{
    [Header("--- إعدادات نوار الأساسية ---")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float proneSpeed = 1.5f; // تمت إضافة سرعة الزحف
    public float rotationSmoothTime = 0.1f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2.5f;

    [Header("--- ميكانيكا القفز الاحترافية ---")]
    [Tooltip("زمن الذئب: الوقت المسموح للقفز بعد مغادرة الحافة (بالثواني)")]
    public float coyoteTime = 0.15f; 
    public float coyoteTimeCounter;

    [Header("--- المراجع ---")]
    public Transform mainCamera;
    public Animator animator;
    public CharacterController Controller { get; private set; }

    // --- متغيرات لتخزين مدخلات اللاعب ---
    public Vector2 CurrentMovementInput { get; private set; }
    public bool IsRunPressed { get; private set; }
    public bool IsJumpPressed { get; set; } 
    public bool IsPronePressed { get; set; } // تمت الإضافة لمعرفة هل اللاعب ضغط زحف أم لا
    
    public float CurrentVelocityY { get; set; }
    public float TurnSmoothVelocity;

    // متغيرات لتخزين حجم اللاعب الأصلي عشان نرجعه بعد الزحف
    public float originalHeight;
    public Vector3 originalCenter;

    // --- نظام الحالات (State Machine) ---
    private PlayerBaseState currentState;

    void Start()
    {
        Application.targetFrameRate = 60;
        Controller = GetComponent<CharacterController>();
        
        if (animator == null) animator = GetComponent<Animator>();
        if (mainCamera == null) mainCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // حفظ حجم نوار الأصلي
        originalHeight = Controller.height;
        originalCenter = Controller.center;

        // التعديل السحري: نوار تبدأ اللعبة في حالة "المشهد السينمائي" مسلوبة الإرادة
        SwitchState(new PlayerCutsceneState(this));
    }

    void Update()
    {
        HandleCoyoteTime();

        if (currentState != null)
        {
            currentState.UpdateState();
        }

        UpdateAnimations();
    }

    private void HandleCoyoteTime()
    {
        if (Controller.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
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

    // --- دالة إنهاء المشهد السينمائي (تستدعى من التايم لاين) ---
    public void EndCutscene()
    {
        // نرجع التحكم للاعب بمجرد انتهاء الكات سين
        SwitchState(new PlayerIdleState(this));
    }

    // --- استقبال المدخلات (Input System) ---
    public void OnMove(InputValue value)
    {
        CurrentMovementInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        IsJumpPressed = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        IsRunPressed = value.isPressed;
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed && animator != null)
        {
            animator.SetTrigger("Interact");
        }
    }

    public void OnProne(InputValue value)
    {        
        if (value.isPressed)
        {
            IsPronePressed = !IsPronePressed; // ضغطة للنزول وضغطة للنهوض
        }
    }
}