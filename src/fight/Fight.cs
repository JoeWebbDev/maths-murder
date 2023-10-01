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
		Ui.Retry += OnRetry;
		Ui.QuitToMenu += OnQuitToMenu;
		Player.HealthChanged += CheckDeath;
		Enemy.HealthChanged += CheckDeath;
		Timer.MatchTimerEnded += EndFight;
	}

	public async void Start()
	{
		await Ui.StartCountdown(_fightCountdownDuration);
		GetTree().Paused = false;
		Timer.Start();
	}

	private void OnRetry()
	{
		Engine.TimeScale = 1f;
		EmitSignal(SignalName.FightRetryRequested);
	}

	private void OnQuitToMenu()
	{
		Engine.TimeScale = 1f;
		EmitSignal(SignalName.QuitRequested);
	}

	private void CheckDeath(int from, int to)
	{
		if (to <= 0) EndFight();
	}

	private void EndFight()
	{
		// We can use this as an alternative to GetTree().Paused for bullet time
		Engine.TimeScale = 0.2f;
		// GetTree().Paused = true;
		var playerWon = Player.Health > Enemy.Health;
		Ui.ShowResultScreen(playerWon);
	}
}
