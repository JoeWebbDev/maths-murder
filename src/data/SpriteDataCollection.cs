using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class SpriteDataCollection : Resource
{
    [Export] public Array<NumberSpriteData> NumberSpriteDataCollection;

    public SpriteDataCollection() : this(null) { }

    public SpriteDataCollection(Array<NumberSpriteData> numberSpriteDataCollection)
    {
        NumberSpriteDataCollection = numberSpriteDataCollection;
    }
    
    public NumberSpriteData GetSpriteDataForLevel(int level)
    {
        if (level < 1)
        {
            GodotLogger.LogError("Level cannot be less than 1");
        }
    
        if (NumberSpriteDataCollection?.Count > 0)
            return NumberSpriteDataCollection[level - 1] ?? NumberSpriteDataCollection.First();
        
        return null;
    }
}
