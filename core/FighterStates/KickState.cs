using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class KickState : FighterState
{
    private Fighter _fighterRef;
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        _fighterRef = fighter;
        fighter.AnimationPlayer.Play(fighter.MovementState is DuckState ? "low_kick" : "kick");
        fighter.AnimationPlayer.AnimationFinished += OnAnimationPlayerOnAnimationFinished;
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is WalkCommand { Completed: true }) fighter.SwitchMovementState(new IdleState());
        if (cmd is DuckCommand { Completed: true }) fighter.SwitchMovementState(new IdleState());
        
        return true;
    }

    public override void OnHit(Fighter fighter, Fighter target)
    {
        GodotLogger.LogDebug($"Hit for {fighter.HitDamage}");
        fighter.DealDamage(fighter.HitDamage, target);
    }
    
    private void OnAnimationPlayerOnAnimationFinished(StringName name)
    {
        GodotLogger.LogDebug("Kick animation finished");
        _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationPlayerOnAnimationFinished;
        _fighterRef.SwitchCombatState(null);
    }
}