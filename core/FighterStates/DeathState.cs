using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class DeathState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        // Play death animation etc etc. borrowing punch for now
        fighter.AnimationPlayer.Play("fighter_anim_lib/punch");
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        // We're dead! consume all commands
        return true;
    }
}