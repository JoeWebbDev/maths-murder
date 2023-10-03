using MathsMurderSpike.Core.FighterStates;

namespace MathsMurderSpike.core.AI;

/// <summary>
/// Base class for different intensity AI's. The <see cref="AIController"/> will call <see cref="Process"/> every frame,
/// passing in the player's <see cref="Fighter"/> root node. See <see cref="ProperNoobAIStateController"/> for an example implementation.
/// </summary>
public abstract class AIStateController
{
    public abstract void Process(Fighter aiFighter, Fighter playerTarget, double delta);
}