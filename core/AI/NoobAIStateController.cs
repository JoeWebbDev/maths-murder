using System;
using Godot;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.FighterStates;

namespace MathsMurderSpike.core.AI;

/// <summary>
/// A slightly more involved <see cref="AIStateController"/> than <see cref="ProperNoobAIStateController"/>.
/// Issues walk commands until within <see cref="_distanceThreshold"/> of the player, and then punches.
/// All magic numbers & not the best implementation; just wanted to demonstrate how we can write up more advanced AI's
/// using the existing state machine!
/// </summary>
public class NoobAIStateController : AIStateController
{
    private float _distanceThreshold = 150f;
    private float _distanceThresholdFloatingPointTolerance = 0.1f;
    private float _punchCooldown = 5f;
    private double _currentPunchCooldown = 5f;
    public override void Process(Fighter aiFighter, Fighter player, double delta)
    {
        var inPunchRange = TrackPlayer(aiFighter, player, delta);

        if (_currentPunchCooldown >= _punchCooldown && inPunchRange) Punch(aiFighter);
        _currentPunchCooldown += delta;
    }

    private void Punch(Fighter aiFighter)
    {
        aiFighter.Execute(new PunchCommand());
        _currentPunchCooldown = 0f;
    }

    private bool TrackPlayer(Fighter aiFighter, Fighter player, double delta)
    {
        var playerXPos = player.GlobalPosition.X;
        var aiXPos = aiFighter.GlobalPosition.X;
        var distance = Mathf.Abs(playerXPos - aiXPos);
        var directionTowardsPlayer = aiXPos > playerXPos ? Vector2.Left : Vector2.Right;
        if (distance > _distanceThreshold)
        {
            aiFighter.Execute(new WalkCommand(directionTowardsPlayer));
        }
        else
        {
            var inverseDirection = directionTowardsPlayer * -1;
            var shouldNotWalkAwayFromPlayer = Math.Abs(distance - _distanceThreshold) < _distanceThresholdFloatingPointTolerance;
            aiFighter.Execute(new WalkCommand(inverseDirection, shouldNotWalkAwayFromPlayer));
            return true;
        }

        return false;
    }
}