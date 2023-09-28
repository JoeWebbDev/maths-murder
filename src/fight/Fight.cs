using System;
using Godot;

public partial class Fight : Node
{
	[Export] public FightUI UI;
	[Export] public MatchTimer Timer { get; private set; }
	[Export] private int _fightCountdownDuration;

	public override void _Ready()
	{
		GetTree().Paused = true;
		UI.StartCountdown(_fightCountdownDuration);
		UI.FightCountdownComplete += StartFight;
	}

	private void StartFight()
	{
		GetTree().Paused = false;
		Timer.Start();
	}
}
