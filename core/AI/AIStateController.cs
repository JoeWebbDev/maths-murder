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
    [Export] protected float PreferredTrackingDistance { get; set; } = 75f;

    [ExportSubgroup("Dash")]
    [Export] protected bool EnableDash { get; set; } = true;
    [Export] protected float DashDelay { get; set; } = 1f;
    [Export] protected float DistanceFromPreferredRequiredToDash { get; set; } = 150f;

    [ExportSubgroup("Duck")]
    [Export] protected bool EnableDuck { get; set; } = true;
    [Export] protected float DuckDelay { get; set; } = 0.2f;
    [ExportGroup("Attack")]
    [Export] protected float AttackCooldown { get; set; } = 2f;
    [Export] protected float AttackRange { get; set; } = 75f;
    [Export] protected bool AutoAttack { get; set; } = true;
    [Export] protected bool EnableCombos { get; set; } = true;
    [ExportSubgroup("Kick")]
    [Export] protected bool EnableKick { get; set; } = true;
    [Export(PropertyHint.Range, "0,100,1")]
    protected int KickChance { get; set; } = 25;
    [ExportGroup("Block")]
    [Export] protected bool PredictiveBlock { get; set; } = true;
    [Export(PropertyHint.Range, "0,100,1")]
    protected int PredictiveBlockChance { get; set; } = 50;
    [Export] protected bool EnableSpamDenial { get; set; } = true;
    [Export] protected int SpamAllowedBeforeDenial { get; set; } = 3;

    protected float CurrentDistanceBetweenFighters { get; set; }

    private float _currentDuckDelay = 0f;
    private float _currentAttackCooldown = 0f;
    private float _currentTrackDelay = 0f;
    private float _currentDashDelay = 0f;
    private int _timesPlayerHasLandedConsecutiveCombos = 0;
    private bool _blockConsumed = false;
    
    public AIStateController() { }

    public virtual void Process(Fighter aiFighter, Fighter playerTarget, double delta)
    {
        CurrentDistanceBetweenFighters = CalculateDistanceBetweenFighters(aiFighter, playerTarget);
        if (AttackOnCooldown()) _currentAttackCooldown -= (float)delta;
        
        if (EnableSpamDenial)
        {
            if (playerTarget.CombatState is PunchThreeState or HighKickState)
                _timesPlayerHasLandedConsecutiveCombos += 1;
        }

        if (EnableDuck && playerTarget.MovementState is DuckState && InAttackRange())
        {
            _currentDuckDelay += (float)delta;
            if (_currentDuckDelay >= DuckDelay)
            {
                aiFighter.Execute(new DuckCommand());
            }
        }
        else
        {
            aiFighter.Execute(new DuckCommand(true));
            _currentDuckDelay = 0f;
        }

        if (playerTarget.CombatState is not null and not BlockState && PredictiveBlock && InAttackRange())
        {
            if (EnableSpamDenial && _timesPlayerHasLandedConsecutiveCombos >= SpamAllowedBeforeDenial)
            {
                aiFighter.Execute(new BlockCommand());
            }
            else
            {
                if (_blockConsumed) return;
                var randomNum = GD.Randi() % 100;
                var shouldBlock = randomNum < PredictiveBlockChance;
                GodotLogger.LogDebug(randomNum.ToString());
                if (shouldBlock)
                {
                    aiFighter.Execute(new BlockCommand());
                    return;
                }

                _blockConsumed = true;
                return;
            }
        }
        
        if (playerTarget.CombatState is null or BlockState) _blockConsumed = false;

        if (playerTarget.CombatState is null or BlockState && aiFighter.CombatState is BlockState)
        {
            aiFighter.Execute(new BlockCommand(true));
            if (_timesPlayerHasLandedConsecutiveCombos >= SpamAllowedBeforeDenial)
            {
                aiFighter.Execute(new PunchCommand());
                _timesPlayerHasLandedConsecutiveCombos = 0;
                return;
            }
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
        if (AttackOnCooldown() && !EnableCombos || AttackOnCooldown() && aiFighter.CombatState is null) return;
        // Atm, only supporting melee attacks. 
        if (!InAttackRange()) return;
        FighterCommand attackCommand = EnableKick && GD.Randi() % 100 < KickChance ? new KickCommand() : new PunchCommand();
        // Block high kicks because of funky rotation issues
        if (aiFighter.CombatState is PunchTwoState) attackCommand = new PunchCommand();
        aiFighter.Execute(attackCommand);
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
    protected bool IsDashViable() => CurrentDistanceBetweenFighters + DefaultFloatingPointTolerance >= PreferredTrackingDistance + DistanceFromPreferredRequiredToDash;
    protected bool InAttackRange() => AttackRange + DefaultFloatingPointTolerance >= CurrentDistanceBetweenFighters;
    protected bool AttackOnCooldown() => _currentAttackCooldown > 0;
    protected bool TrackingOnCooldown() => _currentTrackDelay > 0;
    protected bool DashOnCooldown() => _currentDashDelay > 0;
}