using System;
using Godot;

public partial class Fight : Node
{
	[Signal] public delegate void FightRetryRequestedEventHandler();
	[Signal] public delegate void QuitRequestedEventHandler();
	[Export] public FightUI Ui;
	[Export] public Fighter Player { get; set; }
	[Export] public Fighter Enemy { get; set; }
	[Export] public MatchTimer Timer { get; private set; }
	[Export] private int _fightCountdownDuration;

	public override void _Ready()
	{
		if (Player.PlayerNumber == Enemy.PlayerNumber)
			GodotLogger.LogError("Players must have different player numbers.");
		GetTree().Paused = true;
		Ui.StartCountdown(_fightCountdownDuration);
		Ui.FightCountdownComplete += StartFight;
		Ui.Retry += OnRetry;
		Ui.QuitToMenu += OnQuitToMenu;
		Player.HealthChanged += CheckDeath;
		Enemy.HealthChanged += CheckDeath;
		Timer.MatchTimerEnded += EndFight;
	}

	private void OnRetry()
	{
		EmitSignal(SignalName.FightRetryRequested);
	}

	private void OnQuitToMenu()
	{
		EmitSignal(SignalName.QuitRequested);
	}

	private void CheckDeath(int from, int to)
	{
		if (to <= 0) EndFight();
	}

	private void StartFight()
	{
		GetTree().Paused = false;
		Timer.Start();
	}

	private void EndFight()
	{
		GetTree().Paused = true;
		var playerWon = Player.Health > Enemy.Health;
		Ui.ShowResultScreen(playerWon);
	}
}
