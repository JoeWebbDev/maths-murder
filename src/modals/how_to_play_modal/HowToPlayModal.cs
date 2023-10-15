using Godot;
using System;

public partial class HowToPlayModal : CanvasLayer
{
    [Export] private Button _closeButton;

    public override void _Ready()
    {
        _closeButton.Pressed += Hide;
        Hide();
    }
}
