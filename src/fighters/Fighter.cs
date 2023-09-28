using Godot;
using System;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.FighterStates;

public partial class Fighter : Area2D
{
    [Signal] public delegate void HitRegisteredEventHandler();
    public int Health { get; set; }
    [Export] public AnimatedSprite2D AnimatedSprite { get; set; }
    public FighterState MovementState { get; private set; }
    public FighterState CombatState { get; private set; }
    [Signal] public delegate void MovementStateChangedEventHandler(Fighter fighter);
    [Signal] public delegate void CombatStateChangedEventHandler(Fighter fighter);
    
    public override void _Ready()
    {
        MovementState = new IdleState();
        MovementState.Enter(this);
    }

    public override void _Process(double delta)
    {
        MovementState?.Process(this, delta);
        CombatState?.Process(this, delta);
    }

    public void Execute(FighterCommand cmd)
    {
        GD.Print($"Command received: {cmd.GetType()}");
        // I found that I was writing lots of "if (doing some combat) then do nothing" in the movement states.
        // This provides a way for the CombatState machine to consume commands, so that they never reach
        // the MovementState FSM (which should largely be locked when mid-combat anyway).
        var cmdConsumed = CombatState?.HandleCommand(this, cmd) ?? false;
        if (!cmdConsumed) MovementState?.HandleCommand(this, cmd);
    }

    public void LoadFighter(FighterResource resource)
    {
        GD.Print($"Loading fighter from resource: {resource.ResourcePath}");
        Health = resource.Health;
        AnimatedSprite.SpriteFrames = resource.SpriteFrames;
        MovementState = new IdleState();
        MovementState.Enter(this);
        GD.Print($"Fighter loaded! Health: {Health}");
    }

    public void SwitchMovementState(FighterState to)
    {
        GD.Print($"Switching movement state from: {MovementState?.GetType()} to {to?.GetType()}");
        MovementState?.Exit(this);
        MovementState = to;
        to?.Enter(this);
        EmitSignal(SignalName.MovementStateChanged, this);
    }

    public void SwitchCombatState(FighterState to)
    {
        GD.Print($"Switching combat state from: {CombatState?.GetType()} to {to?.GetType()}");
        CombatState?.Exit(this);
        CombatState = to;
        to?.Enter(this);
        EmitSignal(SignalName.CombatStateChanged, this);
    }
}