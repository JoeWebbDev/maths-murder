using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class BlockState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        fighter.AnimatedSprite.Play("block");
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is not BlockCommand blockCommand) return false;
        
        // What do we switch to here? Walking or idling? we need a way to keep track of previous states (you mentioned this before but idk how!)
        if (blockCommand.Completed)
        {
            // fighter.SwitchMovementState();
        }

        return false;
    }
}