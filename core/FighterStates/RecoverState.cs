using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class RecoverState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        void OnAnimationFinish(StringName animName)
        {
            fighter.AnimationPlayer.AnimationFinished -= OnAnimationFinish;
            fighter.SwitchCombatState(null);
        }
        
        // This will be a recover animation once I figure out how lol
        // Needed a non-looping animation so went with punch
        fighter.AnimationPlayer.Play("punch");
        fighter.AnimationPlayer.AnimationFinished += OnAnimationFinish;
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        // We are busy recovering, so we consume all state changes
        return true;
    }
}