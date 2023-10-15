using System.Threading.Tasks;
using Godot;

public partial class StartMenu : Node
{
	[Export] private Button _startButton;
	[Export] private Button _quitButton;
	[Export] private TextureButton _settingsButton;
	[Export] private Button _howToPlayButton;
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
	[ExportGroup("Music/SFX properties")]
	[Export] private AudioStream _musicTrack;
	[Export] private float _musicDb;
	[Export] private AudioStream _mathsMurderAnnouncerTrack;
	[Export] private float _mathsMurderAnnouncerDb;
	
	private SettingsModal _settingsModal;
	private HowToPlayModal _howToPlayModal;
	private GlobalAudioManager _audioManager;

	public override void _Ready()
	{
		_settingsModal = GetNode<SettingsModal>("/root/SettingsModal");
		_howToPlayModal = GetNode<HowToPlayModal>("/root/HowToPlayModal");
		_audioManager = GetNode<GlobalAudioManager>("/root/GlobalAudioManager");
		_ui.Hide();
		_murderSprite.Hide();
		_background.Modulate = _startColor;
		_startButton.Pressed += OnStartButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;
		_settingsButton.Pressed += OnSettingsButtonPressed;
		_settingsModal.VisibilityChanged += OnModalVisibilityChanged;
		_howToPlayButton.Pressed += () => { _howToPlayModal.Show(); };

		PlayTitleAnimation();
	}

	private async void PlayTitleAnimation()
	{
		_audioManager.PlayMusic(_musicTrack, _musicDb);
		_audioManager.PlaySfx(_mathsMurderAnnouncerTrack, _mathsMurderAnnouncerDb);
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
	
	private void OnSettingsButtonPressed()
	{
		_settingsModal.Show();
	}
	
	private void OnModalVisibilityChanged()
	{
		var visible = _settingsModal.Visible;
		_ui.Visible = !visible;
	}
}
