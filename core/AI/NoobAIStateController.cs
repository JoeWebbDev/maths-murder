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
public partial class NoobAIStateController : AIStateController
{
    public NoobAIStateController() { }

    public override void Process(Fighter aiFighter, Fighter player, double delta)
    {
        base.Process(aiFighter, player, delta);
    }
}