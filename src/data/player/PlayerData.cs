using Godot;
using System;

public partial class PlayerData : Resource
{
    [Export] public FighterData FighterData;
    [Export] public int ExperiencePoints;
    [Export] public FighterData NextOpponent;

    public PlayerData(): this (null) { }
    public PlayerData(FighterData fighterData)
    {
        FighterData = fighterData;
    }
}
