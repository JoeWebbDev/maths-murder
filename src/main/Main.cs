using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class Main : Node
{
	[Export] private PackedScene _startMenuScene;
	[Export] private PackedScene _fightScene;
	[Export] private PackedScene _trainingScene;
	[Export] private PackedScene _bracketScene;
	[Export] private PackedScene _chooseNextFightScene;
	[Export] private ScreenFader _screenFader;
	private Node _currentScene;
	
	public override void _Ready()
	{
		InstantiateStartMenuScene();
	}

	private async void NewGame()
	{
		OnNextFightRequested();
	}

	private async Task LoadFightScene()
	{
		// await _screenFader.FadeOutAsync();
		var fightScene = InstantiateFightScene();
		// await _screenFader.FadeInAsync();
		GodotLogger.LogDebug("Starting fight");
		fightScene.Start();
	}

	private async Task LoadStartMenuScene()
	{
		await _screenFader.FadeOutAsync();
		InstantiateStartMenuScene();
		await _screenFader.FadeInAsync();
	}

	private async Task LoadTrainingScene()
	{
		await _screenFader.FadeOutAsync();
		InstantiateTrainingScene();
		await _screenFader.FadeInAsync();
	}

	private async Task LoadBracketScene()
	{
		await _screenFader.FadeOutAsync();
		var bracketScene = InstantiateBracketScene();
		await _screenFader.FadeInAsync();
		bracketScene.PlayCameraTweens();
	}

	private async Task LoadChooseNextFightScene()
	{
		await _screenFader.FadeOutAsync();
		InstantiateChooseNextFightScene();
		await _screenFader.FadeInAsync();
	}

	private void QuitGame()
	{
		GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
	}

	private void InstantiateStartMenuScene()
	{
		var startMenuScene = _startMenuScene.Instantiate<StartMenu>();
		_currentScene?.QueueFree();
		_currentScene = startMenuScene;
		startMenuScene.StartGame += NewGame;
		startMenuScene.QuitGame += QuitGame;
		AddChild(startMenuScene);
	}
	private Fight InstantiateFightScene()
	{
		GodotLogger.LogDebug("Loading Fight Scene");
		var fightScene = _fightScene.Instantiate<Fight>();
		_currentScene?.QueueFree();
		_currentScene = fightScene;
		fightScene.QuitRequested += async () => { await LoadStartMenuScene(); };
		fightScene.ContinueGameRequested += async () => { await LoadTrainingScene(); };
		AddChild(fightScene);
		return fightScene;
	}

	private void InstantiateTrainingScene()
	{
		GodotLogger.LogDebug("Loading Training Scene");
		var trainingScene = _trainingScene.Instantiate<Training>();
		_currentScene?.QueueFree();
		_currentScene = trainingScene;
		trainingScene.NextFightRequested += OnNextFightRequested;
		AddChild(trainingScene);
	}

	private Bracket InstantiateBracketScene()
	{
		GodotLogger.LogDebug($"Loading {nameof(Bracket)} Scene");
		var bracketScene = _bracketScene.Instantiate<Bracket>();
		_currentScene?.QueueFree();
		_currentScene = bracketScene;
		bracketScene.BracketAnimationsComplete += async () => { await LoadFightScene(); };
		AddChild(bracketScene);
		return bracketScene;
	}

	private void InstantiateChooseNextFightScene()
	{
		GodotLogger.LogDebug($"Loading {nameof(ChooseNextFight)} Scene");
		var chooseNextFightScene = _chooseNextFightScene.Instantiate<ChooseNextFight>();
		_currentScene?.QueueFree();
		_currentScene = chooseNextFightScene;
		chooseNextFightScene.FighterSelected += async () => { await LoadBracketScene(); };
		AddChild(chooseNextFightScene);
	}

	private async void OnNextFightRequested()
	{
		var currentOpponent = GetNode<GameDataManager>("/root/GameDataManager").GetCurrentOpponentData();
		if (currentOpponent == null)
		{
			await LoadChooseNextFightScene();
			return;
		}
		await LoadBracketScene();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
			GetTree().Quit();
	}
}
