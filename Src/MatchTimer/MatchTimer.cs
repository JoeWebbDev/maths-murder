using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// Creates a timer for the match and, on timer complete, emits a signal <see cref="MatchTimerEnded"/>.
/// Based on <a href="https://docs.godotengine.org/en/stable/classes/class_scenetree.html#class-scenetree-method-create-timer">SceneTree.CreateTimer</a>.
/// <para></para>
/// <b>Possible improvements:</b> should we be decoupling the timer from the UI text, or is this OK?
/// Is there a better way to update the text (currently polling every frame in <see cref="_Process"/>)?
/// </summary>
public partial class MatchTimer : Label
{
	[Signal] public delegate void MatchTimerEndedEventHandler();
	[Export] public int MatchTime { get; set; }
	[Export] public bool StartOnReady { get; set; }
	private SceneTreeTimer _timer;
	private bool _isActive;
	public override void _Ready()
	{
		if (StartOnReady) Start();
	}

	public async Task Start()
	{
		_timer = GetTree().CreateTimer(MatchTime);
		_isActive = true;
		await ToSignal(_timer, SceneTreeTimer.SignalName.Timeout);
		Text = "0";
		EmitSignal(SignalName.MatchTimerEnded);
		_isActive = false;
	}

	public override void _Process(double delta)
	{
		if (_isActive) Text = (Mathf.CeilToInt(_timer.TimeLeft)).ToString();
	}
}