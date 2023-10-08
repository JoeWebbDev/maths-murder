using Godot;
using System;

public partial class ChooseNextFight : Node
{
    [Signal] public delegate void FighterSelectedEventHandler();
    
    [Export] private PackedScene _fighterScene;
    [Export] private Vector2 _optionOnePos;
    [Export] private Vector2 _optionTwoPos;
    [Export] private Button _optionOneButton;
    [Export] private Button _optionTwoButton;

    private GameDataManager _gameDataManager;

    public override void _Ready()
    {
        _gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        for (var i = 0; i < 2; i++)
        {
            var fighterOption = _fighterScene.Instantiate<Fighter>();
            // We would have to figure out a smarter way rather than selected a random fighter!
            var randomFighter = _gameDataManager.GetRandomOpponentData(false);
            fighterOption.InitFighter(randomFighter);
            fighterOption.GlobalPosition = i == 0 ? _optionOnePos : _optionTwoPos;
            fighterOption.FlipH = i != 0;
            var button = i == 0 ? _optionOneButton : _optionTwoButton;
            button.Pressed += () => { OnFighterSelected(randomFighter); };
            AddChild(fighterOption);
        }
    }
    
    private void OnFighterSelected(FighterData fighterOption)
    {
        _gameDataManager.SetCurrentOpponentData(fighterOption);
        EmitSignal(SignalName.FighterSelected);
    }
}
