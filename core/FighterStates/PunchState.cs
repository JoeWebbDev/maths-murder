using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class PunchState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        fighter.AnimationPlayer.Play("fighter_anim_lib/punch");
        
        void OnAnimationPlayerOnAnimationFinished(StringName name)
        {
            GD.Print("Animation finished");
            fighter.AnimationPlayer.AnimationFinished -= OnAnimationPlayerOnAnimationFinished;
            fighter.SwitchCombatState(null);
        }

        fighter.AnimationPlayer.AnimationFinished += OnAnimationPlayerOnAnimationFinished;
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is WalkCommand { Completed: true }) fighter.SwitchMovementState(new IdleState());
        return true;
    }
}