using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class BlockState : StaminaConsumingState
{
    public override float ProcessStaminaCost { get; set; } = 8f;
    public float SuccessfulBlockStaminaCost { get; set; } = 3f;

    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        fighter.AnimationPlayer.Play(fighter.MovementState is DuckState ? "duck_block" : "block");
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is BlockCommand { Completed: true } || fighter.StaminaDepleted)
        {
            fighter.SwitchCombatState(null);
            return false;
        }

        if (cmd is PunchCommand)
        {
            fighter.SwitchCombatState(new PunchState());
            
            // We are consuming the input so should not pass anything to the Movement FSM.
            return true;
        }

        if (cmd is WalkCommand { Completed: true })
        {
            fighter.SwitchMovementState(new IdleState());
        }

        if (cmd is DuckCommand duckCommand)
        {
            fighter.SwitchMovementState(duckCommand.Completed ? new IdleState() : new DuckState());
            fighter.AnimationPlayer.Play(duckCommand.Completed ? "block" : "duck_block");
        }
        
        // Currently not interested in any other commands, as holding block should prevent players from moving.
        // So default return is true
        return true;
    }
}