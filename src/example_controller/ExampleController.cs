using Godot;
using System;
using System.Linq;
using MathsMurderSpike.core.Commands;

/// <summary>
/// An example of how the rest of the game will interact with <see cref="Fighter"/>. This represents either player input, or AI.
/// Fighter doesn't care what is controlling it, and ExampleController doesn't care how Fighter is executing it's methods.
/// <seealso cref="BasicFighter"/>
/// </summary>
public partial class ExampleController : Node
{
    // [Export] public Fighter FighterBeingControlled { get; set; }
    // [Export] public FighterResource FighterToLoad { get; set; }
    // [Export] public Button LoadButton { get; set; }
    //
    // public override void _Ready()
    // {
    //     FighterBeingControlled.HitRegistered += OnHitRegistered;
    //     LoadButton.Pressed += () => { FighterBeingControlled.LoadFighter(FighterToLoad); };
    // }
    //
    // private void OnHitRegistered()
    // {
    //     GodotLogger.LogDebug($"{FighterBeingControlled.Name} has taken a hit!");
    //     GodotLogger.LogDebug($"Current health: {FighterBeingControlled.Health}");
    //     FighterBeingControlled.Health -= 10;
    //     GodotLogger.LogDebug($"Health after hit: {FighterBeingControlled.Health}");
    // }
    //
    // public override void _Input(InputEvent @event)
    // {
    //     if (@event is not InputEventKey eventKey) return;
    //     
    //     GodotLogger.LogDebug($"Firing an input event: {eventKey.Keycode}, {eventKey.Pressed}");
    //
    //     if (!eventKey.Pressed && eventKey.Keycode is Key.A or Key.D)
    //     {
    //         FighterBeingControlled.Execute(FighterCommand.StopWalk);
    //         return;
    //     }
    //     
    //     if (eventKey.Keycode == Key.A) FighterBeingControlled.Execute(FighterCommand.WalkLeft);
    //     if (eventKey.Keycode == Key.D) FighterBeingControlled.Execute(FighterCommand.WalkRight);
    //     if (eventKey.Keycode == Key.F && !eventKey.Echo && eventKey.Pressed) FighterBeingControlled.Execute(FighterCommand.Punch);
    // }
}
