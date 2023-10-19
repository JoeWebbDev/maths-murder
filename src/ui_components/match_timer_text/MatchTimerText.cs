using Godot;
using System;

/// <summary>
/// Controls a label to display the current state of a <see cref="MatchTimer"/>.
/// </summary>
public partial class MatchTimerText : Label
{
    [Export] public MatchTimer Timer { get; set; }
    private bool _isTimerActive;

    public override void _Ready()
    {
        if (Timer == null)
        {
            GodotLogger.LogWarning($"{nameof(MatchTimerText)} expects a {nameof(MatchTimer)}, but none was found.");
            return;
        }

        Timer.MatchTimerStarted += OnMatchTimerStart;
        Timer.MatchTimerEnded += OnMatchTimerEnded;
    }

    private void OnMatchTimerStart()
    {
        _isTimerActive = true;
    }

    private void OnMatchTimerEnded()
    {
        _isTimerActive = false;
        Text = "0";
    }

    public override void _Process(double delta)
    {
        if (_isTimerActive) Text = (Mathf.CeilToInt(Timer.TimeLeft ?? 0)).ToString();
    }
}