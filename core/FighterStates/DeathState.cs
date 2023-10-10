using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class DeathState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        // Play death animation - borrowing recover for now
        fighter.AnimationPlayer.Play("recover");
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        // We're dead! consume all commands
        return true;
    }
}