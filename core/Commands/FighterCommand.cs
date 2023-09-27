namespace MathsMurderSpike.core.Commands;

public abstract class FighterCommand
{
    public bool Completed { get; init; }

    public FighterCommand(bool completed = false)
    {
        Completed = completed;
    }
}