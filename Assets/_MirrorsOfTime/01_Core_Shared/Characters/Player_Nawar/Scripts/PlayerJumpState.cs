using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        // 1. حساب قوة القفز (نفس اللوجيك الرياضي الممتاز حقك)
        ctx.CurrentVelocityY = Mathf.Sqrt(ctx.jumpHeight * -2f * ctx.gravity);

        // 2. تشغيل أنيميشن القفز
        if (ctx.animator != null) ctx.animator.SetTrigger("Jump");

        // 3. 🏗️ الترقية المعمارية: إرسال إشارة للـ Telemetry لتسجيل القفزة ومكانها
        // EventManager.Trigger("Telemetry_Player_Jumped", ctx.transform.position);
    }

    public override void UpdateState()
    {
        ApplyAirGravity();
        MoveInAir();

        // هل نوار لمست الأرض وهي تنزل؟ إذاً ارجع لحالة الوقوف أو الركض!
        if (ctx.Controller.isGrounded && ctx.CurrentVelocityY <= 0f)
        {
            if (ctx.CurrentMovementInput.magnitude > 0.1f)
                ctx.SwitchState(new PlayerMoveState(ctx));
            else
                ctx.SwitchState(new PlayerIdleState(ctx));
        }
    }

    public override void ExitState() 
    { 
        // هنا نقدر نضيف مؤثر صوتي أو غبار وقت لمس الأرض مستقبلاً
    }

    private void ApplyAirGravity()
    {
        // سرعة الجاذبية تزيد وقت النزول عشان يعطي إحساس (AAA) لوزن الشخصية
        if (ctx.CurrentVelocityY < 0)
        {
            ctx.CurrentVelocityY += ctx.gravity * ctx.fallMultiplier * Time.deltaTime;
        }
        else
        {
            ctx.CurrentVelocityY += ctx.gravity * Time.deltaTime;
        }
    }

    private void MoveInAir()
    {
        Vector3 direction = new Vector3(ctx.CurrentMovementInput.x, 0f, ctx.CurrentMovementInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // السماح بتوجيه نوار وهي في الهواء
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + ctx.mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(ctx.transform.eulerAngles.y, targetAngle, ref ctx.TurnSmoothVelocity, ctx.rotationSmoothTime);
            ctx.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // دمج الحركة مع سرعة القفز العمودية
            Vector3 finalVelocity = (moveDir.normalized * ctx.walkSpeed) + new Vector3(0, ctx.CurrentVelocityY, 0);
            ctx.Controller.Move(finalVelocity * Time.deltaTime);
        }
        else
        {
            // إذا اللاعب فك يده من أزرار الحركة، نوار تستمر بالنزول العمودي فقط
            ctx.Controller.Move(new Vector3(0, ctx.CurrentVelocityY, 0) * Time.deltaTime);
        }
    }
}