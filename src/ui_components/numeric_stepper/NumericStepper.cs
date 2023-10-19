using System;
using Godot;
using MathsMurderSpike.core.Utility;

public partial class NumericStepper : HBoxContainer
{
	[Export] public int MinimumValue = int.MinValue;
	[Export] public int MaximumValue = int.MaxValue;
	[Export(PropertyHint.Range, "1,100,1,or_greater")]
	public int StepAmount = 1;
	[Export] public int Value
	{
		get => _value;
		set => SetValue(value);
	}
	private Label _label;
	private Button _plusButton;
	private Button _minusButton;
	private int _value;
	private bool _plusDisableOverride = false;
	[Signal] public delegate void ValueChangedEventHandler(int from, int to);

	public override void _Ready()
	{
		_label = GetNode<Label>("Label");
		_plusButton = GetNode<Button>("PlusButton");
		_minusButton = GetNode<Button>("MinusButton");
		if (_value < MinimumValue || _value > MaximumValue)
			GodotLogger.LogError("Value must be within the minimum and maximum range");
		UpdateUi();
		_plusButton.Pressed += () => SetValue(_value + StepAmount);
		_minusButton.Pressed += () => SetValue(_value - StepAmount);
	}

	private void SetValue(int value)
	{
		var previousValue = _value;
		_value = value;
		_value = Math.Max(MinimumValue, Math.Min(MaximumValue, _value));
		EmitSignal(SignalName.ValueChanged, previousValue, _value);
		UpdateUi();
	}

	private void UpdateUi()
	{
		if (_label is null)
			return;
		
		_minusButton.Disabled = _value <= MinimumValue;
		if (!_plusDisableOverride)
			_plusButton.Disabled = _value >= MaximumValue;
		_label.Text = Utilities.FormatAsTwoDigitString(_value);
	}

	public void TogglePlusButton(bool disable)
	{
		_plusDisableOverride = disable;
		_plusButton.Disabled = disable;
	}
}
