using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class PunchState : FighterState
{
    private bool _canDealDamage = true;
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        fighter.AnimationPlayer.Play("fighter_anim_lib/punch");
        
        void OnAnimationPlayerOnAnimationFinished(StringName name)
        {
            GodotLogger.LogDebug("Animation finished");
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

    public override void OnHit(Fighter fighter, Fighter target)
    {
        if (!_canDealDamage) return;
        
        GodotLogger.LogDebug($"Hit for {fighter.HitDamage}");
        target.TakeDamage(fighter.HitDamage);
        _canDealDamage = false;
    }
}