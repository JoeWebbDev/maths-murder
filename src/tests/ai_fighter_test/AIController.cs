using Godot;
using MathsMurderSpike.core.Commands;
using MathsMurderSpike.Core.enums;

public partial class AIController : Node
{
    [Export] public Fighter FighterBeingControlled { get; set; }
    [Export] public FighterResource FighterToLoad { get; set; }
    [Export] public Button LoadButton { get; set; }

    private double _timer;
    private bool _initialized;

    public override void _Ready()
    {
        FighterBeingControlled.HitRegistered += OnHitRegistered;
        LoadButton.Pressed += () =>
        {
            LoadButton.Disabled = true;
            FighterBeingControlled.LoadFighter(FighterToLoad);
            _initialized = true;
            GD.Print("AI fighter initialized");
        };
    }

    private void OnHitRegistered()
    {
        GD.Print($"{FighterBeingControlled.Name} has taken a hit!");
        GD.Print($"Current health: {FighterBeingControlled.Health}");
        FighterBeingControlled.Health -= 10;
        GD.Print($"Health after hit: {FighterBeingControlled.Health}");
    }

    public override void _Process(double delta)
    {
        if (!_initialized)
            return;
        
        _timer += delta;
        if (_timer >= 2)
        {
            GD.Print("Executing punch.");
            FighterBeingControlled.Execute(new PunchCommand());
            _timer = 0;
        }
    }
}
