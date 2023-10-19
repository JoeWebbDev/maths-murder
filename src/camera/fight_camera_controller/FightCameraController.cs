using Godot;
using System;

public partial class FightCameraController : Node2D
{
    [Export] private Fighter _player;
    [Export] private Fighter _enemy;
    [Export] private float _boundaryFeather;
    [Export] private float _lerpSpeed;
    [Export] public CinematicCamera CinematicCamera { get; private set; }
    // X - left bound, Y - right bound
    [Export] private Vector2 _fightBounds;
    public bool Freeze { get; set; }
    private Vector2 _viewportSize;
    private Vector2 _destination;
    
    public override void _Ready()
    {
        _viewportSize = GetViewportRect().Size;
    }

    public override void _Process(double delta)
    {
        if (Freeze) return;
        
        // 1 - find the distance between player & enemy
        var playerPos = _player.GlobalPosition;
        var enemyPos = _enemy.GlobalPosition;
        var distanceBetweenFighters = new Vector2(Mathf.Abs(playerPos.X - enemyPos.X), 0);
        var sumOfXPositions = playerPos.X + enemyPos.X;
        var xMidpoint = sumOfXPositions != 0 ? sumOfXPositions / 2 : sumOfXPositions;
        
        // While we're here, lets lerp to the midpoint of both players
        CinematicCamera.Offset = CinematicCamera.Offset.Lerp(new Vector2(Mathf.Clamp(xMidpoint, _fightBounds.X, _fightBounds.Y), CinematicCamera.Offset.Y), (float)delta * _lerpSpeed);
    }
}
