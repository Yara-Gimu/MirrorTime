using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        ctx.CurrentVelocityY = Mathf.Sqrt(ctx.jumpHeight * -2f * ctx.gravity);
        if (ctx.animator != null) ctx.animator.SetTrigger("Jump");
    }

    public override void UpdateState()
    {
        ApplyAirGravity();
        MoveInAir();

        if (ctx.Controller.isGrounded && ctx.CurrentVelocityY <= 0f)
        {
            // 🌟 استخدام النسخ الجاهزة (Cached States)
            if (ctx.CurrentMovementInput.magnitude > 0.1f)
                ctx.SwitchState(ctx.moveState);
            else
                ctx.SwitchState(ctx.idleState);
        }
    }

    public override void ExitState() { }

    private void ApplyAirGravity()
    {
        if (ctx.CurrentVelocityY < 0)
            ctx.CurrentVelocityY += ctx.gravity * ctx.fallMultiplier * Time.deltaTime;
        else
            ctx.CurrentVelocityY += ctx.gravity * Time.deltaTime;
    }

    private void MoveInAir()
    {
        Vector3 direction = new Vector3(ctx.CurrentMovementInput.x, 0f, ctx.CurrentMovementInput.y).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + ctx.mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(ctx.transform.eulerAngles.y, targetAngle, ref ctx.TurnSmoothVelocity, ctx.rotationSmoothTime);
            ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 finalVelocity = (moveDir.normalized * ctx.walkSpeed) + new Vector3(0, ctx.CurrentVelocityY, 0);
            ctx.Controller.Move(finalVelocity * Time.deltaTime);
        }
        else
        {
            ctx.Controller.Move(new Vector3(0, ctx.CurrentVelocityY, 0) * Time.deltaTime);
        }
    }
}