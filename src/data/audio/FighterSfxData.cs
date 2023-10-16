using Godot;

public partial class FighterSfxData : Resource
{
	[Export] public AudioPool HitPool;
	[Export] public AudioPool SwooshPool;
	[Export] public AudioPool FinalHitPool;
	
	public FighterSfxData() : this(null, null, null) { }
	
	public FighterSfxData(AudioPool hitPool, AudioPool swooshPool, AudioPool finalHitPool)
	{
		HitPool = hitPool;
		SwooshPool = swooshPool;
		FinalHitPool = finalHitPool;
	}
}
