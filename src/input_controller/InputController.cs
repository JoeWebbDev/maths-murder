using Godot;
using System;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.Input;

public partial class InputController : Node
{
    [Export] public Fighter CurrentFighter { get; set; }
    
    private bool _dashLeftWindowOpen;
    private bool _dashRightWindowOpen;
    private float _dashWindowTimer;

    public override void _Input(InputEvent @event)
    {
        // Before handling any other inputs, lets see if we can dash
        if (_dashWindowTimer > CurrentFighter.DashDetectPeriod)
        {
            _dashLeftWindowOpen = false;
            _dashRightWindowOpen = false;
        }
        GD.Print($"{_dashWindowTimer}");
        if (@event.IsActionPressed("move_left") && _dashLeftWindowOpen)
        {
            CurrentFighter.Execute(new DashCommand(Vector2.Left));
            _dashLeftWindowOpen = false;
        }
        if (@event.IsActionPressed("move_right") && _dashRightWindowOpen)
        {
            CurrentFighter.Execute(new DashCommand(Vector2.Right));
            _dashRightWindowOpen = false;
        }
        
        // We exit early from any input that isn't a predefined action (see: InputMap on godot docs)
        if (!@event.IsActionType()) return;
        
        // GD.Print($"Received actionable input: {@event.AsText()}");
        
        // UI & non-fighter related actions get handled here

        if (CurrentFighter == null) return;
        
        var fighterCmd = FighterInputActions.TryGetCommand(@event);
        if (fighterCmd != null)
        {
            // We've pressed other buttons, so we can no longer dash.
            _dashLeftWindowOpen = false;
            _dashRightWindowOpen = false;
            CurrentFighter.Execute(fighterCmd);
        }
    }

    public override void _Process(double delta)
    {
        if (_dashLeftWindowOpen || _dashRightWindowOpen) _dashWindowTimer += (float)delta;
        
        // Similarly to movement below, we have to treat block differently. Without this, if you try to block while punching, the command will be missed :/
        // Is there a better way?
        if (Input.IsActionPressed("block"))
        {
            CurrentFighter.Execute(new BlockCommand());
        }
        
        var direction = Vector2.Zero;
        var leftKeyPressed = false;
        var rightKeyPressed = false;
        if (Input.IsActionPressed("move_left"))
        {
            direction += Vector2.Left;
            leftKeyPressed = true;
        }

        if (Input.IsActionPressed("move_right"))
        {
            direction += Vector2.Right;
            rightKeyPressed = true;
        }

        // We have to treat movement differently, as it is not one-shot. We care about getting consistent updates
        // Have split out this if statement for readability
        if (direction != Vector2.Zero)
        {
            // We have movement, so dispatch command
            CurrentFighter.Execute(new WalkCommand(direction));
        }
        else if (leftKeyPressed && rightKeyPressed)
        {
            // We don't have movement because both keys are held, so we should dispatch a "Completed" WalkCommand.
            CurrentFighter.Execute(new WalkCommand(direction, true));
        }
        else if (Input.IsActionJustReleased("move_left"))
        {
            CurrentFighter.Execute(new WalkCommand(direction, true));
            _dashLeftWindowOpen = true;
            _dashWindowTimer = 0f;
        }
        else if (Input.IsActionJustReleased("move_right"))
        {
            CurrentFighter.Execute(new WalkCommand(direction, true));
            _dashRightWindowOpen = true;
            _dashWindowTimer = 0f;
        }
        else
        {
            // None of the above means we were never walking, and so there's no need to dispatch a WalkCommand or a "Completed" WalkCommand.
        }
    }
}
