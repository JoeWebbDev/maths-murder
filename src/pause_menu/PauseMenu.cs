using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    [Export] private Button _resumeButton;
    [Export] private Button _settingsButton;
    [Export] private Button _quitToMenuButton;
    
    [Signal] public delegate void ResumeButtonPressedEventHandler();
    [Signal] public delegate void QuitToMenuButtonPressedEventHandler();

    public override void _Ready()
    {
        _resumeButton.Pressed += OnResumeButtonPressed;
        _settingsButton.Pressed += OnSettingsButtonPressed;
        _quitToMenuButton.Pressed += OnQuitToMenuButtonPressed;
    }

    private void OnResumeButtonPressed()
    {
        EmitSignal(SignalName.ResumeButtonPressed);
    }

    private void OnSettingsButtonPressed()
    {
    }

    private void OnQuitToMenuButtonPressed()
    {
        EmitSignal(SignalName.QuitToMenuButtonPressed);
    }
}
