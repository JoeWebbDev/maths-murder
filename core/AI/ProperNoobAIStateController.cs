using Godot;
using MathsMurderSpike.core.Commands;
using MonoCustomResourceRegistry;

namespace MathsMurderSpike.core.AI;

/// <summary>
/// A very basic implementation. Currently does the same as what the <see cref="AIController"/> used to do
/// </summary>
public partial class ProperNoobAIStateController : AIStateController
{
    private double _timer = 0;
    
    public ProperNoobAIStateController() { }
    public override void Process(Fighter aiFighter, Fighter playerTarget, double delta)
    {
        _timer += delta;
        if (_timer < 2) return;
        
        GodotLogger.LogDebug("Executing punch.");
        aiFighter.Execute(new PunchCommand());
        _timer = 0;
    }
}