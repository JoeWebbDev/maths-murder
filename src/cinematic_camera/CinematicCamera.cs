using Godot;
using System;
using System.Threading.Tasks;

public partial class CinematicCamera : Camera2D
{
    [Export] public float PositionLerpSpeed { get; set; }
    [Export] public float ZoomLerpSpeed { get; set; }
    [Export] public float PositionLerpTolerance { get; set; } = 2f;
    [Export] public float ZoomLerpTolerance { get; set; } = 0.1f;

    private Vector2 _viewportSize;
    private bool _isInitialized;
    private Vector2 _destination;
    private Vector2 _zoom;
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        if (_isInitialized) return;
        _viewportSize = GetViewportRect().Size;
        Offset = new Vector2(_viewportSize.X / 2, _viewportSize.Y / 2);
    }

    public void Init(Vector2 position, Vector2 zoom)
    {
        _isInitialized = true;
        Offset = position;
        Zoom = zoom;
    }

    public async Task SendToPosition(Vector2 location, float duration, Tween.TransitionType trans = Tween.TransitionType.Linear, Tween.EaseType ease = Tween.EaseType.OutIn)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "offset", location, duration)
            .SetTrans(trans)
            .SetEase(ease);
        await ToSignal(tween, "finished");
    }

    public async Task SendToZoomValue(Vector2 zoom, float duration, Tween.TransitionType trans = Tween.TransitionType.Linear, Tween.EaseType ease = Tween.EaseType.OutIn)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "zoom", zoom, duration)
            .SetTrans(trans)
            .SetEase(ease);
        await ToSignal(tween, "finished");
    }

    private bool IsCloseEnough(Vector2 x, Vector2 y, float tolerance)
    {
        return Math.Abs(x.X - y.X) < tolerance && Math.Abs(x.Y - y.Y) < tolerance;
    }
}
