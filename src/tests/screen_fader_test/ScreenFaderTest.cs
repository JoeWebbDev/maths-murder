using Godot;
using System;

public partial class ScreenFaderTest : Node
{
	[Export] private Button _fadeInButton;
	[Export] private Button _fadeOutButton;
	[Export] private ScreenFader _fader;
	public override void _Ready()
	{
		_fadeInButton.Pressed += () => _fader.FadeInAsync();
		_fadeOutButton.Pressed += () => _fader.FadeOutAsync();
	}
}
