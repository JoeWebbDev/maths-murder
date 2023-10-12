using System;
using System.Threading.Tasks;
using Godot;

public partial class Fight : Node
{
    [Signal]
    public delegate void ContinueGameRequestedEventHandler();

    [Signal]
    public delegate void QuitRequestedEventHandler();

    [Signal]
    public delegate void PlayerDeathEventHandler();

    [Export] public FightUI Ui;
    [Export] public FightInputController InputController { get; private set; }
    [Export] public AIController AiController { get; set; }
    [Export] public Fighter Player { get; set; }
    [Export] public Fighter Enemy { get; set; }
    [Export] public MatchTimer Timer { get; private set; }
    [Export] private PauseMenu _pauseMenu;
    [Export] private int _fightCountdownDuration;
    [Export] private Sprite2D _preFightPlaceholderScreen;
    [Export] private FightCameraController _cameraController;

    [ExportGroup("Death related properties")]
    [Export] private float _engineTimeScaleOnDeath = 0.3f;
    [Export] private ColorRect _deathPostProcess;
    [Export] private float _cameraPanAndZoomRawDurationOnDeath = 1f;
    [Export] private Vector2 _cameraZoomOnDeath = new(2f, 2f);
    [Export] private float _maxDurationBeforeSceneSwitchOnDeath = 2f;

    private GameDataManager _gameDataManager;

    public override void _Ready()
    {
        if (Player.PlayerNumber == Enemy.PlayerNumber)
            GodotLogger.LogError("Players must have different player numbers.");
        GetTree().Paused = true;
        _gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        var playerData = _gameDataManager.GetPlayerData();
        Player.InitFighter(playerData.FighterData);
        var enemyData = playerData.CurrentOpponent ?? _gameDataManager.GetRandomOpponentData();
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
        _preFightPlaceholderScreen.Show();
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
        // Wait for the ACTUAL pre-fight animation to finish
        await Task.Delay(1500);
        _preFightPlaceholderScreen.Hide();
        await Task.Delay(200);
        await Ui.StartCountdown(_fightCountdownDuration);
        GetTree().Paused = false;
        Timer.Start();
    }

    private void OnQuitToMenu()
    {
        Engine.TimeScale = 1f;
        GetTree().Paused = false;
        EmitSignal(SignalName.QuitRequested);
    }

    private void CheckDeath(int from, int to)
    {
        if (to <= 0) EndFight();
    }

    private void EndFight()
    {
        var playerWon = Player.CurrentHealth / Player.TotalHealth > Enemy.CurrentHealth / Enemy.TotalHealth;
        if (playerWon)
        {
            OnFightWon();
            return;
        }

        OnFightLost();
    }

    private void OnFightWon()
    {
        // We can use this as an alternative to GetTree().Paused for bullet time
        Engine.TimeScale = 0.5f;
        _gameDataManager.RegisterDefeatedOpponent();
        Ui.ShowVictoryScreen();
        return;
    }

    private async void OnFightLost()
    {
        _deathPostProcess.Show();
        Engine.TimeScale = _engineTimeScaleOnDeath;
        _cameraController.Freeze = true;
        var cameraMoveTask = _cameraController.CinematicCamera.SendToPosition(Player.GlobalPosition, _cameraPanAndZoomRawDurationOnDeath);
        var cameraZoomTask = _cameraController.CinematicCamera.SendToZoomValue(_cameraZoomOnDeath, _cameraPanAndZoomRawDurationOnDeath);
        var maxDurationTask = Task.Delay((int)(_maxDurationBeforeSceneSwitchOnDeath * 1000));
        await Task.WhenAny(maxDurationTask, Task.WhenAll(cameraMoveTask, cameraZoomTask));
        // Go to death scene, shows stats of the game etc
        EmitSignal(SignalName.PlayerDeath);
    }
}