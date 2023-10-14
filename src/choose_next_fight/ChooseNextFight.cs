using Godot;
using System;

public partial class ChooseNextFight : Node
{
    [Signal] public delegate void FighterSelectedEventHandler();
    
    [Export] private PackedScene _fighterScene;
    [Export] private Vector2 _optionOnePos;
    [Export] private Vector2 _optionTwoPos;
    [Export] private Vector2 _superSecretOptionThreePos;
    [Export] private Button _optionOneButton;
    [Export] private Button _optionTwoButton;
    [Export] private Button _superSecretOptionThreeButton;

    private GameDataManager _gameDataManager;

    public override void _Ready()
    {
        _gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        var playerData = _gameDataManager.GetPlayerData();
        if (playerData.DefeatedOpponents.Count >= playerData.TotalNumberOfFightsPerGame - 1)
        {
            _optionTwoButton.Hide();
            _optionOneButton.Hide();
            _superSecretOptionThreeButton.Show();
            var fighterOption = _fighterScene.Instantiate<Fighter>();
            AddChild(fighterOption);
            var opponentData = _gameDataManager.GetHardestOpponent();
            fighterOption.InitFighter(opponentData);
            fighterOption.GlobalPosition = _superSecretOptionThreePos;
            _superSecretOptionThreeButton.Pressed +=  () => { OnFighterSelected(opponentData); };
            return;
        }
        _superSecretOptionThreeButton.Hide();
        for (var i = 0; i < 2; i++)
        {
            var fighterOption = _fighterScene.Instantiate<Fighter>();
            // i = 0 for left, 1 for right opponent. So we add i * 2 to defeated opponents to get the next highest number,
            // and then skip one take one
            var opponentData = _gameDataManager.GetOpponentDataFromCollection(playerData.DefeatedOpponents.Count + (i * 2));
            AddChild(fighterOption);
            fighterOption.InitFighter(opponentData);
            fighterOption.GlobalPosition = i == 0 ? _optionOnePos : _optionTwoPos;
            fighterOption.FlipH = i != 0;
            var button = i == 0 ? _optionOneButton : _optionTwoButton;
            button.Pressed += () => { OnFighterSelected(opponentData); };
        }
    }
    
    private void OnFighterSelected(FighterData fighterOption)
    {
        _gameDataManager.SetCurrentOpponentData(fighterOption);
        EmitSignal(SignalName.FighterSelected);
    }
}
