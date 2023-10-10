using System.Threading.Tasks;
using Godot;

public partial class FightUI : CanvasLayer
{
	[Export] private Fighter _player;
	[Export] private Fighter _enemy;
	[Export] private HealthBar _playerHealthBar;
	[Export] private HealthBar _enemyHealthBar;
	[Export] private MatchTimer _matchTimer;
	[Export] private Label _matchTimerLabel;
	[Export] private Label _countdownLabel;
	[Export] private Button _continueButton;

	[Signal] public delegate void ContinueGameEventHandler();
	[Signal] public delegate void FightersInitializedEventHandler();

	private bool _isTimerActive;
	
	public override void _Ready()
	{
		_continueButton.Pressed += () =>
		{
			EmitSignal(SignalName.ContinueGame);
		};

		_matchTimer.MatchTimerStarted += OnMatchTimerStart;
		_matchTimer.MatchTimerEnded += OnMatchTimerEnded;
	}

	private void OnMatchTimerEnded()
	{
		_isTimerActive = false;
		_matchTimerLabel.Text = "0";
	}
	
	public void SetFighters(Fighter player, Fighter enemy)
	{
		_player = player;
		_enemy = enemy;
		_playerHealthBar.Fighter = player;
		_enemyHealthBar.Fighter = enemy;
		EmitSignal(SignalName.FightersInitialized);
	}

	private void OnMatchTimerStart()
	{
		_isTimerActive = true;
	}

	public async Task StartCountdown(int duration)
	{
		for (var i = duration; i > 0; i--)
		{
			_countdownLabel.Text = $"{i}...";
			await Task.Delay(1000);
		}

		_countdownLabel.Text = "Fight!";
		await Task.Delay(1000);
		_countdownLabel.Hide();
	}

	public void ShowVictoryScreen()
	{
		_continueButton.Visible = true;
		_countdownLabel.Text = "You win!";
		_countdownLabel.Show();
	}

	public override void _Process(double delta)
	{
		if (_isTimerActive) _matchTimerLabel.Text = (Mathf.CeilToInt(_matchTimer.TimeLeft ?? 0)).ToString();
	}
}
