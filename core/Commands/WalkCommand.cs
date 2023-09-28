using Godot;

namespace MathsMurderSpike.core.Commands;

public class WalkCommand : FighterCommand
{
    public Vector2 Direction { get; init; }

    public WalkCommand(Vector2 direction, bool completed = false) : base(completed)
    {
        Direction = direction;
    }
}