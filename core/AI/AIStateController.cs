using System;
using Godot;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.FighterStates;

namespace MathsMurderSpike.core.AI;

/// <summary>
/// Base class for different intensity AI's. The <see cref="AIController"/> will call <see cref="Process"/> every frame,
/// passing in the player's <see cref="Fighter"/> root node. See <see cref="ProperNoobAIStateController"/> for an example implementation.
/// </summary>
public partial class AIStateController : Resource
{
    private const float DefaultFloatingPointTolerance = 1f;
    
    [ExportGroup("Movement")]
    [Export] protected bool TrackPlayer { get; set; } = true;
    [Export] protected float TrackDelay { get; set; } = 1f;
    [Export] protected float PreferredTrackingDistance { get; set; } = 130f;

    [ExportSubgroup("Dash")]
    [Export] protected bool EnableDash { get; set; } = true;
    [Export] protected float DashDelay { get; set; } = 1f;
    [Export] protected float DistanceFromPreferredRequiredToDash { get; set; } = 400f;
    [ExportGroup("Attack")]
    [Export] protected float AttackCooldown { get; set; } = 2f;
    [Export] protected bool AutoAttack { get; set; } = true;
    [Export] protected bool EnableCombos { get; set; } = true;
    [ExportSubgroup("Punch")]
    [Export] protected float PunchRange { get; set; } = 130f;
    [ExportGroup("Block")]
    [Export] protected bool PredictiveBlock { get; set; } = true;

    [Export(PropertyHint.Range, "0,100,1")]
    protected int PredictiveBlockChance { get; set; } = 50;

    protected float CurrentDistanceBetweenFighters { get; set; }
    
    private float _currentAttackCooldown = 0f;
    private float _currentTrackDelay = 0f;
    private float _currentDashDelay = 0f;
    
    public AIStateController() { }

    public virtual void Process(Fighter aiFighter, Fighter playerTarget, double delta)
    {
        CurrentDistanceBetweenFighters = CalculateDistanceBetweenFighters(aiFighter, playerTarget);
        if (AttackOnCooldown()) _currentAttackCooldown -= (float)delta;

        if (playerTarget.CombatState is not null && PredictiveBlock)
        {
            var shouldBlock = GD.Randi() % 100 < PredictiveBlockChance;
            if (shouldBlock)
            {
                aiFighter.Execute(new BlockCommand());
                return;
            }
        }

        if (playerTarget.CombatState is null && aiFighter.CombatState is BlockState)
        {
            aiFighter.Execute(new BlockCommand(true));
        }

        if (TrackPlayer)
        {
            if (!AtPreferredDistance())
            {
                if (EnableDash && TryDash(aiFighter, playerTarget, delta)) return;
                if (TrackingOnCooldown())
                {
                    _currentTrackDelay -= (float)delta;
                    return;
                }
                MoveTowardsPreferredDistance(aiFighter, playerTarget);
                _currentTrackDelay = TrackDelay;
                return;
            }
            if (aiFighter.MovementState is WalkingState) aiFighter.Execute(new WalkCommand(Vector2.Zero, true));
        }
        
        if (!AutoAttack) return;
        // If EnableCombo, we only bomb out of attacks if there's no combo opportunity (CombatState is null)
        if (AttackOnCooldown() && !EnableCombos || AttackOnCooldown() && aiFighter.CombatState == null) return;
        // Atm, only supporting melee attacks. 
        if (!InPunchRange()) return;
        aiFighter.Execute(new PunchCommand());
        _currentAttackCooldown = AttackCooldown;
    }

    protected bool TryDash(Fighter aiFighter, Fighter playerTarget, double delta)
    {
        if (DashOnCooldown())
        {
            _currentDashDelay -= (float)delta;
            return false;
        }

        if (!IsDashViable()) return false;
        
        aiFighter.Execute(new DashCommand(GetDirectionToMove(aiFighter, playerTarget)));
        _currentDashDelay = DashDelay;
        return true;
    }

    protected float CalculateDistanceBetweenFighters(Fighter aiFighter, Fighter player)
    {
        var playerXPos = player.GlobalPosition.X;
        var aiXPos = aiFighter.GlobalPosition.X;
        return Mathf.Abs(playerXPos - aiXPos);
    }

    protected Vector2 GetDirectionToMove(Fighter aiFighter, Fighter player)
    {
        var directionTowardsPlayer = aiFighter.GlobalPosition.X > player.GlobalPosition.X ? Vector2.Left : Vector2.Right;
        return CurrentDistanceBetweenFighters > PreferredTrackingDistance ? directionTowardsPlayer : directionTowardsPlayer * -1;
    }

    protected void MoveTowardsPreferredDistance(Fighter aiFighter, Fighter player)
    {
        var directionToMove = GetDirectionToMove(aiFighter, player);
        aiFighter.Execute(new WalkCommand(directionToMove));
    }

    protected bool AtPreferredDistance() => Math.Abs(CurrentDistanceBetweenFighters - PreferredTrackingDistance) < DefaultFloatingPointTolerance;
    protected bool IsDashViable() => CurrentDistanceBetweenFighters > PreferredTrackingDistance + DistanceFromPreferredRequiredToDash;
    protected bool InPunchRange() => PunchRange > CurrentDistanceBetweenFighters;
    protected bool AttackOnCooldown() => _currentAttackCooldown > 0;
    protected bool TrackingOnCooldown() => _currentTrackDelay > 0;
    protected bool DashOnCooldown() => _currentDashDelay > 0;
}