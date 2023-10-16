using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class PunchTwoState : StaminaConsumingState
{
    private float _punchComboWindow = 0.3f;
    private double _elapsedTimeSincePunch;
    private Fighter _fighterRef;

    public override float EnterStaminaCost { get; set; } = 8f;
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        _fighterRef = fighter;
        fighter.AnimationPlayer.Play("punch_two");
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

        if (cmd is PunchCommand && _elapsedTimeSincePunch >= _punchComboWindow)
        {
            _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationPlayerOnAnimationFinished;
            fighter.SwitchCombatState(new PunchThreeState());
            return true;
        }
        
        if (cmd is KickCommand && _elapsedTimeSincePunch >= _punchComboWindow)
        {
            _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationPlayerOnAnimationFinished;
            fighter.SwitchCombatState(new HighKickState());
        }
        
        return true;
    }

    public override void OnHit(Fighter fighter, Fighter target)
    {
        GodotLogger.LogDebug($"Second punch hit for {fighter.HitDamage + 1}");
        fighter.DealDamage(fighter.HitDamage + 1, target);
    }
    
    private void OnAnimationPlayerOnAnimationFinished(StringName name)
    {
        GodotLogger.LogDebug("Animation finished");
        _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationPlayerOnAnimationFinished;
        _fighterRef.SwitchCombatState(null);
    }
}