using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class DuckState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        
        // Code to change animation goes here
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is DuckCommand { Completed: true })
        {
            fighter.SwitchMovementState(new IdleState());
            return false;
        }
        // Code to handle punching while ducking etc goes here!

        return true;
    }
}