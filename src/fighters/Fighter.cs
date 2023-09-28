using Godot;
using System;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.FighterStates;

public partial class Fighter : Area2D
{
    private int _health;
    [Signal] public delegate void HitRegisteredEventHandler();
    [Signal] public delegate void HealthChangedEventHandler(int from, int to);
    [Signal] public delegate void MovementStateChangedEventHandler(Fighter fighter);
    [Signal] public delegate void CombatStateChangedEventHandler(Fighter fighter);
    [Export] public AnimatedSprite2D AnimatedSprite { get; set; }
    [Export] public bool FlipH { get; set; }
    [Export] public int MaxHealth { get; private set; }

    public int Health
    {
        get => _health;
        set
        {
            var previousHealth = _health;
            _health = value;
            EmitSignal(SignalName.HealthChanged, previousHealth, _health);
        }
    }
    public FighterState MovementState { get; private set; }
    public FighterState CombatState { get; private set; }
    
    public override void _Ready()
    {
        Health = MaxHealth;
        AnimatedSprite.FlipH = FlipH;
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
        MaxHealth = resource.Health;
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