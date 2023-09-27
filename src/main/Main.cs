using System.Threading;
using Godot;

public partial class Main : Node
{
	[Export] private PackedScene _startMenuScene;
	[Export] private PackedScene _fightScene;
	private Node _currentScene;
	
	public override void _Ready()
	{
		LoadStartMenuScene();
	}

	private void NewGame()
	{
		LoadFightScene();
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
		AddChild(startMenuScene);
		startMenuScene.StartGame += NewGame;
		startMenuScene.QuitGame += QuitGame;
	}
	private void LoadFightScene()
	{
		var fightScene = _fightScene.Instantiate<Fight>();
		_currentScene?.QueueFree();
		_currentScene = fightScene;
		fightScene.UI.QuitToMenu += LoadStartMenuScene;
		fightScene.UI.Retry += LoadFightScene;
		AddChild(fightScene);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
			GetTree().Quit();
	}
}
