using System.Threading.Tasks;
using Godot;

public partial class ScreenFader : Node
{
	[Export] private ColorRect _colorRect;

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
	}

	public async Task FadeInAsync(double delayBeforeFade = 0.0d, float fadeTime = 0.5f)
	{
		GodotLogger.LogDebug(  "Fading in");
		await TweenAlphaAsync(0.0f, delayBeforeFade, fadeTime);
	}

 	public async Task FadeOutAsync(double delayBeforeFade = 0.0d, float fadeTime = 0.5f)
    {
	    GodotLogger.LogDebug("Fading out");
	    await TweenAlphaAsync(1.0f, delayBeforeFade, fadeTime);
    }

    private async Task TweenAlphaAsync(float finalValue, double delayBeforeFade, float fadeTime)
    {
	    var tween = GetTree().CreateTween();
	    tween.SetPauseMode(Tween.TweenPauseMode.Process);
	    tween.TweenProperty(_colorRect,"modulate:a", finalValue, fadeTime)
		    .SetDelay(delayBeforeFade)
		    .SetTrans(Tween.TransitionType.Linear)
		    .SetEase(Tween.EaseType.InOut);
	    await ToSignal(tween, "finished");
    }
}
