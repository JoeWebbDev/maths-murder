using Godot;
using System;
using MathsMurderSpike.Core.FighterStates;

public partial class Training : Node
{
    [Signal] public delegate void NextFightRequestedEventHandler();
    [Export] private Button _addHealthButton;
    [Export] private Button _addSpeedButton;
    [Export] private Button _addStrengthButton;
    [Export] private Button _addDefenseButton;
    [Export] private Button _subtractHealthButton;
    [Export] private Button _subtractSpeedButton;
    [Export] private Button _subtractStrengthButton;
    [Export] private Button _subtractDefenseButton;
    [Export] private Label _remainingExpPointsLabel;
    [Export] private Label _healthLabel;
    [Export] private Label _speedLabel;
    [Export] private Label _strengthLabel;
    [Export] private Label _defenseLabel;   
    private int _healthOriginalValue;
    private int _speedOriginalValue;
    private int _strengthOriginalValue;
    private int _defenseOriginalValue;
    [Export] private Button _nextFightButton;
    [Export] private Fighter _fighter;
    private double _switchAnimationTimer;
    private string _currentState;
    private int _playCount;
    
    private GameDataManager _gameDataManager;
    private FighterData _playerDataInstance;
    public override void _Ready()
    {
        _gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        _playerDataInstance = _gameDataManager.GetPlayerFighterData();
        var remainingExp = UpdateRemainingExpLabel();
        if (remainingExp <= 0) ToggleAddButtons(true);
        _healthLabel.Text = _playerDataInstance.Health.ToString();
        _speedLabel.Text = _playerDataInstance.Speed.ToString();
        _strengthLabel.Text = _playerDataInstance.Strength.ToString();
        _defenseLabel.Text = _playerDataInstance.Defense.ToString();
        _healthOriginalValue = (int)_playerDataInstance.Health;
        _speedOriginalValue = (int)_playerDataInstance.Speed;
        _strengthOriginalValue = (int)_playerDataInstance.Strength;
        _defenseOriginalValue = (int)_playerDataInstance.Defense;
        _addHealthButton.Pressed += () => { _playerDataInstance.Health = UpdatePlayerFighterValue(_playerDataInstance.Health, _healthLabel, 1, _subtractHealthButton, _healthOriginalValue); };
        _addSpeedButton.Pressed += () => { _playerDataInstance.Speed = UpdatePlayerFighterValue(_playerDataInstance.Speed, _speedLabel, 1, _subtractSpeedButton, _speedOriginalValue); };
        _addStrengthButton.Pressed += () => { _playerDataInstance.Strength = UpdatePlayerFighterValue(_playerDataInstance.Strength, _strengthLabel, 1, _subtractStrengthButton, _strengthOriginalValue); };
        _addDefenseButton.Pressed += () => { _playerDataInstance.Defense = UpdatePlayerFighterValue(_playerDataInstance.Defense, _defenseLabel, 1, _subtractDefenseButton, _defenseOriginalValue); };
        _subtractHealthButton.Pressed += () => { _playerDataInstance.Health = UpdatePlayerFighterValue(_playerDataInstance.Health, _healthLabel, -1, _subtractHealthButton, _healthOriginalValue); };
        _subtractSpeedButton.Pressed += () => { _playerDataInstance.Speed = UpdatePlayerFighterValue(_playerDataInstance.Speed, _speedLabel, -1, _subtractSpeedButton, _speedOriginalValue); };
        _subtractStrengthButton.Pressed += () => { _playerDataInstance.Strength = UpdatePlayerFighterValue(_playerDataInstance.Strength, _strengthLabel, -1, _subtractStrengthButton, _strengthOriginalValue); };
        _subtractDefenseButton.Pressed += () => { _playerDataInstance.Defense = UpdatePlayerFighterValue(_playerDataInstance.Defense, _defenseLabel, -1, _subtractDefenseButton, _defenseOriginalValue); };
        _subtractHealthButton.Disabled = true;
        _subtractSpeedButton.Disabled = true;
        _subtractStrengthButton.Disabled = true;
        _subtractDefenseButton.Disabled = true;
            
        _nextFightButton.Pressed += () => EmitSignal(SignalName.NextFightRequested);
        _fighter.InitFighter(_playerDataInstance);
        _fighter.AnimationPlayer.Play("idle");
        _currentState = "idle";
    }
    
    private float UpdatePlayerFighterValue(float valueToAddTo, Label _labelToUpdate, int numberToAdd, Button buttonToCheckForUpdate, int buttonOriginalValue)
    {
        valueToAddTo += numberToAdd;
        _labelToUpdate.Text = valueToAddTo.ToString();
        if (numberToAdd < 0)
        {
            if (valueToAddTo <= buttonOriginalValue)
                buttonToCheckForUpdate.Disabled = true;
            _gameDataManager.DecreasePlayerExperience(numberToAdd);
        }
        else
        {
            _gameDataManager.IncreasePlayerExperience(-numberToAdd);
            buttonToCheckForUpdate.Disabled = false;
        }
        var remainingExp = UpdateRemainingExpLabel();

        if (remainingExp <= 0) 
            ToggleAddButtons(true);
        else
            ToggleAddButtons(false);
        return valueToAddTo;
    }

    private int UpdateRemainingExpLabel()
    {
        var remainingExp = _gameDataManager.GetPlayerExperiencePoints();
        var label = remainingExp.ToString();
        if (label.Length < 2)
            label = "0" + label;
        _remainingExpPointsLabel.Text = label;
        return remainingExp;
    }

    public override void _Process(double delta)
    {
        _switchAnimationTimer += delta;
        if (_switchAnimationTimer > 1)
        {
            if (_currentState == "punch")
                _fighter.SwitchCombatState(new PunchState());
            _switchAnimationTimer = 0;
            _playCount++;
        }
        
        if (_playCount >= 5)
            _currentState = _currentState == "idle" ? "punch" : "idle";
        
        GodotLogger.LogDebug("playcount " + _playCount);
        GodotLogger.LogDebug("currentanim " + _currentState);
        base._Process(delta);
    }

    private void ToggleAddButtons(bool disable)
    {
        _addHealthButton.Disabled = disable;
        _addSpeedButton.Disabled = disable;
        _addStrengthButton.Disabled = disable;
        _addDefenseButton.Disabled = disable;
    }

    private void ToggleButton(Button button, bool enable)
    {
        button.Disabled = enable;
    }
}
