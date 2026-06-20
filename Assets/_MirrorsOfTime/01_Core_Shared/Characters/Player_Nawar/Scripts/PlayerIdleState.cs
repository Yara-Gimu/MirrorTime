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
        if (!ctx.Controller.isGrounded)
        {
            ctx.SwitchState(ctx.fallState); 
            return;
        }

        ApplyGravity();

        if (ctx.IsJumpPressed && ctx.coyoteTimeCounter > 0f)
        {
            ctx.IsJumpPressed = false; 
            ctx.CurrentVelocityY = 0f; // 🌟 الإصلاح: تصفير السرعة العمودية لمنع التعليق
            ctx.SwitchState(ctx.jumpState);
            return; 
        }

        if (ctx.CurrentMovementInput.magnitude > 0.1f)
        {
            ctx.SwitchState(ctx.moveState);
        }
    }
    public override void ExitState() { }

    private void ApplyGravity()
    {
        if (ctx.Controller.isGrounded && ctx.CurrentVelocityY < 0) ctx.CurrentVelocityY = -2f;
        ctx.CurrentVelocityY += ctx.gravity * Time.deltaTime;
        ctx.Controller.Move(new Vector3(0, ctx.CurrentVelocityY, 0) * Time.deltaTime);
    }
}