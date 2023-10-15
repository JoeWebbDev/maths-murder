using System.Threading.Tasks;
using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class RecoverState : StaminaConsumingState
{
    private Fighter _fighterRef;

    public override float EnterStaminaCost { get; set; } = 5f;
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        _fighterRef = fighter;
        fighter.AnimationPlayer.Play("recover");
        fighter.AnimationPlayer.Seek(0);
        fighter.AnimationPlayer.AnimationFinished += OnAnimationFinish;
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is WalkCommand { Completed: true }) fighter.SwitchMovementState(new IdleState());
        if (cmd is DuckCommand { Completed: true }) fighter.SwitchMovementState(new IdleState());

        // We are busy recovering, so we consume all commands
        return true;
    }

    public override Task Exit(Fighter fighter)
    {
        return base.Exit(fighter);
        _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationFinish;
    }

    private void OnAnimationFinish(StringName animName)
    {
        _fighterRef.AnimationPlayer.AnimationFinished -= OnAnimationFinish;
        _fighterRef.SwitchCombatState(null);
    }
}