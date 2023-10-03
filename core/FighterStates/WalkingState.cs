using Godot;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.Input;

namespace MathsMurderSpike.Core.FighterStates;

public class WalkingState : FighterState
{
    private Vector2 _direction;

    public WalkingState(FighterCommand cmd)
    {
        if (cmd is not WalkCommand walkCommand)
        {
            GodotLogger.LogError("Walking state being initialised with non-walk command. Something is wrong");
            return;
        }

        _direction = walkCommand.Direction;
    }
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        fighter.AnimationPlayer.Play("walk");
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        switch (cmd)
        {
            case WalkCommand walkCommand:
                if (walkCommand.Completed)
                {
                    fighter.SwitchMovementState(new IdleState());
                    break;
                };
                _direction = walkCommand.Direction;
                break;
            case PunchCommand:
                fighter.SwitchCombatState(new PunchState());
                break;
            case KickCommand:
                fighter.SwitchCombatState(new KickState());
                break;
            case BlockCommand blockCommand:
                if (blockCommand.Completed) break;
                // Block should be in the CombatFSM, since we want to lock most movement when blocking, and we can never block and execute other combat moves simultaneously
                fighter.SwitchCombatState(new BlockState());
                break;
            case DuckCommand duckCommand:
                if (duckCommand.Completed) break;
                fighter.SwitchMovementState(new DuckState());
                break;
        }

        return false;
    }

    public override void Process(Fighter fighter, double delta)
    {
        if (fighter.CombatState != null) return;

        if (fighter.AnimationPlayer.CurrentAnimation != "walk")
        {
            fighter.AnimationPlayer.Play("walk");
        }
        var vec = _direction * fighter.WalkingSpeed * (float)delta;
        fighter.MoveAndCollide(vec);
    }
}