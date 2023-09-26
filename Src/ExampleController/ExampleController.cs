using Godot;
using System;
using System.Linq;

/// <summary>
/// An example of how the rest of the game will interact with <see cref="Fighter"/>. This represents either player input, or AI.
/// Fighter doesn't care what is controlling it, and ExampleController doesn't care how Fighter is executing it's methods.
/// <seealso cref="FighterOne"/>
/// </summary>
public partial class ExampleController : Node
{
    [Export] public Fighter FighterBeingControlled { get; set; }

    public override void _Ready()
    {
        FighterBeingControlled.HitRegistered += OnHitRegistered;
    }

    private void OnHitRegistered()
    {
        GD.Print($"{FighterBeingControlled.Name} has taken a hit!");
        GD.Print($"Current health: {FighterBeingControlled.Health}");
        FighterBeingControlled.Health -= 10;
        GD.Print($"Health after hit: {FighterBeingControlled.Health}");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey eventKey) return;
        var movementKeys = new Key[4] { Key.W, Key.A, Key.S, Key.D };
        
        if (movementKeys.Contains(eventKey.Keycode)) FighterBeingControlled.Move(Vector2.Zero);
        if (eventKey.Keycode == Key.Space) FighterBeingControlled.PrimaryAttack();
    }
}
