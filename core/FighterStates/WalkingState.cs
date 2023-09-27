using Godot;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.Input;

namespace MathsMurderSpike.Core.FighterStates;

public class WalkingState : FighterState
{
    private Vector2 _direction;

    public WalkingState(FighterCommand cmd)
    {
        var walkCommand = cmd as WalkCommand;
        if (walkCommand == null)
        {
            GD.PushError("Walking state being initialised with non-walk command. Something is wrong");
            return;
        }

        _direction = walkCommand.Direction;
    }
    public override void Enter(Fighter fighter)
    {
        fighter.AnimatedSprite.Play("walk");
    }

    public override void HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        switch (cmd)
        {
            case WalkCommand walkCommand:
                if (walkCommand.Completed)
                {
                    // we NEED to not access input here to get this to work for AI also. but removing this means you stop walking as soon as you
                    // release a walk key (which sends a command.Completed command.) help! :(
                    var existingWalkCommand = FighterInputActions.TryGetWalkCommand();
                    if (existingWalkCommand == null)
                    {
                        fighter.SwitchMovementState(new IdleState());
                        break;
                    }

                    _direction = existingWalkCommand.Direction;
                    break;
                };
                _direction = walkCommand.Direction;
                break;
            case PunchCommand:
                fighter.SwitchCombatState(new PunchState());
                break;
        }
    }

    public override void Process(Fighter fighter, double delta)
    {
        if (fighter.CombatState != null) return;

        if (fighter.AnimatedSprite.Animation != "walk")
        {
            fighter.AnimatedSprite.Play("walk");
        }
        fighter.Position += _direction * 100 * (float)delta;
    }
}