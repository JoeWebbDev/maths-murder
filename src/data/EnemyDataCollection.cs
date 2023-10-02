using Godot;
public partial class EnemyDataCollection : Resource
{
    [Export] public Godot.Collections.Array<FighterData> FighterDataCollection;

    public EnemyDataCollection() : this(null) { }

    public EnemyDataCollection(Godot.Collections.Array<FighterData> fighterDataCollection)
    {
        FighterDataCollection = fighterDataCollection;
    }
}
