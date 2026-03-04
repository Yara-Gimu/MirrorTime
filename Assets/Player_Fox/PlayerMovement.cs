using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("إعدادات الحركة (Movement)")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    [Tooltip("سرعة دوران الشخصية مع الكاميرا")]
    public float rotationSmoothTime = 0.1f;

    [Header("إعدادات القفز والجاذبية (Jump & Gravity)")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2.5f;

    [Header("المراجع (References)")]
    public Transform mainCamera;
    public Animator animator;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity; 
    private float turnSmoothVelocity;
    private bool isRunning;

    void Start()
    {
        // إجبار اللعبة تشتغل على 60 إطار لحل أي استهبال أو لاق في المحرر
        Application.targetFrameRate = 60;

        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();
        if (mainCamera == null) mainCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null) animator.SetTrigger("Jump");
        }
    }

    public void OnSprint(InputValue value)
    {
        isRunning = value.isPressed;
    }

    // --- الإضافة الجديدة: زر التفاعل (مثل لمس المرآة أو التقاط غرض) ---
    public void OnInteract(InputValue value)
    {
        if (value.isPressed && animator != null)
        {
            animator.SetTrigger("Interact");
        }
    }

    void Update()
    {
        // 1. معالجة الجاذبية
        HandleGravity();
        
        // 2. حساب اتجاه وسرعة المشي
        Vector3 moveVelocity = CalculateMovement();
        
        // 3. دمج الحركة والجاذبية في أمر تحريك *واحد* فقط
        Vector3 finalVelocity = moveVelocity + velocity;
        controller.Move(finalVelocity * Time.deltaTime);
        
        // 4. تحديث الأنيميشن
        UpdateAnimations();
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // تثبيت اللاعب على الأرض بقوة خفيفة
        }

        if (velocity.y < 0)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private Vector3 CalculateMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 moveDir = Vector3.zero;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            
            return moveDir.normalized * currentSpeed;
        }
        
        return Vector3.zero; // إذا ما ضغط شيء، يرجع صفر
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        float speedPercent = 0f;
        if (moveInput.magnitude > 0.1f)
        {
            speedPercent = isRunning ? 1f : 0.5f;
        }

        // إرسال السرعة مباشرة
        animator.SetFloat("Speed", speedPercent);
        animator.SetBool("IsGrounded", controller.isGrounded);
        animator.SetFloat("VerticalVelocity", velocity.y);
    }
}