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
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        if (cmd is DuckCommand { Completed: true })
        {
            fighter.SwitchMovementState(new IdleState());
            return false;
        }
        // Code to handle punching while ducking etc goes here!

        return true;
    }

    public async override Task Exit(Fighter fighter)
    {
        base.Exit(fighter);
        fighter.AnimationPlayer.Play("RESET");
        await fighter.ToSignal(fighter.AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
    }
}