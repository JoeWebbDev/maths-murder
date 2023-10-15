using Godot;
using System;

public partial class SettingsModal : CanvasLayer
{
	[Export] private DisplayModeDropDown _displayModeOptionButton;
	[Export] private VolumeSlider _masterSlider;
	[Export] private VolumeSlider _musicSlider;
	[Export] private VolumeSlider _sfxSlider;
	[Export] private Button _okayButton;
	[Export] private Button _closeButton;
	
	public override void _Ready()
	{
		Hide();
		_okayButton.Pressed += OnOkayPressed;
		_closeButton.Pressed += OnClosePressed;
	}

	private void OnClosePressed()
	{
		_masterSlider.OnCloseRequested();
		_musicSlider.OnCloseRequested();
		_sfxSlider.OnCloseRequested();
		_displayModeOptionButton.OnCloseRequested();
		Hide();
	}

	private void OnOkayPressed()
	{
		_masterSlider.OnOkayRequested();
		_musicSlider.OnOkayRequested();
		_sfxSlider.OnOkayRequested();
		_displayModeOptionButton.OnOkayRequested();
		Hide();
	}

	public void Show()
	{
		Visible = true;
	}
}
