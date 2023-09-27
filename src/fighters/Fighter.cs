using Godot;
using System;
using MathsMurderSpike.Core.enums;
using MathsMurderSpike.Core.FighterStates;

public partial class Fighter : Area2D
{
    [Signal] public delegate void HitRegisteredEventHandler();
    public int Health { get; set; }
    [Export] public AnimatedSprite2D AnimatedSprite { get; set; }

    public FighterState MovementState { get; private set; }
    public FighterState CombatState { get; private set; }

    public override void _Process(double delta)
    {
        MovementState?.Process(this, delta);
        CombatState?.Process(this, delta);
    }

    public void Execute(FighterCommand cmd)
    {
        CombatState?.HandleCommand(this, cmd);
        MovementState?.HandleCommand(this, cmd);
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
    }

    public void SwitchCombatState(FighterState to)
    {
        GD.Print($"Switching combat state from: {CombatState?.GetType()} to {to?.GetType()}");
        CombatState?.Exit(this);
        CombatState = to;
        to?.Enter(this);
    }
}