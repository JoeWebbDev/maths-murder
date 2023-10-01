using Godot;
using System;

public partial class MathsMurderButton : Button
{
    [Export] private Texture2D _defaultCursor;
    [Export] private Texture2D _hoverCursor;

    public override void _Ready()
    {
        MouseEntered += () => { ChangeCursor(_hoverCursor); };
        MouseExited += () => { ChangeCursor(_defaultCursor); };
    }

    private void ChangeCursor(Texture2D cursor)
    {
        Input.SetCustomMouseCursor(cursor);
    }
}
