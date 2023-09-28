using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

public class IdleState : FighterState
{
    public override void Enter(Fighter fighter)
    {
        if (fighter.CombatState == null) fighter.AnimatedSprite.Play("idle");
        fighter.CombatStateChanged += OnCombatStateChanged;
    }

    private void OnCombatStateChanged(Fighter fighter)
    {
        if (fighter.CombatState == null) fighter.AnimatedSprite.Play("idle");
    }

    public override bool HandleCommand(Fighter fighter, FighterCommand cmd)
    {
        switch (cmd)
        {
            case WalkCommand walkCommand:
                if (walkCommand.Completed) break;
                fighter.SwitchMovementState(new WalkingState(cmd));
                break;
            case PunchCommand:
                fighter.SwitchCombatState(new PunchState());
                break;
            case BlockCommand blockCommand:
                if (blockCommand.Completed) break;
                // Block should be in the CombatFSM, since we want to lock most movement when blocking, and we can never block and execute other combat moves simultaneously
                fighter.SwitchCombatState(new BlockState());
                break;
        }

        return false;
    }

    public override void Exit(Fighter fighter)
    {
        fighter.CombatStateChanged -= OnCombatStateChanged;
    }
}