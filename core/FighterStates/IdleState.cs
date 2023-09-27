using MathsMurderSpike.Core.enums;

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
            case FighterCommand.WalkLeft:
            case FighterCommand.WalkRight:
                fighter.SwitchMovementState(new WalkingState(cmd));
                break;
            case FighterCommand.Punch:
                fighter.SwitchCombatState(new PunchState());
                break;
        }
    }
}