using Godot;
using System;

public partial class PlayerDeath : Node2D
{
    [Signal] public delegate void PlayAgainRequestedEventHandler();
    [Export] private Button _playAgainButton;
    [Export] private Label _numOfEnemiesDefeated;
    [Export] private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        _playAgainButton.Pressed += OnPlayAgainPressed;
        _numOfEnemiesDefeated.Text = GetNode<GameDataManager>("/root/GameDataManager").GetPlayerData().DefeatedOpponents.Count.ToString();
    }

    public void PlayAnimations()
    {
        _animPlayer.Play("initial_death");
        _animPlayer.AnimationFinished += _ => _animPlayer.Play("arm_bleed");
    }

    private void OnPlayAgainPressed()
    {
        EmitSignal(SignalName.PlayAgainRequested);
    }
}
