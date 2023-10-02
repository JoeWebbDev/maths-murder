using Godot;
using System;

public partial class PlayerData : Resource
{
    [Export] public FighterData FighterData;

    public PlayerData(): this (null) { }
    public PlayerData(FighterData fighterData)
    {
        FighterData = fighterData;
    }
}
