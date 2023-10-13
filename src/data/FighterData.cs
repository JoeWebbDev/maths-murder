using Godot;
using System;
using MathsMurderSpike.core.AI;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(FighterData), "", nameof(Resource))]
public partial class FighterData : Resource
{
    [Export(PropertyHint.Range, "0,1000,1")] public float Health { get; set; }
    [Export(PropertyHint.Range, "0,1000,1")] public float Speed { get; set; }
    [Export(PropertyHint.Range, "0,1000,1")] public float Strength { get; set; }
    [Export(PropertyHint.Range, "0,1000,1")] public float Defense { get; set; }
    [Export(PropertyHint.Range, "0,1000,1")] public int ExperiencePointsValue;
    [Export] public SpriteDataCollection SpriteDataCollection;
    [Export] public LevelsConfig LevelsConfig;
    [Export] public AIStateController AiStateController { get; set; }
    
    public FighterData() : this(0, 0, 0, 0, new LevelsConfig(), new SpriteDataCollection()) { }

    public FighterData(float health, float speed, float strength, float defense, LevelsConfig levelsConfig, SpriteDataCollection spriteDataCollection)
    {
        Health = health;
        Speed = speed;
        Strength = strength;
        Defense = defense;
    }

    public int GetLevel()
    {
        return LevelsConfig.CalculateLevel(Health + Speed + Strength + Defense);
    }

    public NumberSpriteData GetSpriteData()
    {
        return SpriteDataCollection.GetSpriteDataForLevel(GetLevel());
    }
}
