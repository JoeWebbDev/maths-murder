using System.Threading.Tasks;
using Godot;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.FighterStates;

public partial class Fighter : CharacterBody2D
{
    private float _currentHealth;
    [Signal] public delegate void HitRegisteredEventHandler();
    [Signal] public delegate void HealthChangedEventHandler(int from, int to);
    [Signal] public delegate void MovementStateChangedEventHandler(Fighter fighter);
    [Signal] public delegate void CombatStateChangedEventHandler(Fighter fighter);
    [Export] public AnimatedSprite2D AnimatedSprite { get; set; }
    [Export] public Sprite2D NumberRef { get; set; }
    [Export] public AnimationPlayer AnimationPlayer { get; set; }
    [Export] public Area2D PunchColliderObject { get; set; }
    [Export] public bool FlipH { get; set; }
    [Export] public float TotalHealth { get; private set; }
    [Export] public float HitDamage { get; private set; }
    [Export] public float DamageReduction { get; private set; }
    [Export] public float WalkingSpeed { get; private set; } = 100;
    [Export] public StatScalingConfig StatScaling; 
        
    // The period of time to detect dashes from repeated key presses
    [Export] public float DashDetectPeriod { get; private set; } = 0.3f;
    [Export] public int PlayerNumber { get; private set; }
    [Export] private Node2D _nodeToFlip;
    private Node2D _armsGroup;
    private Node2D _legsGroup;
    private Node2D _spriteGroup;
    
    
    public float CurrentHealth
    {
        get => _currentHealth;
        private set
        {
            var previousHealth = _currentHealth;
            _currentHealth = value;
            EmitSignal(SignalName.HealthChanged, previousHealth, _currentHealth);
            if (_currentHealth <= 0)
            {
                // Should emit another signal here & maybe take the CheckDeath code out of Fight.cs?
                SwitchCombatState(new DeathState());
            }
        }
    }
    public FighterState MovementState { get; private set; }
    public FighterState CombatState { get; private set; }
    
    public override void _Ready()
    {
        if (FlipH)
            _nodeToFlip.Scale = new Vector2(-_nodeToFlip.Scale.X, _nodeToFlip.Scale.Y);
        PunchColliderObject.BodyEntered += OnHit;
        MovementState = new IdleState();
        MovementState.Enter(this);
        // if player 1, mask set to only hit player 2, else if player 2, mask set to hit player 1
        var enemyMask = (uint)(PlayerNumber == 1 ? 2 : 1);
        PunchColliderObject.CollisionMask = enemyMask;
        CollisionMask = enemyMask;
        _spriteGroup = GetNode<Node2D>("ScalableChildren/SpriteGroup");
        _armsGroup = GetNode<Node2D>("ScalableChildren/ArmsGroup");
        _legsGroup = GetNode<Node2D>("ScalableChildren/LegsGroup");
        
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
        TotalHealth = resource.Health;
        CurrentHealth = resource.Health;
        AnimatedSprite.SpriteFrames = resource.SpriteFrames;
        MovementState = new IdleState();
        MovementState.Enter(this);
        GodotLogger.LogDebug($"Fighter loaded! Health: {CurrentHealth}");
    }

    public void InitFighter(FighterData data)
    {
        // Load texture depending on level
        // Set stats using scaling config.
        TotalHealth = data.Health * StatScaling.HealthModifier;
        CurrentHealth = data.Health * StatScaling.HealthModifier;
        HitDamage = data.Strength * StatScaling.DamageModifier;
        DamageReduction = data.Defense * StatScaling.DamageReductionModifier;
        WalkingSpeed = WalkingSpeed + data.Speed * StatScaling.SpeedModifier;
        
        // Need to separate numbers from fighter data
        var spriteData = data.GetSpriteData();
        NumberRef.Texture = spriteData.NumberTexture;
        _armsGroup.Position += spriteData.ArmsOffset;
        _legsGroup.Position += spriteData.LegsOffset;
        Position += spriteData.BodyOffset;
        _spriteGroup.Position += spriteData.SpriteOffset;
        GodotLogger.LogDebug($"Fighter {PlayerNumber} TotalHealth: {TotalHealth}");
        GodotLogger.LogDebug($"Fighter {PlayerNumber} HitDamage: {HitDamage}");
        GodotLogger.LogDebug($"Fighter {PlayerNumber} DamageReduction: {DamageReduction}");
        GodotLogger.LogDebug($"Fighter {PlayerNumber} WalkingSpeed: {WalkingSpeed}");
    }

    public async void SwitchMovementState(FighterState to)
    {
        GodotLogger.LogDebug($"Switching movement state from: {MovementState?.GetType()} to {to?.GetType()}");
        await (MovementState?.Exit(this) ?? Task.CompletedTask);
        MovementState = to;
        to?.Enter(this);
        EmitSignal(SignalName.MovementStateChanged, this);
    }

    public async void SwitchCombatState(FighterState to)
    {
        GodotLogger.LogDebug($"Switching combat state from: {CombatState?.GetType()} to {to?.GetType()}");
        await (CombatState?.Exit(this) ?? Task.CompletedTask);
        CombatState = to;
        to?.Enter(this);
        EmitSignal(SignalName.CombatStateChanged, this);
    }
    
    // We're doing a crude check here to see if we're in a block state, and if not, take damage & enter RecoverState.
    // Ideally, this is deferred to the state machine too; but that seems like over-engineering given that only the
    // BlockState will have any impact on what happens when damage is received
    public void TakeDamage(float damage)
    {
        if (CombatState is BlockState blockState)
        {
            // Chip damage etc
            return;
        }
        GodotLogger.LogDebug($"Taking {damage} damage");
        CurrentHealth -= damage * (20/(20 * DamageReduction)) ;
        if (CurrentHealth > 0) SwitchCombatState(new RecoverState());
    }
    
    // Moving functionality down to individual states - will come in handy in the future when we want
    // different combat states to have more granular control, i.e. deal differing amounts of damage
    // Also, should leave it to the fighter receiving the hit to determine what happens (see above).
    // This will make it cleaner to implement chip damage etc that might be reliant on the stats of the Fighter
    // being hit
    private void OnHit(Node2D body)
    {
        if (body is not Fighter enemy) 
            return;
        
        // Don't need to pass to movement FSM as movement shouldn't be dealing damage
        CombatState?.OnHit(this, enemy);
    }
}