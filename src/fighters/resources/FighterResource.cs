using Godot;

/// <summary>
/// Base resource for all fighter info. The equivalent of a scriptable object. We can store any info about a fighter (sprites, health, etc),
/// and inject into the fighter in scene when we call <see cref="Fighter.LoadFighter"/>.
/// </summary>
public partial class FighterResource : Resource
{
    [Export] public SpriteFrames SpriteFrames { get; set; }
    [Export] public AnimationLibrary AnimationLibrary { get; set; }
    [Export] public int Health { get; set; }
}