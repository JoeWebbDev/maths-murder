using Godot;

public partial class VolumeSlider : Node
{
	[Export] private string _busName;
	[Export] private string _labelText;
	private int _busIndex;

	public override void _Ready()
	{
		var slider = GetNode<HSlider>("HSlider");
		slider.ValueChanged += OnValueChanged;
		var label = GetNode<Label>("Label");
		label.Text = _labelText;
		_busIndex = AudioServer.GetBusIndex(_busName);
	}

	private void OnValueChanged(double value)
	{
		Mathf.LinearToDb(value);
		AudioServer.SetBusVolumeDb(_busIndex, (float)Mathf.LinearToDb(value));
	}

}
