using System;

public interface IActionDecider
{

    public ActionState ActionState { get; }

    public void MoveAction();
    public void AttackAction();
    public void AcknowledgedEnd();

}