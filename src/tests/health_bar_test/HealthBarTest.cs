using Godot;
using System;

public partial class HealthBarTest : Node
{
    [Export] private Button _playerDamageButton;
    [Export] private Fighter _player;
    [Export] private Button _enemyDamageButton;
    [Export] private Fighter _enemy;

    public override void _Ready()
    {
        _playerDamageButton.Pressed += () => { _player.Health -= 10; };
        _enemyDamageButton.Pressed += () => { _enemy.Health -= 10; };
    }
}
