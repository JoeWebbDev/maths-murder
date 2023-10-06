using Godot;
using System;
using MathsMurderSpike.core.AI;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(FighterData), "", nameof(Resource))]
public partial class FighterData : Resource
{
    [Export] public int Health { get; set; }
    [Export] public int Speed { get; set; }
    [Export] public int Strength { get; set; }
    [Export] public int Defense { get; set; }
    [Export] public int ExperiencePointsValue;
    [Export] public Texture2D NumberTexture { get; set; }
    [Export] public AIStateController AiStateController { get; set; }
    
    public FighterData() : this(0, 0, 0, 0, null) { }

    public FighterData(int health, int speed, int strength, int defense, Texture2D numberTexture)
    {
        Health = health;
        Speed = speed;
        Strength = strength;
        Defense = defense;
        NumberTexture = numberTexture;
    }
}
