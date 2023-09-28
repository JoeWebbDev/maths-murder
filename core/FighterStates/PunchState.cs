using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class PunchState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        fighter.AnimatedSprite.Play("punch");

        void OnAnimatedSpriteOnAnimationFinished()
        {
            GD.Print("Animation finished");
            fighter.AnimatedSprite.AnimationFinished -= OnAnimatedSpriteOnAnimationFinished;
            fighter.SwitchCombatState(null);
        }

        fighter.AnimatedSprite.AnimationFinished += OnAnimatedSpriteOnAnimationFinished;
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is WalkCommand { Completed: true }) fighter.SwitchMovementState(new IdleState());
        return true;
    }
}