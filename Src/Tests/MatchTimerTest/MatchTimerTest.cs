using Godot;
using System;

public partial class MatchTimerTest : CanvasLayer
{
    [Export] public Button StartTimerButton { get; set; }
    [Export] public MatchTimer Timer { get; set; }

    public override void _Ready()
    {
        StartTimerButton.Pressed += () => { Timer.Start(); };
    }
}