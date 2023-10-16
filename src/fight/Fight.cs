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
    [Export] private ColorRect _pausePostProcess;
    [Export] private int _fightCountdownDuration;
    [Export] private FightVersus _preFightVersus;
    [Export] private FightCameraController _cameraController;
    [Export] private Sprite2D _background;
    [Export] private GenericSpriteCollection _backgroundCollection;

    [ExportGroup("Death related properties")]
    [Export] private float _engineTimeScaleOnDeath = 0.3f;
    [Export] private ColorRect _deathPostProcess;
    [Export] private float _cameraPanAndZoomRawDurationOnDeath = 1f;
    [Export] private Vector2 _cameraZoomOnDeath = new(2f, 2f);
    [Export] private float _maxDurationBeforeSceneSwitchOnDeath = 2f;

    [ExportGroup("SFX tracks")] 
    [Export] private AudioStream _fightCountdownAnnouncerSfx;
    [Export] private AudioStream _victoryAnnouncerSfx;
    [Export] private AudioStream _failureAnnouncerSfx;

    private GameDataManager _gameDataManager;
    private GlobalAudioManager _audioManager;
    private FighterData _playerFighterData;
    private FighterData _enemyFighterData;

    public override void _Ready()
    {
        if (Player.PlayerNumber == Enemy.PlayerNumber)
            GodotLogger.LogError("Players must have different player numbers.");
        GetTree().Paused = true;
        _gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        _audioManager = GetNode<GlobalAudioManager>("/root/GlobalAudioManager");
        var playerData = _gameDataManager.GetPlayerData();
        _playerFighterData = playerData.FighterData;
        Player.InitFighter(_playerFighterData, true);
        _enemyFighterData = playerData.CurrentOpponent ?? _gameDataManager.GetRandomOpponentData();
        Enemy.InitFighter(_enemyFighterData);
        AiController.StateController = _enemyFighterData.AiStateController;
        Ui.SetFighters(Player, Enemy);
        Ui.ContinueGame += OnContinueGame;
        Player.HealthChanged += CheckDeath;
        Enemy.HealthChanged += CheckDeath;
        Timer.MatchTimerEnded += EndFight;
        InputController.PauseRequested += Pause;
        _pauseMenu.ResumeButtonPressed += Resume;
        _pauseMenu.QuitToMenuButtonPressed += OnQuitToMenu;
        _pauseMenu.Hide();
        _background.Texture = _backgroundCollection.Sprites[(int)(GD.Randi() % _backgroundCollection.Sprites.Count)];
    }

    private void OnContinueGame()
    {
        Engine.TimeScale = 1f;
        EmitSignal(SignalName.ContinueGameRequested);
    }

    private void Pause()
    {
        GetTree().Paused = true;
        _pausePostProcess.Show();
        _pauseMenu.Show();
    }

    private void Resume()
    {
        GetTree().Paused = false;
        _pausePostProcess.Hide();
        _pauseMenu.Hide();
    }

    public async void Start()
    {
        await _preFightVersus.PlayPreFightVersusSequence(_playerFighterData, _enemyFighterData, _background.Texture);
        await Task.Delay(200);
        _audioManager.PlaySfx(_fightCountdownAnnouncerSfx);
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

    private void CheckDeath(float from, float to)
    {
        if (to <= 0) EndFight();
    }

    private void EndFight()
    {
        Player.FreezeCommandInput = true;
        Enemy.FreezeCommandInput = true;
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
        _audioManager.PlaySfx(_victoryAnnouncerSfx);
        // We can use this as an alternative to GetTree().Paused for bullet time
        Engine.TimeScale = 0.5f;
        _gameDataManager.RegisterDefeatedOpponent();
        Ui.ShowVictoryScreen();
    }

    private async void OnFightLost()
    {
        _audioManager.PlaySfx(_failureAnnouncerSfx);
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