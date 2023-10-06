using Godot;
using System;

public partial class Training : Node
{
    [Export] private Button _addHealthButton;
    [Export] private Button _addSpeedButton;
    [Export] private Button _addStrengthButton;
    [Export] private Button _addDefenseButton;
    [Export] private Label _remainingExpPointsLabel;
    [Export] private Label _healthLabel;
    [Export] private Label _speedLabel;
    [Export] private Label _strengthLabel;
    [Export] private Label _defenseLabel;
    [Export] private Button _nextFightButton;
    
    private GameDataManager _gameDataManager;
    private FighterData _playerDataInstance;
    public override void _Ready()
    {
        _gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        _playerDataInstance = _gameDataManager.GetPlayerFighterData();
        var remainingExp = _gameDataManager.GetPlayerExperienceValue();
        _remainingExpPointsLabel.Text = $"EXP Points Available: {remainingExp}";
        if (remainingExp <= 0) DisableAddButtons();
        _healthLabel.Text = _playerDataInstance.Health.ToString();
        _speedLabel.Text = _playerDataInstance.Speed.ToString();
        _strengthLabel.Text = _playerDataInstance.Strength.ToString();
        _defenseLabel.Text = _playerDataInstance.Defense.ToString();
        _addHealthButton.Pressed += () => { _playerDataInstance.Health = IncrementPlayerFighterValue(_playerDataInstance.Health, _healthLabel); };
        _addSpeedButton.Pressed += () => { _playerDataInstance.Speed = IncrementPlayerFighterValue(_playerDataInstance.Speed, _speedLabel); };
        _addStrengthButton.Pressed += () => { _playerDataInstance.Strength = IncrementPlayerFighterValue(_playerDataInstance.Strength, _strengthLabel); };
        _addDefenseButton.Pressed += () => { _playerDataInstance.Defense = IncrementPlayerFighterValue(_playerDataInstance.Defense, _defenseLabel); };
    }
    
    // We pass by reference so we can update the value on the player data directly
    private int IncrementPlayerFighterValue(int value, Label _labelToUpdate)
    {
        value += 1;
        _labelToUpdate.Text = value.ToString();
        _gameDataManager.DecreasePlayerExperience(1);
        var remainingExp = _gameDataManager.GetPlayerExperienceValue();
        _remainingExpPointsLabel.Text = $"EXP Points Available: {remainingExp}";
        if (remainingExp <= 0) DisableAddButtons();
        return value;
    }

    private void DisableAddButtons()
    {
        _addHealthButton.Disabled = true;
        _addSpeedButton.Disabled = true;
        _addStrengthButton.Disabled = true;
        _addDefenseButton.Disabled = true;
    }
}
