using Godot;
using MathsMurderSpike.Core.Input;

public partial class HowToPlayModal : CanvasLayer
{
    [Export] private Button _closeButton;
    [Export] private Label _moveLeftKey;
    [Export] private Label _moveRightKey;
    [Export] private Label _punchKey;
    [Export] private Label _kickKey;
    [Export] private Label _blockKey;

    public override void _Ready()
    {
        _moveLeftKey.Text = $"{InputMap.ActionGetEvents(InputAction.MoveLeft)[0].AsText()}";
        _moveRightKey.Text = $"{InputMap.ActionGetEvents(InputAction.MoveRight)[0].AsText()}";
        _punchKey.Text = $"{InputMap.ActionGetEvents(InputAction.Punch)[0].AsText()}";
        _kickKey.Text = $"{InputMap.ActionGetEvents(InputAction.Kick)[0].AsText()}";
        _blockKey.Text = $"{InputMap.ActionGetEvents(InputAction.Block)[0].AsText()}";
        _closeButton.Pressed += Hide;
        Hide();
    }
}
