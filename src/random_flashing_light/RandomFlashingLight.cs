using Godot;
using System;

public partial class RandomFlashingLight : PointLight2D
{
    [Export] private Vector2 _minMaxEnergy;
    [Export] private Vector2 _minMaxTimeUntilFlash;
    [Export] private Vector2 _minMaxFlashDuration;

    private float _elapsedTimeSinceFlash;
    private float _nextFlashTime;

    public override void _Ready()
    {
        Energy = _minMaxEnergy.X;
        
        RollForNextFlashTime();
    }

    public override void _Process(double delta)
    {
        _elapsedTimeSinceFlash += (float)delta;
        if (_elapsedTimeSinceFlash >= _nextFlashTime)
        {
            Flash();
        }
    }

    // Naive quick implementation for rolling a pseudo-random time between min and max _minMaxTimeUntilFlash.
    // (Max time - min time) * 0.0-1.0 + min time
    private void RollForNextFlashTime()
    {
        _nextFlashTime = RandfRange(_minMaxTimeUntilFlash);
    }

    private async void Flash()
    {
        var flashDuration = RandfRange(_minMaxFlashDuration);
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "energy", _minMaxEnergy.Y, flashDuration / 2);
        tween.TweenProperty(this, "energy", _minMaxEnergy.X, flashDuration / 2);
        await ToSignal(tween, "finished");
        RollForNextFlashTime();
        _elapsedTimeSinceFlash = 0f;
    }
    
    // I know this exists already, i found this quicker, go away >:(
    private float RandfRange (Vector2 range) => (range.Y - range.X) * GD.Randf() + range.X;
}
