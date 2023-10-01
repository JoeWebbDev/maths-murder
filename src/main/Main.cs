using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class Main : Node
{
	[Export] private PackedScene _startMenuScene;
	[Export] private PackedScene _fightScene;
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
		fightScene.FightRetryRequested += async () => { await LoadFightScene(); };
		AddChild(fightScene);
		return fightScene;
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
			GetTree().Quit();
	}
}
