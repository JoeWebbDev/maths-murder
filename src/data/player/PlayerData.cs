using Godot;
using System;

public partial class PlayerData : Resource
{
    [Export] public FighterData FighterData;
    [Export] public int ExperiencePoints;
    [Export] public FighterData CurrentOpponent;
    
    [ExportGroup("Bracket-related properties")]
    [Export] public Godot.Collections.Array<FighterData> DefeatedOpponents { get; private set; }
    [Export] public int TotalNumberOfFightsPerGame { get; private set; } = 5;
    [Export] public int TotalExpGained { get; set; } = 0;
    [Export] public int DamageDealt { get; set; } = 0;
    [Export] public int DamageTaken { get; set; } = 0;

    public PlayerData(): this (null) { }
    public PlayerData(FighterData fighterData)
    {
        FighterData = fighterData;
    }
}
