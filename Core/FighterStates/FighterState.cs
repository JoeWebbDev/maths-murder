namespace MathsMurderSpike.Core.FighterStates;

// Going with abstract class > interface for now. We may want some common functionality between all states down the line!
public abstract class FighterState
{
    public abstract void Enter(Fighter fighter);
    public abstract void HandleInput(Fighter fighter, object input); // object for now since I'm not sure what input looks like in godot!
    public abstract void Process(Fighter fighter, double delta);
    public abstract void Exit(Fighter fighter);
}