using Godot;
using System;
using MathsMurderSpike.Core.Input;

public partial class InputController : Node
{
    [Export] public Fighter CurrentFighter { get; set; }

    public override void _Input(InputEvent @event)
    {
        // We exit early from any input that isn't a predefined action (see: InputMap on godot docs)
        if (!@event.IsActionType()) return;
        
        GD.Print($"Received actionable input: {@event.AsText()}");
        
        // UI & non-fighter related actions get handled here

        if (CurrentFighter == null) return;
        
        var fighterCmd = FighterInputActions.TryGetCommand(@event);
        if (fighterCmd != null)
        {
            CurrentFighter.Execute(fighterCmd);
        }
    }
}
