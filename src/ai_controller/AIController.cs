using Godot;
using System;
using MathsMurderSpike.core.Commands;

public partial class AIController : Node
{
	[Export] public Fighter Fighter { get; set; }
	private double _timer;
	private bool _initialized;

	public override void _Ready()
	{
		_initialized = true;
		GodotLogger.LogDebug("AI fighter initialized");
	}

	public override void _Process(double delta)
	{
		if (!_initialized)
			return;
        
		_timer += delta;
		if (_timer >= 2)
		{
			GodotLogger.LogDebug("Executing punch.");
			Fighter.Execute(new PunchCommand());
			_timer = 0;
		}
	}
}
