﻿using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class HighKickState : StaminaConsumingState
{
    private Fighter _fighterRef;

    public override float EnterStaminaCost { get; set; } = 14f;

    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        _fighterRef = fighter;
        fighter.AnimationPlayer.Play("high_kick");
        fighter.AudioManager.PlaySfx(fighter.FighterSfx.SwooshPool.GetRandomFromPool());
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
        GodotLogger.LogDebug("High kick animation finished");
        _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationPlayerOnAnimationFinished;
        _fighterRef.SwitchCombatState(null);
    }
}