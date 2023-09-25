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
public partial class MatchTimer : Node
{
	[Signal] public delegate void MatchTimerStartedEventHandler();
	[Signal] public delegate void MatchTimerEndedEventHandler();
	[Export] public int TotalMatchTime { get; set; }
	public double? TimeLeft => _timer?.TimeLeft;
	private SceneTreeTimer _timer;
	private SignalAwaiter _ongoingTask;

	public async Task Start()
	{
		if (_ongoingTask is { IsCompleted: false })
		{
			GD.PushWarning($"Attempting to start a {nameof(MatchTimer)} while one is ongoing is not allowed!");
			return;
		}
		EmitSignal(SignalName.MatchTimerStarted);
		_timer = GetTree().CreateTimer(TotalMatchTime);
		_ongoingTask = ToSignal(_timer, SceneTreeTimer.SignalName.Timeout);
		await _ongoingTask;
		EmitSignal(SignalName.MatchTimerEnded);
	}
}