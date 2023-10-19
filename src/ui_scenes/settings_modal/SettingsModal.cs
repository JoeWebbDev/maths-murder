using Godot;
using System;

public partial class SettingsModal : CanvasLayer
{
	[Export] private DisplayModeDropDown _displayModeOptionButton;
	[Export] private VolumeSlider _masterSlider;
	[Export] private VolumeSlider _musicSlider;
	[Export] private VolumeSlider _sfxSlider;
	[Export] private Button _confirmButton;
	[Export] private Button _cancelButton;
	[Export] private Button _keybindsButton;
	[Export] private KeybindMenu _keybindMenu;
	[Export] private PanelContainer _modalContents;
	
	public override void _Ready()
	{
		Hide();
		_confirmButton.Pressed += OnConfirmPressed;
		_cancelButton.Pressed += OnCancelPressed;
		_keybindsButton.Pressed += OnKeybindsButtonPressed;
		_keybindMenu.MenuClosed += OnKeybindMenuClosed;
	}

	private void OnKeybindMenuClosed()
	{
		Show();
	}

	private void OnCancelPressed()
	{
		_masterSlider.OnCloseRequested();
		_musicSlider.OnCloseRequested();
		_sfxSlider.OnCloseRequested();
		_displayModeOptionButton.OnCloseRequested();
		Hide();
	}

	private void OnConfirmPressed()
	{
		_masterSlider.OnOkayRequested();
		_musicSlider.OnOkayRequested();
		_sfxSlider.OnOkayRequested();
		_displayModeOptionButton.OnOkayRequested();
		Hide();
	}

	private void OnKeybindsButtonPressed()
	{
		_keybindMenu.Show();
		Hide();
	}

	public void Show()
	{
		_modalContents.Visible = true;
	}

	public void Hide()
	{
		_modalContents.Visible = false;
	}
}
