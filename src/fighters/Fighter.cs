using Godot;
using System;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.FighterStates;

public partial class Fighter : CharacterBody2D
{
    private int _health;
    [Signal] public delegate void HitRegisteredEventHandler();
    [Signal] public delegate void HealthChangedEventHandler(int from, int to);
    [Signal] public delegate void MovementStateChangedEventHandler(Fighter fighter);
    [Signal] public delegate void CombatStateChangedEventHandler(Fighter fighter);
    [Export] public AnimatedSprite2D AnimatedSprite { get; set; }
    [Export] public AnimationPlayer AnimationPlayer { get; set; }
    [Export] public Area2D PunchColliderObject { get; set; }
    [Export] public bool FlipH { get; set; }
    [Export] public int MaxHealth { get; private set; }
    [Export] public int HitDamage { get; private set; }
    [Export] public int PlayerNumber { get; private set; }
    
    private bool _canDealDamage = true;

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
        if (FlipH)
            Scale = new Vector2(-Scale.X, Scale.Y);
        Health = MaxHealth;
        PunchColliderObject.BodyEntered += OnHit;
        MovementState = new IdleState();
        MovementState.Enter(this);
        // if player 1, mask set to only hit player 2, else if player 2, mask set to hit player 1
        var enemyMask = (uint)(PlayerNumber == 1 ? 2 : 1);
        PunchColliderObject.CollisionMask = enemyMask;
        CollisionMask = enemyMask;
        AnimationPlayer.AnimationFinished += OnAnimFinished;
    }

    private void OnAnimFinished(StringName animName)
    {
        if (animName == "fighter_anim_lib/punch")
            _canDealDamage = true;
    }

    public override void _Process(double delta)
    {
        MovementState?.Process(this, delta);
        CombatState?.Process(this, delta);
    }

    public void Execute(FighterCommand cmd)
    {
        GodotLogger.LogDebug($"Command received: {cmd.GetType()}");
        // I found that I was writing lots of "if (doing some combat) then do nothing" in the movement states.
        // This provides a way for the CombatState machine to consume commands, so that they never reach
        // the MovementState FSM (which should largely be locked when mid-combat anyway).
        var cmdConsumed = CombatState?.HandleCommand(this, cmd) ?? false;
        if (!cmdConsumed) MovementState?.HandleCommand(this, cmd);
    }

    public void LoadFighter(FighterResource resource)
    {
        GodotLogger.LogDebug($"Loading fighter from resource: {resource.ResourcePath}");
        MaxHealth = resource.Health;
        Health = resource.Health;
        AnimatedSprite.SpriteFrames = resource.SpriteFrames;
        MovementState = new IdleState();
        MovementState.Enter(this);
        GodotLogger.LogDebug($"Fighter loaded! Health: {Health}");
    }

    public void SwitchMovementState(FighterState to)
    {
        GodotLogger.LogDebug($"Switching movement state from: {MovementState?.GetType()} to {to?.GetType()}");
        MovementState?.Exit(this);
        MovementState = to;
        to?.Enter(this);
        EmitSignal(SignalName.MovementStateChanged, this);
    }

    public void SwitchCombatState(FighterState to)
    {
        GodotLogger.LogDebug($"Switching combat state from: {CombatState?.GetType()} to {to?.GetType()}");
        CombatState?.Exit(this);
        CombatState = to;
        to?.Enter(this);
        EmitSignal(SignalName.CombatStateChanged, this);
    }

    private void OnHit(Node2D body)
    {
        if (!_canDealDamage)
            return;
        
        GodotLogger.LogDebug($"Hit for {HitDamage}");
        if (body is Fighter enemy)
        {
            if (enemy.CombatState is BlockState)
                return;
            
            enemy.TakeDamage(HitDamage);
            _canDealDamage = false;
        }
    }

    private void TakeDamage(int damage)
    {
        GodotLogger.LogDebug($"Taking {damage} damage");
        Health -= damage;
    }
}