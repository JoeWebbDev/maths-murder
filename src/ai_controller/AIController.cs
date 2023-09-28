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
		GD.Print("AI fighter initialized");
	}

	public override void _Process(double delta)
	{
		if (!_initialized)
			return;
        
		_timer += delta;
		if (_timer >= 2)
		{
			GD.Print("Executing punch.");
			Fighter.Execute(new PunchCommand());
			_timer = 0;
		}
	}
}
