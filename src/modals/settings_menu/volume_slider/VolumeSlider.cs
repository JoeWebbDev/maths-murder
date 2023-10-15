using System;
using Godot;

public partial class VolumeSlider : Node
{
	[Export] private string _busName;
	[Export] private string _labelText;
	private int _busIndex;
	private double _previousVolume;
	private double _currentVolume;
	private HSlider _slider;

	public override void _Ready()
	{
		_slider = GetNode<HSlider>("HSlider");
		_slider.ValueChanged += OnValueChanged;
		var label = GetNode<Label>("Label");
		label.Text = _labelText;
		_busIndex = AudioServer.GetBusIndex(_busName);
		_previousVolume = _slider.Value;
	}

	public void OnCloseRequested()
	{
		if (Math.Abs(_currentVolume - _previousVolume) > 0.0001)
		{
			AudioServer.SetBusVolumeDb(_busIndex, (float)Mathf.LinearToDb(_previousVolume));
			_slider.SetValueNoSignal(_previousVolume);
		}
	}

	public void OnOkayRequested()
	{
		_previousVolume = _currentVolume;
	}

	private void OnValueChanged(double value)
	{
		_currentVolume = value;
		AudioServer.SetBusVolumeDb(_busIndex, (float)Mathf.LinearToDb(value));
	}
}
