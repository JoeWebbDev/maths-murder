using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class KeybindMenu : Control
{
	[Export] private Array<ActionRemapButton> _remapButtons;
	[Export] private Button _confirmButton;
	[Export] private Button _cancelButton;
	[Signal] public delegate void MenuClosedEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_confirmButton.Pressed += OnConfirmPressed;
		_cancelButton.Pressed += OnCancelPressed;
		foreach (var remapButton in _remapButtons)
		{
			remapButton.WaitingForNewBind += OnToggled;
			remapButton.Rebind += OnRebind;
		}
	}

	private void OnRebind()
	{
			_confirmButton.Disabled = _remapButtons.Any(x => !x.ActionHasEvent());
	}

	private void OnToggled(string action)
	{
		foreach (var remapButton in _remapButtons)
		{
			if (remapButton.Action != action && remapButton.WaitingForInput)
			{
				remapButton.WaitingForInput = false;
				remapButton.ResetButton();
			}
		}
	}
	
	private void OnCancelPressed()
	{
		Hide();
		foreach (var remapButton in _remapButtons)
		{
			remapButton.ResetEvent();
		}
		EmitSignal(SignalName.MenuClosed);
	}

	private void OnConfirmPressed()
	{
		Hide();
		foreach (var remapButton in _remapButtons)
		{
			remapButton.ConfirmEvent();
		}
		EmitSignal(SignalName.MenuClosed);
	}

	public void Show()
	{
		Visible = true;
	}
}
