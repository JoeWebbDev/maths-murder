using Godot;
using System;
using MathsMurderSpike.Core.FighterStates;

/// <summary>
/// Base class for all fighters in the game. Provides a way for controlling nodes (e.g. <see cref="ExampleController"/>) to access signals, properties and methods
/// that will be shared amongst all fighters.
/// <para></para>
/// This kind of structure is beneficial in that we can:
/// <list type="bullet">
/// <item>
/// <description>Create root nodes that inherit from this, giving each character control over what its moves do</description>
/// </item>
/// <item>
/// <description>Defer firing these actions to a parent, controlling class.</description>
/// </item>
/// </list>
/// <para></para>
/// The main drawback is we will have to create new scenes for each fighter.
/// <seealso cref="BasicFighter"/>
/// </summary>
public partial class Fighter : Area2D
{
    [Signal] public delegate void HitRegisteredEventHandler();
    public int Health { get; set; }
    [Export] public AnimatedSprite2D AnimatedSprite { get; set; }
    
    public FighterState MovementState { get; private set; }
    public FighterState CombatState { get; private set; }

    public virtual void Move(Vector2 location) { }

    public virtual void PrimaryAttack() { }

    public void LoadFighter(FighterResource resource)
    {
        GD.Print($"Loading fighter from resource: {resource.ResourcePath}");
        Health = resource.Health;
        AnimatedSprite.SpriteFrames = resource.SpriteFrames;
        AnimatedSprite.Play("idle");
        GD.Print($"Fighter loaded! Health: {Health}");
    }
}