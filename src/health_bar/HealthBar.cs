using Godot;
using System;

public partial class HealthBar : Node2D
{
    private static class ShaderParameters
    {
        public static string FillAmount = "fill_amount";
        public static string UnderlyingColorFillAmount = "underlying_color_fill_amount";
    }
    [Export] public Fighter Fighter { get; set; }
    [Export] public FightUI Ui { get; private set; }

    [Export] private float _underlyingColorDelayTime;
    [Export] private float _underlyingColorSpeed;

    [ExportGroup("Internal scene properties")]
    [Export] private Sprite2D _border;
    [Export] private Sprite2D _fill;

    private float _maxValue;
    private float _value;
    private float _underlyingValue;
    private float _currentUnderlyingValueDelay;
    private bool _isUpdatingUnderlyingValue;
    private ShaderMaterial _fillShader;
    
    public float CurrentHealthDecimal => _value > 0 ? (float)_value / _maxValue : 0;

    public override void _Ready()
    {
        Ui.FightersInitialized += OnUiReady;
        _fillShader = (_fill.Material as ShaderMaterial);
    }

    public override void _Process(double delta)
    {
        UpdateShaderUnderlyingColor((float)delta);
    }

    private void OnUiReady()
    {
        Fighter.HealthChanged += OnHealthChange;
        _maxValue = Fighter.MaxHealth;
        _value = Fighter.Health;
        _underlyingValue = Fighter.Health;
        UpdateShader();
    }

    private void OnHealthChange(int from, int to)
    {
        _value = to;
        UpdateShader();
        TryUpdateBorderFrame();
    }

    private void UpdateShader()
    {
        _fillShader.SetShaderParameter(ShaderParameters.FillAmount, CurrentHealthDecimal);
        if (!_isUpdatingUnderlyingValue) _currentUnderlyingValueDelay = _underlyingColorDelayTime;
    }

    private void TryUpdateBorderFrame()
    {
        var targetFrame = Mathf.Abs(Mathf.CeilToInt(CurrentHealthDecimal * 4) - 4);
        if (_border.Frame != targetFrame) _border.Frame = targetFrame;
    }

    private void UpdateShaderUnderlyingColor(float delta)
    {
        if (_underlyingValue <= _value)
        {
            _isUpdatingUnderlyingValue = false;
            return;
        }
        if (_currentUnderlyingValueDelay > 0)
        {
            _currentUnderlyingValueDelay -= delta;
            return;
        }
        _isUpdatingUnderlyingValue = true;
        _underlyingValue -= delta * _underlyingColorSpeed;
        var underlyingValueAsFraction = _underlyingValue > 0 ? _underlyingValue / _maxValue : 0;
        _fillShader.SetShaderParameter(ShaderParameters.UnderlyingColorFillAmount, underlyingValueAsFraction);
    }
}
