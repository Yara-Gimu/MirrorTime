using UnityEngine;
using UnityEngine.Playables; 

public class PlayerCutsceneState : PlayerBaseState
{
    public PlayerCutsceneState(PlayerStateMachine currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        // تصفير السرعة أول ما يبدأ المشهد
        if (ctx.animator != null) ctx.animator.SetFloat("Speed", 0f);
    }

    public override void UpdateState()
    {
        // 1. تطبيق الجاذبية عشان نوار ما تسبح في الهواء!
        if (ctx.Controller != null && !ctx.Controller.isGrounded)
        {
            ctx.Controller.Move(new Vector3(0, ctx.gravity * Time.deltaTime, 0));
        }

        // 2. نراقب التايم لاين
        if (ctx.director != null)
        {
            // إذا التايم لاين خلص لعبه ووقف
            if (ctx.director.state != PlayState.Playing)
            {
                // ✨ الحل هنا: نطلب من الـ Context (نظامك الأساسي) ينهي المشهد باستخدام دالتك!
                ctx.EndCutscene(); 
            }
        }
    }

    public override void ExitState()
    {
        Debug.Log("انتهى العرض السينمائي، نوار جاهزة للعب!");
    }
}