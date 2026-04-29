using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        // 🏗️ قريباً: إرسال إشارة لمدير البيانات (Telemetry) لتسجيل مكان بداية السقوط
    }

    public override void UpdateState()
    {
        ApplyAirGravity();
        MoveInAir();

        // 🦘 هل ضغط اللاعب قفز وكان "زمن الذئب" مسموح؟ (ينقذه من السقوط!)
        if (ctx.IsJumpPressed && ctx.coyoteTimeCounter > 0f)
        {
            ctx.IsJumpPressed = false;
            ctx.SwitchState(new PlayerJumpState(ctx));
            return;
        }

        // هل لمست نوار الأرض بسلام؟
        if (ctx.Controller.isGrounded)
        {
            if (ctx.CurrentMovementInput.magnitude > 0.1f)
                ctx.SwitchState(new PlayerMoveState(ctx));
            else
                ctx.SwitchState(new PlayerIdleState(ctx));
        }
    }

    public override void ExitState() { }

    private void ApplyAirGravity()
    {
        ctx.CurrentVelocityY += ctx.gravity * ctx.fallMultiplier * Time.deltaTime;
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