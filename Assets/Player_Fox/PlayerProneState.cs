using UnityEngine;

public class PlayerProneState : PlayerBaseState
{
    public PlayerProneState(PlayerStateMachine currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        // 1. تفعيل أنيميشن الزحف
        if (ctx.animator != null) ctx.animator.SetBool("isProne", true);
        
        // 2. تصغير حجم التصادم (Collider) عشان نوار تقدر تدخل الكهوف الضيقة
        ctx.Controller.height = ctx.originalHeight / 3f;
        ctx.Controller.center = new Vector3(ctx.originalCenter.x, ctx.originalCenter.y / 3f, ctx.originalCenter.z);
    }

    public override void UpdateState()
    {
        ApplyGravity();
        MoveInProne();

        // السقوط أثناء الزحف (لو زحفت من فوق حافة)
        if (!ctx.Controller.isGrounded)
        {
            ctx.IsPronePressed = false; // نلغي الزحف
            ctx.SwitchState(new PlayerFallState(ctx));
            return;
        }

        // إذا اللاعب ضغط الزر مرة ثانية للنهوض
        if (!ctx.IsPronePressed)
        {
            if (ctx.CurrentMovementInput.magnitude > 0.1f)
                ctx.SwitchState(new PlayerMoveState(ctx));
            else
                ctx.SwitchState(new PlayerIdleState(ctx));
        }
    }

    public override void ExitState()
    {
        // 1. إيقاف أنيميشن الزحف
        if (ctx.animator != null) ctx.animator.SetBool("isProne", false);
        
        // 2. إرجاع حجم التصادم لحجمه الطبيعي للوقوف
        ctx.Controller.height = ctx.originalHeight;
        ctx.Controller.center = ctx.originalCenter;
    }

    private void ApplyGravity()
    {
        if (ctx.Controller.isGrounded && ctx.CurrentVelocityY < 0)
            ctx.CurrentVelocityY = -2f;

        ctx.CurrentVelocityY += ctx.gravity * Time.deltaTime;
    }

    private void MoveInProne()
    {
        Vector3 direction = new Vector3(ctx.CurrentMovementInput.x, 0f, ctx.CurrentMovementInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + ctx.mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(ctx.transform.eulerAngles.y, targetAngle, ref ctx.TurnSmoothVelocity, ctx.rotationSmoothTime);
            ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // نستخدم سرعة الزحف البطيئة
            Vector3 finalVelocity = (moveDir.normalized * ctx.proneSpeed) + new Vector3(0, ctx.CurrentVelocityY, 0);
            ctx.Controller.Move(finalVelocity * Time.deltaTime);

            // تفعيل حركة الزحف للأمام في الـ Blend Tree
            if (ctx.animator != null) ctx.animator.SetFloat("Speed", 1f); 
        }
        else
        {
            // اللاعب توقف عن الحركة وهو منبطح
            if (ctx.animator != null) ctx.animator.SetFloat("Speed", 0f);
            ctx.Controller.Move(new Vector3(0, ctx.CurrentVelocityY, 0) * Time.deltaTime);
        }
    }
}