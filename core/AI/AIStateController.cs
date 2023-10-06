using Godot;

namespace MathsMurderSpike.core.AI;

/// <summary>
/// Base class for different intensity AI's. The <see cref="AIController"/> will call <see cref="Process"/> every frame,
/// passing in the player's <see cref="Fighter"/> root node. See <see cref="ProperNoobAIStateController"/> for an example implementation.
/// </summary>
public partial class AIStateController : Resource
{
    public AIStateController() { }

    public virtual void Process(Fighter aiFighter, Fighter playerTarget, double delta) { }
}