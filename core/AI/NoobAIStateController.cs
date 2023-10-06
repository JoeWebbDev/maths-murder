using System;
using Godot;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.FighterStates;
using MonoCustomResourceRegistry;

namespace MathsMurderSpike.core.AI;

/// <summary>
/// A slightly more involved <see cref="AIStateController"/> than <see cref="ProperNoobAIStateController"/>.
/// Issues walk commands until within <see cref="_punchRange"/> of the player, and then punches.
/// All magic numbers & not the best implementation; just wanted to demonstrate how we can write up more advanced AI's
/// using the existing state machine!
/// </summary>
[RegisteredType(nameof(NoobAIStateController), "", nameof(Resource))]
public partial class NoobAIStateController : AIStateController
{
    private float _punchRange = 140f;
    private float _punchRangeTolerance = 1f;
    private float _punchCooldown = 5f;
    private double _currentPunchCooldown = 5f;
    private float _moveDelay = 1f;
    private double _currentMoveDelay = 1f;
    
    public NoobAIStateController() { }
    public override void Process(Fighter aiFighter, Fighter player, double delta)
    {
        _currentPunchCooldown += delta;
        
        if (!AtPunchRange(aiFighter, player))
        {
            if (_currentMoveDelay < _moveDelay)
            {
                _currentMoveDelay += delta;
                return;
            }
            MoveTowardsPunchRange(aiFighter, player, delta);
            _currentMoveDelay = 0f;
            return;
        }
        if (aiFighter.MovementState is WalkingState) aiFighter.Execute(new WalkCommand(Vector2.Zero, true));

        if (_currentPunchCooldown > _punchCooldown) Punch(aiFighter);
    }

    private void Punch(Fighter aiFighter)
    {
        aiFighter.Execute(new PunchCommand());
        _currentPunchCooldown = 0f;
    }

    private void MoveTowardsPunchRange(Fighter aiFighter, Fighter player, double delta)
    {
        var distance = Mathf.Abs(player.GlobalPosition.X - aiFighter.GlobalPosition.X);
        var directionTowardsPlayer = aiFighter.GlobalPosition.X > player.GlobalPosition.X ? Vector2.Left : Vector2.Right;
        var directionToMove = distance > _punchRange ? directionTowardsPlayer : directionTowardsPlayer * -1;
        
        aiFighter.Execute(new WalkCommand(directionToMove));
    }

    private bool AtPunchRange(Fighter aiFighter, Fighter player)
    {
        var playerXPos = player.GlobalPosition.X;
        var aiXPos = aiFighter.GlobalPosition.X;
        var distance = Mathf.Abs(playerXPos - aiXPos);
        return Math.Abs(distance - _punchRange) < _punchRangeTolerance;
    }
}