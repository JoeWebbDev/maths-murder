using System.Linq;
using Godot;
using MathsMurderSpike.Core.Input;

public partial class ActionRemapButton : Node
{
	[Export(PropertyHint.Enum, $"{InputAction.AllAsHintString}")] public string Action;
	[Export] private string _labelText;
	private StringName[] _otherPlayerActions;
	private Button _remapButton;
	private Label _label;
	private double _refreshTimer;
	public bool WaitingForInput;
	private InputEvent _previousEvent;
	private InputEvent _currentEvent;

	[Signal] public delegate void WaitingForNewBindEventHandler(string action);
	[Signal] public delegate void RebindEventHandler();
	
	public override void _Ready()
	{
		_otherPlayerActions = InputMap.GetActions().Where(x =>
		{
			var action = x.ToString();
			return action != null && !string.Equals(action, Action) && action.StartsWith("p_");
		}).ToArray();
		_previousEvent = InputMap.ActionGetEvents(Action)[0];
		_currentEvent = InputMap.ActionGetEvents(Action)[0];
		_label = GetNode<Label>("Label");
		_remapButton = GetNode<Button>("Button");
		if(!InputMap.HasAction(Action))
			GodotLogger.LogError($"{Action} action does not exist in the Input Map");
		_remapButton.SetProcessUnhandledKeyInput(false);
		_label.Text = _labelText;
		_remapButton.Toggled += OnToggled;
		DisplayCurrentKey();
	}

	public override void _Process(double delta)
	{
		_refreshTimer += delta;
		if (_refreshTimer > 1)
		{
			if (!ActionHasEvent())
				_remapButton.Text = "Bind me!";
		}
	}

	private void OnToggled(bool buttonPressed)
	{
		_remapButton.SetProcessUnhandledKeyInput(buttonPressed);
		if (buttonPressed)
		{
			_remapButton.Text = "...";
			WaitingForInput = true;
			_remapButton.ReleaseFocus();
			EmitSignal(SignalName.WaitingForNewBind, Action);
		}
		else
		{
			DisplayCurrentKey();
		}
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (!WaitingForInput) return;
		RemapAction(@event);
		_remapButton.SetPressedNoSignal(false);
		WaitingForInput = false;
	}
	
	private void RemapAction(InputEvent @event)
	{
		foreach (var action in _otherPlayerActions)
		{
			if (InputMap.ActionHasEvent(action, @event))
				InputMap.ActionEraseEvent(action, @event);
		}
		InputMap.ActionEraseEvents(Action);
		InputMap.ActionAddEvent(Action, @event);
		_currentEvent = @event;
		_remapButton.Text = $"{@event.AsText()}";
		EmitSignal(SignalName.Rebind);
	}

	public void DisplayCurrentKey()
	{
		var currentKey = InputMap.ActionGetEvents(Action)[0].AsText();
		_remapButton.Text = $"{currentKey}";
	}

	public void ResetButton()
	{
		DisplayCurrentKey();
		_remapButton.SetPressedNoSignal(false);
	}

	public void ResetEvent()
	{
		InputMap.ActionEraseEvents(Action);
		InputMap.ActionAddEvent(Action, _previousEvent);
		DisplayCurrentKey();
	}
	
	public void ConfirmEvent()
	{
		_previousEvent = InputMap.ActionGetEvents(Action)[0];
	}

	public bool ActionHasEvent()
	{
		return InputMap.ActionGetEvents(Action).Count > 0;
	}
}
