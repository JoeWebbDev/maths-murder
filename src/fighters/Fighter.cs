using System.Threading.Tasks;
using Godot;
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
    [Export] public int WalkingSpeed { get; private set; } = 100;
    [Export] public int DashMultiplier { get; private set; } = 3;
    
    // The period of time to detect dashes from repeated key presses
    [Export] public float DashDetectPeriod { get; private set; } = 0.3f;
    [Export] public int PlayerNumber { get; private set; }
    [Export] private Node2D _nodeToFlip;
    
    public int Health
    {
        get => _health;
        private set
        {
            var previousHealth = _health;
            _health = value;
            EmitSignal(SignalName.HealthChanged, previousHealth, _health);
            if (_health <= 0)
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
        Health = MaxHealth;
        PunchColliderObject.BodyEntered += OnHit;
        MovementState = new IdleState();
        MovementState.Enter(this);
        // if player 1, mask set to only hit player 2, else if player 2, mask set to hit player 1
        var enemyMask = (uint)(PlayerNumber == 1 ? 2 : 1);
        PunchColliderObject.CollisionMask = enemyMask;
        CollisionMask = enemyMask;
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
    public void TakeDamage(int damage)
    {
        if (CombatState is BlockState blockState)
        {
            // Chip damage etc
            return;
        }
        GodotLogger.LogDebug($"Taking {damage} damage");
        Health -= damage;
        if (Health > 0) SwitchCombatState(new RecoverState());
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