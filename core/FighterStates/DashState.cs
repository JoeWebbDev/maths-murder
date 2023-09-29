using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class DashState : FighterState
{
    private Vector2 _direction;

    public DashState(FighterCommand cmd)
    {
        if (cmd is not DashCommand dashCommand)
        {
            GD.PushError("Dash state being initialised with non-dash command. Something is wrong");
            return;
        }

        _direction = dashCommand.Direction;
    }
    public override void Enter(Fighter fighter)
    {
        // using punch as a placeholder animation
        fighter.AnimationPlayer.Play("fighter_anim_lib/punch");
        
        void OnAnimationFinished(StringName name)
        {
            fighter.AnimationPlayer.AnimationFinished -= OnAnimationFinished;
            fighter.SwitchMovementState(new IdleState());
        }

        fighter.AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        // We'll be in the movement machine, so doesn't really matter what we return here
        return false;
    }

    public override void Process(Fighter fighter, double delta)
    {
        
        if (fighter.CombatState != null) return;

        var vec = _direction * fighter.WalkingSpeed * fighter.DashMultiplier * (float)delta;
        fighter.MoveAndCollide(vec);
    }
}