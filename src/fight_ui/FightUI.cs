using Godot;

public partial class FightUI : CanvasLayer
{
	[Export] private MatchTimer _matchTimer;
	[Export] private Button _retryButton;
	[Export] private Button _quitToMenuButton;
	[Signal] public delegate void QuitToMenuEventHandler();
	[Signal] public delegate void RetryEventHandler();
	
	public override void _Ready()
	{
		_retryButton.Pressed += () => EmitSignal(SignalName.Retry);
		_quitToMenuButton.Pressed += () => EmitSignal(SignalName.QuitToMenu);
		_matchTimer.MatchTimerEnded += ShowResultScreen;
		_matchTimer.Start();
	}

	private void ShowResultScreen()
	{
		_retryButton.Visible = true;
		_quitToMenuButton.Visible = true;
	}
}
