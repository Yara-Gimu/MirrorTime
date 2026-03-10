using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        if (ctx.animator != null) ctx.animator.SetFloat("Speed", 0f);
    }

    public override void UpdateState()
    {
        // 🪂 اللمسة الأخيرة: هل نوار لم تعد تلمس الأرض؟ اذهب لحالة السقوط!
        if (!ctx.Controller.isGrounded)
        {
            ctx.SwitchState(new PlayerFallState(ctx));
            return;
        }

        ApplyGravity();

        // 🦘 هل ضغط اللاعب زر القفز وكان الـ Coyote Time مسموح؟
        if (ctx.IsJumpPressed && ctx.coyoteTimeCounter > 0f)
        {
            ctx.IsJumpPressed = false; // تصفير الزر عشان ما تقفز مرتين
            ctx.SwitchState(new PlayerJumpState(ctx));
            return; // إيقاف الكود هنا
        }

        // هل ضغط أزرار الحركة؟
        if (ctx.CurrentMovementInput.magnitude > 0.1f)
        {
            ctx.SwitchState(new PlayerMoveState(ctx));
        }
    }

    public override void ExitState() { }

    private void ApplyGravity()
    {
        if (ctx.Controller.isGrounded && ctx.CurrentVelocityY < 0)
            ctx.CurrentVelocityY = -2f;

        ctx.CurrentVelocityY += ctx.gravity * Time.deltaTime;
        ctx.Controller.Move(new Vector3(0, ctx.CurrentVelocityY, 0) * Time.deltaTime);
    }
}