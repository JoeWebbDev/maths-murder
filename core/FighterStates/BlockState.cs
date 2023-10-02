using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class BlockState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        // fighter.AnimatedSprite.Play("block");
        fighter.AnimationPlayer.Play("block");
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is BlockCommand { Completed: true })
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
        
        // Currently not interested in any other commands, as holding block should prevent players from moving.
        // So default return is true
        return true;
    }
}