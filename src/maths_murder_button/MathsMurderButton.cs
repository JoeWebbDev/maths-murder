using Godot;
using System;

public partial class MathsMurderButton : Button
{
    [ExportGroup("Internal properties")]
    [Export] private Texture2D _defaultCursor;
    [Export] private Texture2D _hoverCursor;
    [Export] private AudioStream _hoverSfx;
    [Export] private float _hoverDb;

    private GlobalAudioManager _audioManager;

    public override void _Ready()
    {
        _audioManager = GetNode<GlobalAudioManager>("/root/GlobalAudioManager");
        MouseEntered += OnMouseEnter;
        MouseExited += OnMouseExit;
    }

    private void OnMouseEnter()
    {
        _audioManager.PlaySfx(_hoverSfx, _hoverDb);
        ChangeCursor(_hoverCursor);
    }

    private void OnMouseExit()
    {
        ChangeCursor(_defaultCursor);
    }

    private void ChangeCursor(Texture2D cursor)
    {
        Input.SetCustomMouseCursor(cursor);
    }

    public override void _ExitTree()
    {
        ChangeCursor(_defaultCursor);
    }
}
