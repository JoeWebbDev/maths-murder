using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public abstract class StaminaConsumingState : FighterState
{
    public virtual float EnterStaminaCost { get; set; }
    public virtual float ProcessStaminaCost { get; set; }

    public override void Process(Fighter fighter, double delta)
    {
        base.Process(fighter, delta);
        if (ProcessStaminaCost > 0) fighter.SpendStamina(ProcessStaminaCost * (float)delta);
    }

    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        if (EnterStaminaCost > 0) fighter.SpendStamina(EnterStaminaCost);
    }
}