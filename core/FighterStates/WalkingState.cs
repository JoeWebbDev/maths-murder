using Godot;
using MathsMurderSpike.Core.enums;

namespace MathsMurderSpike.Core.FighterStates;

public class WalkingState : FighterState
{
    private bool _walkingRight;

    public WalkingState(FighterCommand cmd)
    {
        _walkingRight = cmd == FighterCommand.WalkRight;
    }
    public override void Enter(Fighter fighter)
    {
        fighter.AnimatedSprite.Play("walk");
    }

    public override void HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        switch (cmd)
        {
            case FighterCommand.StopWalk:
                fighter.SwitchMovementState(new IdleState());
                break;
            case FighterCommand.WalkLeft:
                _walkingRight = false;
                break;
            case FighterCommand.WalkRight:
                _walkingRight = true;
                break;
            case FighterCommand.Punch:
                fighter.SwitchCombatState(new PunchState());
                break;
        }
    }

    public override void Process(Fighter fighter, double delta)
    {
        if (fighter.CombatState != null) return;
        
        fighter.AnimatedSprite.FlipH = !_walkingRight;
        var direction = new Vector2(_walkingRight ? 1 : -1, 0);
        fighter.Position += direction * 100 * (float)delta;
    }
}