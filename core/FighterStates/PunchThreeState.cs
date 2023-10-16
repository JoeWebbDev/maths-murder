using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class PunchThreeState : StaminaConsumingState
{
    private Fighter _fighterRef;

    public override float EnterStaminaCost { get; set; } = 14f;
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        _fighterRef = fighter;
        fighter.AnimationPlayer.Play("punch_three");
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
        GodotLogger.LogDebug($"Second punch hit for {fighter.HitDamage + 2}");
        fighter.DealDamage(fighter.HitDamage + 2, target);
    }
    
    private void OnAnimationPlayerOnAnimationFinished(StringName name)
    {
        GodotLogger.LogDebug("Animation finished");
        _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationPlayerOnAnimationFinished;
        _fighterRef.SwitchCombatState(null);
    }
}