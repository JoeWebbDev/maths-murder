using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class Main : Node
{
	[Export] private PackedScene _startMenuScene;
	[Export] private PackedScene _fightScene;
	[Export] private PackedScene _trainingScene;
	[Export] private ScreenFader _screenFader;
	private Node _currentScene;
	
	public override void _Ready()
	{
		InstantiateStartMenuScene();
	}

	private async void NewGame()
	{
		await LoadFightScene();
	}

	private async Task LoadFightScene()
	{
		await _screenFader.FadeOutAsync();
		var fightScene = InstantiateFightScene();
		await _screenFader.FadeInAsync();
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
		GodotLogger.LogDebug("Loading Fight Scene");
		var trainingScene = _trainingScene.Instantiate<Training>();
		_currentScene?.QueueFree();
		_currentScene = trainingScene;
		trainingScene.NextFightRequested += async () => { await LoadFightScene(); };
		AddChild(trainingScene);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
			GetTree().Quit();
	}
}
