using Godot;

namespace MathsMurderSpike.core.Commands;

public class DashCommand : FighterCommand
{
    public Vector2 Direction { get; init; }

    public DashCommand(Vector2 direction, bool completed = false) : base(completed)
    {
        Direction = direction;
    }
}