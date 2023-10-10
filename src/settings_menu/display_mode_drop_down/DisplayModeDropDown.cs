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
		_optionButton.ItemSelected += OnItemSelected;
		_previousIndex = _optionButton.Selected;
	}

	private void OnItemSelected(long index)
	{
		if (index == 0)
		{
			_currentIndex = 0;
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
		else if (index == 1)
		{
			_currentIndex = 1;
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
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
