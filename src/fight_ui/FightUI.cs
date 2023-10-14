using System.Threading.Tasks;
using Godot;

public partial class FightUI : CanvasLayer
{
	[Export] private Fighter _player;
	[Export] private Fighter _enemy;
	[Export] private HealthBar _playerHealthBar;
	[Export] private HealthBar _enemyHealthBar;
	[Export] private HealthBar _playerStaminaBar;
	[Export] private Texture2D _playerStaminaBarFullTexture;
	[Export] private Texture2D _playerStaminaBarDepletedTexture;
	[Export] private MatchTimer _matchTimer;
	[Export] private Label _matchTimerLabel;
	[Export] private Label _countdownLabel;
	[Export] private Button _continueButton;

	[Signal] public delegate void ContinueGameEventHandler();

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
		_playerHealthBar.Initialize(player.CurrentHealth, player.TotalHealth);
		_player.HealthChanged += (from, to) => { _playerHealthBar.UpdateValue(to); };
		_enemyHealthBar.Initialize(enemy.CurrentHealth, enemy.TotalHealth);
		_enemy.HealthChanged += (from, to) => { _enemyHealthBar.UpdateValue(to); };
		_playerStaminaBar.Initialize(player.TotalStamina, player.CurrentStamina, false, false);
		_player.StaminaChanged += OnPlayerStaminaChanged;
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

	private void OnPlayerStaminaChanged(int to)
	{
		_playerStaminaBar.UpdateValue(to);
		if (to <= 0) _playerStaminaBar.UpdateFillTexture(_playerStaminaBarDepletedTexture);
		if (to >= _player.TotalStamina) _playerStaminaBar.UpdateFillTexture(_playerStaminaBarFullTexture);
	}
}
