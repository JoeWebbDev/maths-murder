using System.Threading;
using Godot;

public partial class Main : Node
{
	[Export] private PackedScene _startMenuScene;
	[Export] private PackedScene _fightScene;
	[Export] private PackedScene _pauseMenuScene;
	[Export] private GlobalInputController _globalInputController;
	private Node _currentScene;
	private PauseMenu _pauseMenu;
	
	public override void _Ready()
	{
		LoadStartMenuScene();
		_globalInputController.OpenPauseMenuRequested += OnOpenPauseMenuRequested;
	}

	private void OnOpenPauseMenuRequested()
	{
		if (_pauseMenu != null)
		{
			Resume();
			return;
		}
		if (_currentScene is not StartMenu) Pause();
	}

	private void Pause()
	{
		GetTree().Paused = true;
		
		// We create a new pause menu each time here. Probs better to have it live on main
		// and just show?
		_pauseMenu = _pauseMenuScene.Instantiate<PauseMenu>();
		_pauseMenu.ResumeButtonPressed += Resume;
		_pauseMenu.QuitToMenuButtonPressed += LoadStartMenuScene;
		AddChild(_pauseMenu);
	}

	private void Resume()
	{
		GetTree().Paused = false;
		_pauseMenu.QueueFree();
		_pauseMenu = null;
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
		fightScene.QuitRequested += LoadStartMenuScene;
		fightScene.FightRetryRequested += LoadFightScene;
		AddChild(fightScene);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
			GetTree().Quit();
	}
}
