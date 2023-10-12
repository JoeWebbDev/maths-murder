using Godot;
using System;

public partial class PlayerDeath : Node2D
{
    [Signal] public delegate void PlayAgainRequestedEventHandler();
    [Export] private Button _playAgainButton;
    [Export] private AnimationPlayer _animPlayer;
    [Export] private PackedScene _fighterScene;
    [Export] private float _defeatedOpponentScale = 0.2f;
    [Export] private Container _defeatedOpponentsContainer;
    [Export] private int _maxDefeatedOpponentsInARow;
    [Export] private Vector2 _defeatedOpponentsOffset;
    [Export] private Label _damageDealtLabel;
    [Export] private Label _damageTakenLabel;
    [Export] private Label _experiencePointsGainedLabel;

    public override void _Ready()
    {
        _playAgainButton.Pressed += OnPlayAgainPressed;
        var playerData = GetNode<GameDataManager>("/root/GameDataManager").GetPlayerData();
        var defeatedOpponents = playerData.DefeatedOpponents;
        for (var i = 0; i < defeatedOpponents.Count; i++)
        {
            var opponentData = defeatedOpponents[i];
            var fighter = _fighterScene.Instantiate<Fighter>();
            var yOffset = _defeatedOpponentsOffset.Y * (i == 0 ? 0 : i / _maxDefeatedOpponentsInARow);
            var xOffset = _defeatedOpponentsOffset.X * (i % _maxDefeatedOpponentsInARow);
            var offset = new Vector2(xOffset, yOffset);
            fighter.Scale = new Vector2(_defeatedOpponentScale, _defeatedOpponentScale);
            fighter.Position = new Vector2(_defeatedOpponentsContainer.Position.X + offset.X,
                (_defeatedOpponentsContainer.Position.Y - _defeatedOpponentsContainer.Size.Y) + offset.Y);
            _defeatedOpponentsContainer.AddChild(fighter);
            fighter.InitFighter(opponentData);
        }
        _damageDealtLabel.Text = playerData.DamageDealt.ToString();
        _damageTakenLabel.Text = playerData.DamageTaken.ToString();
        _experiencePointsGainedLabel.Text = playerData.TotalExpGained.ToString();
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
