using Godot;
using System;

public partial class HealthBar : TextureProgressBar
{
    [Export] public Fighter Fighter { get; set; }

    public override void _Ready()
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
