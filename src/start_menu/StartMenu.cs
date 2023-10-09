using System.Threading.Tasks;
using Godot;

public partial class StartMenu : Node
{
	[Export] private Button _startButton;
	[Export] private Button _quitButton;
	[Export] private CanvasLayer _ui;
	[Signal] public delegate void StartGameEventHandler();
	[Signal] public delegate void QuitGameEventHandler();

	[ExportGroup("Title animation properties")]
	[Export] private int _uiShowDelayInMs;
	[Export] private int _delayBetweenAnimationsInMs;
	[ExportSubgroup("'Maths' title animation properties")] 
	[Export] private Control _mask;
	[Export] private Vector2 _maskStartingSize;
	[Export] private Vector2 _maskFinishingSize;
	[Export] private float _maskSlideDuration;
	[ExportSubgroup("'Murder' title slam animation properties")]
	[Export] private Sprite2D _murderSprite;
	[Export] private Vector2 _startScale;
	[Export] private Vector2 _finishScale;
	[Export] private float _murderScaleDuration;
	[ExportSubgroup("Background animation properties")] 
	[Export] private Sprite2D _background;
	[Export] private Color _startColor;
	[Export] private Color _endColor;
	[Export] private float _colorChangeDuration;

	public override void _Ready()
	{
		_ui.Hide();
		_murderSprite.Hide();
		_background.Modulate = _startColor;
		_startButton.Pressed += OnStartButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;
		PlayTitleAnimation();
	}

	private async void PlayTitleAnimation()
	{
		_mask.Size = _maskStartingSize;
		var tree = GetTree();
		var tween = tree.CreateTween();
		tween.TweenProperty(_mask, "size", _maskFinishingSize, _maskSlideDuration);
		await ToSignal(tween, "finished");
		await Task.Delay(_delayBetweenAnimationsInMs);
		_murderSprite.Scale = _startScale;
		_murderSprite.Show();
		tween = tree.CreateTween();
		tween.TweenProperty(_murderSprite, "scale", _finishScale, _murderScaleDuration);
		await ToSignal(tween, "finished");
		tween = tree.CreateTween();
		tween.TweenProperty(_background, "modulate", _endColor, _colorChangeDuration);
		await ToSignal(tween, "finished");
		await Task.Delay(_uiShowDelayInMs);
		_ui.Show();
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
