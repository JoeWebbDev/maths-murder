using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class PunchState : StaminaConsumingState
{
    private bool _canDealDamage = true;
    private bool _isDuckingPunch = false;
    private float _punchComboWindow = 0.3f;
    private double _elapsedTimeSincePunch;
    private Fighter _fighterRef;

    public override float EnterStaminaCost { get; set; } = 6f;
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        _fighterRef = fighter;
        _isDuckingPunch = fighter.MovementState is DuckState;
        fighter.AnimationPlayer.Play(_isDuckingPunch ? "duck_punch" : "punch");
        fighter.AudioManager.PlaySfx(fighter.FighterSfx.SwooshPool.GetRandomFromPool());
        fighter.AnimationPlayer.AnimationFinished += OnAnimationPlayerOnAnimationFinished;
    }

    public override void Process(Fighter fighter, double delta)
    {
        base.Process(fighter, delta);
        _elapsedTimeSincePunch += delta;
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is WalkCommand { Completed: true }) fighter.SwitchMovementState(new IdleState());
        if (cmd is DuckCommand { Completed: true }) fighter.SwitchMovementState(new IdleState());

        if (cmd is PunchCommand && _elapsedTimeSincePunch >= _punchComboWindow && !_isDuckingPunch)
        {
            _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationPlayerOnAnimationFinished;
            fighter.SwitchCombatState(new PunchTwoState());
        }
        return true;
    }

    public override void OnHit(Fighter fighter, Fighter target)
    {
        if (!_canDealDamage) return;
        
        GodotLogger.LogDebug($"Hit for {fighter.HitDamage}");
        fighter.DealDamage(fighter.HitDamage, target);
        _canDealDamage = false;
    }
    
    private void OnAnimationPlayerOnAnimationFinished(StringName name)
    {
        GodotLogger.LogDebug("Punch animation finished");
        _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationPlayerOnAnimationFinished;
        _fighterRef.SwitchCombatState(null);
    }
}