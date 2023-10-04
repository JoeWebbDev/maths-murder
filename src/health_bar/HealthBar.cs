using Godot;
using System;

public partial class HealthBar : TextureProgressBar
{
    [Export] public Fighter Fighter { get; set; }
    [Export] public FightUI Ui { get; private set; }

    public override void _Ready()
    {
        Ui.FightersInitialized += OnUiReady;
    }

    private void OnUiReady()
    {
        Fighter.HealthChanged += OnHealthChange;
        MaxValue = Fighter.MaxHealth;
        Value = Fighter.Health;
    }

    private void OnHealthChange(int from, int to)
    {
        Value = to;
    }
}
