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
		LoadStartMenuScene();
	}

	private async void NewGame()
	{
		await _screenFader.FadeOutAsync();
		var fightScene = LoadFightScene();
		await _screenFader.FadeInAsync();
		GodotLogger.LogDebug("Starting fight");
		fightScene.Start();
	}
	
	private void QuitGame()
	{
		GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
	}

	private void LoadStartMenuScene()
	{
		var startMenuScene = _startMenuScene.Instantiate<StartMenu>();
		_currentScene?.QueueFree();
		_currentScene = startMenuScene;
		startMenuScene.StartGame += NewGame;
		startMenuScene.QuitGame += QuitGame;
		AddChild(startMenuScene);
	}
	private Fight LoadFightScene()
	{
		GodotLogger.LogDebug("Loading Fight Scene");
		var fightScene = _fightScene.Instantiate<Fight>();
		_currentScene?.QueueFree();
		_currentScene = fightScene;
		fightScene.QuitRequested += LoadStartMenuScene;
		fightScene.FightRetryRequested += () => LoadFightScene();
		AddChild(fightScene);
		return fightScene;
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
			GetTree().Quit();
	}
}
