using Godot;
using System;

public partial class DisplayModeDropDown : HBoxContainer
{
	private OptionButton _optionButton;
	private long _previousIndex;
	private long _currentIndex;
	
	public override void _Ready()
	{
		_optionButton = GetNode<OptionButton>("OptionButton");
		_optionButton.AddItem(DisplayServer.WindowMode.Windowed.ToString(), (int)DisplayServer.WindowMode.Windowed);
		_optionButton.AddItem(DisplayServer.WindowMode.Maximized.ToString(), (int)DisplayServer.WindowMode.Maximized);
		_optionButton.AddItem(DisplayServer.WindowMode.Fullscreen.ToString(), (int)DisplayServer.WindowMode.Fullscreen);
		// _optionButton.AddItem(DisplayServer.WindowMode.ExclusiveFullscreen.ToString(), (int)DisplayServer.WindowMode.ExclusiveFullscreen);
		var modeOnLoad = DisplayServer.WindowGetMode();
		_optionButton.ItemSelected += OnItemSelected;
		_optionButton.Selected = (int)modeOnLoad;
		_currentIndex = _optionButton.Selected;
		_previousIndex = _optionButton.Selected;
	}

	private void OnItemSelected(long index)
	{
		_currentIndex = index;
		DisplayServer.WindowSetMode((DisplayServer.WindowMode)_optionButton.GetItemId((int)index));
	}
	
	public void OnCloseRequested()
	{
		OnItemSelected(_previousIndex);
		_optionButton.Select((int)_previousIndex);
	}

	public void OnOkayRequested()
	{
		_previousIndex = _currentIndex;
	}
}
