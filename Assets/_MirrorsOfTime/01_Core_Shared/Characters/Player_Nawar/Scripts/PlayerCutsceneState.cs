using UnityEngine;

public class PlayerCutsceneState : PlayerBaseState
{
    public PlayerCutsceneState(PlayerStateMachine currentContext) : base(currentContext) { }

    public override void EnterState()
    {
        // أول ما يبدأ المشهد، نصفر سرعة نوار عشان ما تتزحلق
        if (ctx.animator != null) ctx.animator.SetFloat("Speed", 0f);
    }

    public override void UpdateState()
    {
        // نترك هذي الدالة فاضية عن قصد!
        // ما فيه جاذبية، ما فيه حركة، ما فيه استجابة للأزرار.
        // التايم لاين هو اللي بيتحكم بحركة نوار بالكامل في هذي اللحظات.
    }

    public override void ExitState()
    {
        // لما يخلص التايم لاين وتطلع من هذي الحالة، ما نحتاج نسوي شيء 
        // لأنها بتنتقل تلقائياً لحالة الوقوف (Idle)
    }
}