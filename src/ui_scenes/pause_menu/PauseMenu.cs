using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    [Export] private Button _resumeButton;
    [Export] private Button _settingsButton;
    [Export] private Button _quitToMenuButton;
    [Export] private Label _label;
    private SettingsModal _settingsModal;
    
    [Signal] public delegate void ResumeButtonPressedEventHandler();
    [Signal] public delegate void QuitToMenuButtonPressedEventHandler();

    public override void _Ready()
    {
        _settingsModal = GetNode<SettingsModal>("/root/SettingsModal");
        _settingsModal.VisibilityChanged += OnModalVisibilityChanged;
        _resumeButton.Pressed += OnResumeButtonPressed;
        _settingsButton.Pressed += OnSettingsButtonPressed;
        _quitToMenuButton.Pressed += OnQuitToMenuButtonPressed;
    }

    private void OnModalVisibilityChanged()
    {
        var visible = _settingsModal.Visible;
        _resumeButton.Visible = !visible;
        _settingsButton.Visible = !visible;
        _quitToMenuButton.Visible = !visible;
        _label.Visible = !visible;
    }

    private void OnResumeButtonPressed()
    {
        EmitSignal(SignalName.ResumeButtonPressed);
    }

    private void OnSettingsButtonPressed()
    {
        _settingsModal.Show();
    }

    private void OnQuitToMenuButtonPressed()
    {
        EmitSignal(SignalName.QuitToMenuButtonPressed);
    }
}
