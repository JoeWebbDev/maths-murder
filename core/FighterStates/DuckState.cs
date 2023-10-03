using System.Threading.Tasks;
using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class DuckState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        base.Enter(fighter);
        
        fighter.AnimationPlayer.Play("duck");
        fighter.CombatStateChanged += OnCombatStateChanged;
    }

    private void OnCombatStateChanged(Fighter fighter)
    {
        if (fighter.CombatState == null) fighter.AnimationPlayer.Play("duck");
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is DuckCommand { Completed: true })
        {
            fighter.SwitchMovementState(new IdleState());
            return false;
        }

        if (cmd is PunchCommand)
        {
            fighter.SwitchCombatState(new PunchState());
        }
        
        return true;
    }

    public async override Task Exit(Fighter fighter)
    {
        base.Exit(fighter);
    }
}