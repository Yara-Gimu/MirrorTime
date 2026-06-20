using UnityEngine;
using UnityEngine.Playables;

public class PlayerCutsceneState : PlayerBaseState
{
    public PlayerCutsceneState(PlayerStateMachine currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        if (ctx.animator != null) ctx.animator.SetFloat("Speed", 0f);
    }

    public override void UpdateState()
    {
        if (ctx.Controller != null && !ctx.Controller.isGrounded)
        {
            ctx.Controller.Move(new Vector3(0, ctx.gravity * Time.deltaTime, 0));
        }

        // 🌟 الإصلاح الجذري: إذا كان التايم لاين غير موجود في المشهد الحالي أو انتهى، حرر نوار فوراً لمنع التجميد!
        if (ctx.director == null || ctx.director.state != PlayState.Playing)
        {
            ctx.EndCutscene(); // سينقلها لحالة IdleState تلقائياً بسلام
        }
    }

    public override void ExitState() { }
}