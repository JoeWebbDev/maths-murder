using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class IdleState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        fighter.AnimatedSprite.Play("idle");
    }

    public override void HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        switch (cmd)
        {
            case WalkCommand walkCommand:
                if (walkCommand.Completed) break;
                fighter.SwitchMovementState(new WalkingState(cmd));
                break;
            case PunchCommand:
                fighter.SwitchCombatState(new PunchState());
                break;
        }
    }
}