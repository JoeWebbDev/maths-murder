using Godot;
using System;

/// <summary>
/// An example root node class extending from <see cref="Fighter"/>.
/// </summary>
public partial class FighterOne : Fighter
{
    public override void Move(Vector2 location)
    {
        GD.Print($"Moving to: {location}. This is where I play my own special animation, and do my own special thing when I move.");
    }

    public override void PrimaryAttack()
    {
        GD.Print("Throwing fists! This is a complete different attack to a different character (FighterTwo), but only I care about how that is executed.");
        EmitSignal(Fighter.SignalName.HitRegistered);
    }
}