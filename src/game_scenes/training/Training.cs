using Godot;
using System;
using MathsMurderSpike.Core.FighterStates;
using MathsMurderSpike.core.Utility;

public partial class Training : Node
{
    [Signal] public delegate void NextFightRequestedEventHandler();

    [Export] private NumericStepper _healthStepper;
    [Export] private NumericStepper _speedStepper;
    [Export] private NumericStepper _strengthStepper;
    [Export] private NumericStepper _defenseStepper;
    [Export] private Label _remainingExpPointsLabel;
    [Export] private Button _nextFightButton;
    [Export] private Fighter _fighter;
    private int _remainingExp;
    private double _switchAnimationTimer;
    private string _currentState;
    private int _playCount;
    private GameDataManager _gameDataManager;
    private FighterData _playerDataInstance;
    
    public override void _Ready()
    {
        _gameDataManager = GetNode<GameDataManager>("/root/GameDataManager");
        _playerDataInstance = _gameDataManager.GetPlayerFighterData();
        _remainingExp = _gameDataManager.GetPlayerExperiencePoints();
        if (_remainingExp <= 0) ToggleAddButtons(true);
        UpdateRemainingExpLabel();
        _healthStepper.MinimumValue = (int)_playerDataInstance.Health;
        _speedStepper.MinimumValue = (int)_playerDataInstance.Speed;
        _strengthStepper.MinimumValue = (int)_playerDataInstance.Strength;
        _defenseStepper.MinimumValue = (int)_playerDataInstance.Defense;
        _healthStepper.Value = (int)_playerDataInstance.Health;
        _speedStepper.Value = (int)_playerDataInstance.Speed;
        _strengthStepper.Value = (int)_playerDataInstance.Strength;
        _defenseStepper.Value = (int)_playerDataInstance.Defense;
        _healthStepper.ValueChanged += OnStepperValueChanged;
        _speedStepper.ValueChanged += OnStepperValueChanged;
        _strengthStepper.ValueChanged += OnStepperValueChanged;
        _defenseStepper.ValueChanged += OnStepperValueChanged;
        _nextFightButton.Pressed += OnNextFightButtonPressed;
        _fighter.InitFighter(_playerDataInstance);
        _fighter.AnimationPlayer.Play("idle");
        _currentState = "idle";
    }

    private void OnStepperValueChanged(int from, int to)
    {
        UpdateRemainingExp(from - to);
    }

    private void UpdateRemainingExp(int numberToAdd)
    {
        _remainingExp += numberToAdd;
        ToggleAddButtons(_remainingExp <= 0);
        UpdateRemainingExpLabel();
    }

    private void UpdateRemainingExpLabel()
    {
        _remainingExpPointsLabel.Text = Utilities.FormatAsTwoDigitString(_remainingExp);
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
        _healthStepper.TogglePlusButton(disable);
        _speedStepper.TogglePlusButton(disable);
        _strengthStepper.TogglePlusButton(disable);
        _defenseStepper.TogglePlusButton(disable);
    }
    
    private void OnNextFightButtonPressed()
    {
        _playerDataInstance.Health = _healthStepper.Value;
        _playerDataInstance.Speed = _speedStepper.Value;
        _playerDataInstance.Strength = _strengthStepper.Value;
        _playerDataInstance.Defense = _defenseStepper.Value;
        _gameDataManager.SetPlayerExperience(_remainingExp);
        EmitSignal(SignalName.NextFightRequested);
    }
    
}
