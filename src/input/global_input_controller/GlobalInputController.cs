using Godot;
using System;

/// <summary>
/// Now unused following a previous implementation of the pause menu. But perhaps we should
/// keep it in for now as we may want some way to register input outside of a fight?
/// </summary>
public partial class GlobalInputController : Node
{
    public override void _Input(InputEvent @event)
    {
    }
}
