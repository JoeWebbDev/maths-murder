using Godot;
using Godot.Collections;

public partial class LevelsConfig : Resource
{
    [Export] public Array<int> LevelThresholdConfig;

    public LevelsConfig() : this (new Array<int>()) { }
    
    public LevelsConfig(Array<int> levelThresholdConfig)
    {
        LevelThresholdConfig = levelThresholdConfig;
    }
    
    public int CalculateLevel(float totalPoints)
    {
        var level = 1;
        foreach (var threshold in LevelThresholdConfig)
        {
            if (totalPoints < threshold)
                return level;
            level++;
        }
        return level;
    }
}
