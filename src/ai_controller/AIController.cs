using Godot;
using System;
using MathsMurderSpike.core.AI;
using MathsMurderSpike.core.Commands;

public partial class AIController : Node
{
	[Export] public Fighter Fighter { get; set; }
	// We will need the player injected in at some point so the AI can read player state and know how to behave
	[Export] public Fighter Player { get; set; }
	private double _timer;
	private bool _initialized;
	// Once we get to loading in different difficulties, we will likely expose this and update it in a load method
	public AIStateController StateController { get; set; }

	public override void _Ready()
	{
		_initialized = true;
		GodotLogger.LogDebug("AI fighter initialized");
	}

	public override void _Process(double delta)
	{
		StateController.Process(Fighter, Player, delta);
	}
}
