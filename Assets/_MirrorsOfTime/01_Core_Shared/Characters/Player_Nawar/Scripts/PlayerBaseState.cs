public abstract class PlayerBaseState
{
    // مرجع للمدير الأساسي عشان الحالة تقدر توصل للكاميرا والانيميشن
    protected PlayerStateMachine ctx; 

    // دالة البناء
    public PlayerBaseState(PlayerStateMachine currentContext)
    {
        ctx = currentContext;
    }

    // هذي الدوال الثلاثة هي قلب كل حالة
    public abstract void EnterState();    // أول ما تدخل نوار في هذي الحالة
    public abstract void UpdateState();   // التحديث المستمر طوال بقائها في الحالة
    public abstract void ExitState();     // لحظة خروجها من الحالة
}