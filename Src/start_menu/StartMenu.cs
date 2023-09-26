using Godot;

public partial class StartMenu : Node
{
	[Export] private Button _startButton;
	[Export] private Button _quitButton;
	[Signal] public delegate void StartGameEventHandler();
	[Signal] public delegate void QuitGameEventHandler();

	public override void _Ready()
	{
		_startButton.Pressed += OnStartButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;
	}

	private void OnStartButtonPressed()
	{
		EmitSignal(SignalName.StartGame);
	}
	
	private void OnQuitButtonPressed()
	{
		EmitSignal(SignalName.QuitGame);
	}
}
