using System;
using Godot;
public partial class EnemyDataCollection : Resource
{
    [Export] public Godot.Collections.Array<FighterData> FighterDataCollection;

    public EnemyDataCollection() : this(null) { }

    public EnemyDataCollection(Godot.Collections.Array<FighterData> fighterDataCollection)
    {
        FighterDataCollection = fighterDataCollection;
    }

    public FighterData GetRandomOpponent()
    {
        Random random = new Random();
        var index = random.Next(0, FighterDataCollection.Count);
        return FighterDataCollection[index];
    }
}
