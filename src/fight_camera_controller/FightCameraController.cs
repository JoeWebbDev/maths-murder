using Godot;
using System;

public partial class FightCameraController : Node2D
{
    [Export] private Fighter _player;
    [Export] private Fighter _enemy;
    [Export] private float _boundaryFeather;
    [Export] private float _lerpSpeed;
    [Export] public CinematicCamera CinematicCamera { get; private set; }
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
        CinematicCamera.Offset = CinematicCamera.Offset.Lerp(new Vector2(xMidpoint, CinematicCamera.Offset.Y), (float)delta * _lerpSpeed);
        

        // 2 - If distance > viewport size, find multiplier required to get viewport size > distance
        if (distanceBetweenFighters.X + _boundaryFeather <= _viewportSize.X) return;
        
        var multiplier = 1f;
        // For some reason, as the player approaches the edge, the camera can't keep up. Adding a linear factor that
        // increases as the distance increases helps mitigate this
        var multiplierFactor = 1f;
        while (distanceBetweenFighters.X + _boundaryFeather > _viewportSize.X / multiplier)
        {
            multiplier -= 0.01f;
            multiplierFactor -= 0.01f;
        }

        multiplier = Mathf.Clamp(multiplier * multiplierFactor, 0.01f, 1);
        // 3 - set destination to that multiplier
        _destination = new Vector2(multiplier, multiplier);
        
        // 4 - Lerp to destination
        CinematicCamera.Zoom = CinematicCamera.Zoom.Lerp(_destination, (float)delta * _lerpSpeed);
    }
}
