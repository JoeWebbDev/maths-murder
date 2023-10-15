using System.Threading.Tasks;
using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class DashState : StaminaConsumingState
{
    private Vector2 _direction;

    public override float EnterStaminaCost { get; set; } = 10f;

    public DashState(FighterCommand cmd)
    {
        if (cmd is not DashCommand dashCommand)
        {
            GD.PushError("Dash state being initialised with non-dash command. Something is wrong");
            return;
        }

        _direction = dashCommand.Direction;
    }
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        
        fighter.AnimationPlayer.Play("dash");
        
        void OnAnimationFinished(StringName name)
        {
            fighter.AnimationPlayer.AnimationFinished -= OnAnimationFinished;
            fighter.SwitchMovementState(new IdleState());
        }

        fighter.AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        // We'll be in the movement machine, so doesn't really matter what we return here
        return false;
    }

    public override void Process(Fighter fighter, double delta)
    {
        
        if (fighter.CombatState != null) return;

        var vec = _direction * fighter.WalkingSpeed * fighter.StatScaling.DashMultiplier * (float)delta;
        fighter.MoveAndCollide(vec);
    }

    public override async Task Exit(Fighter fighter)
    {
        base.Exit(fighter);
    }
}