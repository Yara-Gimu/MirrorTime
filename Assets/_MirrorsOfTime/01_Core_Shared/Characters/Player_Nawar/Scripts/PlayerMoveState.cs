using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine currentContext) : base(currentContext) { }

    public override void EnterState() 
    { 
        if (PauseMenuManager.Instance != null) PauseMenuManager.Instance.TriggerPauseHint();
    }

public override void UpdateState()
    {
        if (!ctx.Controller.isGrounded)
        {
            ctx.SwitchState(ctx.fallState);
            return;
        }

        ApplyGravity();
        MoveNawar();

        if (ctx.IsJumpPressed && ctx.coyoteTimeCounter > 0f)
        {
            ctx.IsJumpPressed = false; 
            ctx.CurrentVelocityY = 0f; // 🌟 الإصلاح: تصفير السرعة العمودية لمنع التعليق
            ctx.SwitchState(ctx.jumpState);
            return; 
        }

        if (ctx.CurrentMovementInput.magnitude <= 0.1f)
        {
            ctx.SwitchState(ctx.idleState);
        }
    }

    public override void ExitState() { }

    private void ApplyGravity()
    {
        if (ctx.Controller.isGrounded && ctx.CurrentVelocityY < 0) ctx.CurrentVelocityY = -2f;
        ctx.CurrentVelocityY += ctx.gravity * Time.deltaTime;
    }

    private void MoveNawar()
    {
        Vector3 direction = new Vector3(ctx.CurrentMovementInput.x, 0f, ctx.CurrentMovementInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + ctx.mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(ctx.transform.eulerAngles.y, targetAngle, ref ctx.TurnSmoothVelocity, ctx.rotationSmoothTime);
            ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float currentSpeed = ctx.IsRunPressed ? ctx.runSpeed : ctx.walkSpeed;

            Vector3 finalVelocity = (moveDir.normalized * currentSpeed) + new Vector3(0, ctx.CurrentVelocityY, 0);
            ctx.Controller.Move(finalVelocity * Time.deltaTime);

            if (ctx.animator != null) ctx.animator.SetFloat("Speed", ctx.IsRunPressed ? 1f : 0.5f);
        }
    }
}