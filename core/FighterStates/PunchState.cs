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
            fighter.SwitchCombatState(null);
        }

        fighter.AnimatedSprite.AnimationFinished += OnAnimatedSpriteOnAnimationFinished;
    }

    public override void HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        
    }
}