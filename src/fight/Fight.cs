using System;
using Godot;

public partial class Fight : Node
{
    [Signal]
    public delegate void ContinueGameRequestedEventHandler();

    [Signal]
    public delegate void QuitRequestedEventHandler();

    [Export] public FightUI Ui;
    [Export] public FightInputController InputController { get; private set; }
    [Export] public AIController AiController { get; set; }
    [Export] public Fighter Player { get; set; }
    [Export] public Fighter Enemy { get; set; }
    [Export] public MatchTimer Timer { get; private set; }
    [Export] private PauseMenu _pauseMenu;
    [Export] private int _fightCountdownDuration;

    private GameDataManager _gameDataManager;

    public override void _Ready()
    {
        if (Player.PlayerNumber == Enemy.PlayerNumber)
            GodotLogger.LogError("Players must have different player numbers.");
        GetTree().Paused = true;
        _gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        Player.InitFighter(_gameDataManager.GetPlayerFighterData());
        var enemyData = _gameDataManager.GetRandomOpponentData();
        Enemy.InitFighter(enemyData);
        AiController.StateController = enemyData.AiStateController;
        Ui.SetFighters(Player, Enemy);
        Ui.ContinueGame += OnContinueGame;
        Player.HealthChanged += CheckDeath;
        Enemy.HealthChanged += CheckDeath;
        Timer.MatchTimerEnded += EndFight;
        InputController.PauseRequested += Pause;
        _pauseMenu.ResumeButtonPressed += Resume;
        _pauseMenu.QuitToMenuButtonPressed += OnQuitToMenu;
        _pauseMenu.Hide();
    }

    private void OnContinueGame()
    {
        Engine.TimeScale = 1f;
        EmitSignal(SignalName.ContinueGameRequested);
    }

    private void Pause()
    {
        GetTree().Paused = true;
        _pauseMenu.Show();
    }

    private void Resume()
    {
        GetTree().Paused = false;
        _pauseMenu.Hide();
    }

    public async void Start()
    {
        await Ui.StartCountdown(_fightCountdownDuration);
        GetTree().Paused = false;
        Timer.Start();
    }

    private void OnQuitToMenu()
    {
        Engine.TimeScale = 1f;
        _gameDataManager.ResetPlayerData();
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
        if (playerWon)
            _gameDataManager.IncreasePlayerExperienceFromCurrentOpponent();
        Ui.ShowResultScreen(playerWon);
    }
}