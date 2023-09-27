using MathsMurderSpike.Core.enums;

namespace MathsMurderSpike.Core.FighterStates;

// Going with abstract class > interface for now. We may want some common functionality between all states down the line!
public abstract class FighterState
{
    public abstract void Enter(Fighter fighter);
    public abstract void HandleCommand(Fighter fighter, FighterCommand cmd);

    public virtual void Process(Fighter fighter, double delta) { }

    public virtual void Exit(Fighter fighter) { }
}