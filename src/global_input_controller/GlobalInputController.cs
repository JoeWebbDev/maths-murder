using Godot;
using System;

public partial class GlobalInputController : Node
{
    [Signal] public delegate void OpenPauseMenuRequestedEventHandler();
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            EmitSignal(SignalName.OpenPauseMenuRequested);
        }
    }
}
