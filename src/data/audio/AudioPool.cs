using Godot;
using System;
using Godot.Collections;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(AudioPool), "", nameof(Resource))]
public partial class AudioPool : Resource
{
	[Export] private Array<AudioStream> _pool;

	public AudioPool() : this(null) { }
	
	public AudioPool(Array<AudioStream> pool)
	{
		_pool = pool;
	}

	public AudioStream GetRandomFromPool()
	{
		return _pool[GD.RandRange(0, _pool.Count - 1)];
	}
}
