using Godot;
using MathsMurderSpike.Core.enums;

namespace MathsMurderSpike.Core.FighterStates;

public class PunchState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        fighter.AnimatedSprite.Play("punch");

        void OnAnimatedSpriteOnAnimationFinished()
        {
            GD.Print("Animation finished");
            fighter.SwitchCombatState(null);
        }

        fighter.AnimatedSprite.AnimationFinished += OnAnimatedSpriteOnAnimationFinished;
    }

    public override void Exit(Fighter fighter)
    {
        fighter.SwitchMovementState(new IdleState());
    }

    public override void HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        
    }
}